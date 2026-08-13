Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports MySqlConnector
Imports Renci.SshNet

Public Class DatabaseConnectionManager
    Implements IDisposable


    Private _sshClient As SshClient = Nothing

    Private _forwardedPort As ForwardedPortLocal = Nothing

    Private _databaseConnection As MySqlConnection = Nothing


    ''' <summary>
    ''' Connexion MariaDB.
    ''' En mode direct, la connexion se fait directement.
    ''' En mode SSH, un tunnel est créé avant la connexion MariaDB.
    ''' </summary>
    Public Async Function ConnectAsync(
        settings As AppSettings
    ) As Task(Of MySqlConnection)

        ' Nettoyage d'une éventuelle connexion précédente
        Disconnect()


        If settings Is Nothing Then
            Throw New ArgumentNullException(NameOf(settings))
        End If


        ' Validation générale
        ValidateSettings(settings)


        If settings.UseSshTunnel Then

            ' ----------------------------------------------------
            ' Connexion via tunnel SSH
            ' ----------------------------------------------------

            Await Task.Run(
                Sub()
                    CreateSshTunnel(settings)
                End Sub
            )

            Try

                ' La connexion MariaDB se fait maintenant
                ' sur le port local du tunnel.
                Dim connectionString As String =
                    BuildConnectionString(
                        "127.0.0.1",
                        _forwardedPort.BoundPort,
                        settings
                    )

                _databaseConnection =
                    New MySqlConnection(connectionString)

                Await _databaseConnection.OpenAsync()

                Return _databaseConnection

            Catch

                Disconnect()

                Throw

            End Try


        Else

            ' ----------------------------------------------------
            ' Connexion directe
            ' ----------------------------------------------------

            Dim connectionString As String =
                BuildConnectionString(
                    settings.DatabaseServer,
                    settings.DatabasePort,
                    settings
                )

            _databaseConnection =
                New MySqlConnection(connectionString)

            Try

                Await _databaseConnection.OpenAsync()

                Return _databaseConnection

            Catch

                Disconnect()

                Throw

            End Try

        End If

    End Function


    ''' <summary>
    ''' Crée le tunnel SSH.
    ''' Cette méthode est exécutée dans un thread secondaire.
    ''' </summary>
    Private Sub CreateSshTunnel(settings As AppSettings)

        Dim privateKey As New PrivateKeyFile(
            settings.SshPrivateKeyFile,
            settings.SshPassphrase
        )


        Dim authenticationMethod As New PrivateKeyAuthenticationMethod(
            settings.SshUser,
            privateKey
        )


        Dim connectionInfo As New ConnectionInfo(
            settings.SshServer,
            settings.SshPort,
            settings.SshUser,
            authenticationMethod
        )


        _sshClient = New SshClient(connectionInfo)


        ' Timeout SSH
        _sshClient.ConnectionInfo.Timeout =
            TimeSpan.FromSeconds(10)


        ' Connexion SSH
        _sshClient.Connect()


        If Not _sshClient.IsConnected Then

            Throw New InvalidOperationException(
                "La connexion SSH n'a pas pu être établie."
            )

        End If


        ' --------------------------------------------------------
        ' Recherche d'un port local libre
        ' --------------------------------------------------------

        Dim localPort As Integer =
            settings.SshLocalPort

        If localPort <= 0 Then
            localPort = FindFreeTcpPort()
        End If


        ' --------------------------------------------------------
        ' Création du forwarding
        '
        ' localhost:localPort
        '       ↓
        ' serveur MariaDB:DatabasePort
        '       via SSH
        ' --------------------------------------------------------

        _forwardedPort =
            New ForwardedPortLocal(
                "127.0.0.1",
                CType(localPort, UInteger),
                settings.DatabaseServer,
                CType(settings.DatabasePort, UInteger)
            )


        _sshClient.AddForwardedPort(_forwardedPort)


        _forwardedPort.Start()


        If Not _forwardedPort.IsStarted Then

            Throw New InvalidOperationException(
                "Le tunnel SSH n'a pas pu être démarré."
            )

        End If

    End Sub


    ''' <summary>
    ''' Construit la chaîne de connexion MariaDB.
    ''' </summary>
    Private Function BuildConnectionString(
        server As String,
        port As Integer,
        settings As AppSettings
    ) As String

        Dim builder As New MySqlConnectionStringBuilder()

        builder.Server = server
        builder.Port = CType(port, UInt32)
        builder.Database = settings.DatabaseName
        builder.UserID = settings.DatabaseUser
        builder.Password = settings.DatabasePassword

        ' Timeout de connexion
        builder.ConnectionTimeout = 10

        Return builder.ConnectionString

    End Function


    ''' <summary>
    ''' Vérifie les paramètres avant toute tentative de connexion.
    ''' </summary>
    Private Sub ValidateSettings(settings As AppSettings)

        If String.IsNullOrWhiteSpace(settings.DatabaseServer) Then
            Throw New ArgumentException(
                "Le serveur MariaDB est obligatoire."
            )
        End If


        If settings.DatabasePort < 1 OrElse
           settings.DatabasePort > 65535 Then

            Throw New ArgumentException(
                "Le port MariaDB doit être compris entre 1 et 65535."
            )

        End If


        If String.IsNullOrWhiteSpace(settings.DatabaseName) Then
            Throw New ArgumentException(
                "Le nom de la base de données est obligatoire."
            )
        End If


        If String.IsNullOrWhiteSpace(settings.DatabaseUser) Then
            Throw New ArgumentException(
                "L'utilisateur MariaDB est obligatoire."
            )
        End If


        If settings.UseSshTunnel Then

            If String.IsNullOrWhiteSpace(settings.SshServer) Then
                Throw New ArgumentException(
                    "Le serveur SSH est obligatoire."
                )
            End If


            If settings.SshPort < 1 OrElse
               settings.SshPort > 65535 Then

                Throw New ArgumentException(
                    "Le port SSH doit être compris entre 1 et 65535."
                )

            End If


            If String.IsNullOrWhiteSpace(settings.SshUser) Then
                Throw New ArgumentException(
                    "L'utilisateur SSH est obligatoire."
                )
            End If


            If String.IsNullOrWhiteSpace(
                settings.SshPrivateKeyFile
            ) Then

                Throw New ArgumentException(
                    "Le fichier de clé privée SSH est obligatoire."
                )

            End If


            If Not IO.File.Exists(
                settings.SshPrivateKeyFile
            ) Then

                Throw New FileNotFoundException(
                    "Le fichier de clé privée SSH est introuvable.",
                    settings.SshPrivateKeyFile
                )

            End If

        End If

    End Sub


    ''' <summary>
    ''' Recherche un port TCP local disponible.
    ''' </summary>
    Private Function FindFreeTcpPort() As Integer

        Using listener As New TcpListener(
            IPAddress.Loopback,
            0
        )

            listener.Start()

            Dim port As Integer =
                CType(
                    DirectCast(
                        listener.LocalEndpoint,
                        IPEndPoint
                    ).Port,
                    Integer
                )

            listener.Stop()

            Return port

        End Using

    End Function


    ''' <summary>
    ''' Ferme proprement la connexion MariaDB
    ''' et le tunnel SSH.
    ''' </summary>
    Public Sub Disconnect()

        ' MariaDB
        If _databaseConnection IsNot Nothing Then

            Try
                _databaseConnection.Close()
            Catch
            End Try

            Try
                _databaseConnection.Dispose()
            Catch
            End Try

            _databaseConnection = Nothing

        End If


        ' Forwarding SSH
        If _forwardedPort IsNot Nothing Then

            Try
                If _forwardedPort.IsStarted Then
                    _forwardedPort.Stop()
                End If
            Catch
            End Try

            _forwardedPort = Nothing

        End If


        ' SSH
        If _sshClient IsNot Nothing Then

            Try
                If _sshClient.IsConnected Then
                    _sshClient.Disconnect()
                End If
            Catch
            End Try

            Try
                _sshClient.Dispose()
            Catch
            End Try

            _sshClient = Nothing

        End If

    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose

        Disconnect()

    End Sub

End Class
