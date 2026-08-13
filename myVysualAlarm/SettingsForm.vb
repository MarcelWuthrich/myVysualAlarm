Imports System.ComponentModel
Imports System.IO

Public Class SettingsForm

    ''' <summary>
    ''' Paramètres finalement validés.
    ''' MainForm pourra les récupérer après DialogResult.OK.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Settings As AppSettings


    ''' <summary>
    ''' Gestionnaire de connexion.
    ''' Il reste vivant après la fermeture du formulaire
    ''' si la connexion est conservée par l'application.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ConnectionManager As DatabaseConnectionManager


    Private Sub SettingsForm_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        ApplySettingsToForm(AppSettingsStore.Load())

        ' Masquer les mots de passe
        txtDatabasePassword.UseSystemPasswordChar = True
        txtSshPassphrase.UseSystemPasswordChar = True

        chkShowDatabasePassword.Checked = False
        chkShowSshPassphrase.Checked = False

        ' État de la connexion
        lblConnectionStatus.Text = ""
        lblConnectionStatus.ForeColor = SystemColors.ControlText
        lblConnectionStatus.Visible = False

        ' Barre de progression
        progressBar.Visible = False

        ' Activer/désactiver la partie SSH
        UpdateSshControls()

        ConfigureProfessionalLayout()

    End Sub


    ''' <summary>
    ''' Organise les contrôles générés par le concepteur dans une interface
    ''' plus lisible, sans modifier le comportement de la fenêtre.
    ''' </summary>
    Private Sub ConfigureProfessionalLayout()

        Const contentLeft As Integer = 28
        Const labelLeft As Integer = 22
        Const inputLeft As Integer = 185
        Const inputWidth As Integer = 410

        BackColor = Color.FromArgb(246, 248, 251)
        Font = New Font("Segoe UI", 9.0F)
        ClientSize = New Size(650, 720)
        Padding = New Padding(contentLeft)
        Text = "Paramètres de connexion"

        lblTitle.Font = New Font("Segoe UI Semibold", 16.0F)
        lblTitle.ForeColor = Color.FromArgb(31, 41, 55)
        lblTitle.Location = New Point(contentLeft, 22)
        lblSubtitle.ForeColor = Color.FromArgb(90, 99, 112)
        lblSubtitle.Location = New Point(contentLeft, 55)
        grpDatabase.Font = New Font("Segoe UI Semibold", 9.0F)
        grpDatabase.ForeColor = Color.FromArgb(31, 41, 55)
        grpDatabase.Location = New Point(contentLeft, 88)
        grpDatabase.Size = New Size(594, 245)

        ConfigureField(lblDatabaseServer, txtDatabaseServer, "Serveur ou adresse IP", labelLeft, inputLeft, 36, inputWidth)
        ConfigureField(lblDatabasePort, nudDatabasePort, "Port", labelLeft, inputLeft, 74, 120)
        nudDatabasePort.Maximum = 65535
        ConfigureField(lblDatabaseName, txtDatabaseName, "Nom de la base", labelLeft, inputLeft, 112, inputWidth)
        ConfigureField(lblDatabaseUser, txtDatabaseUser, "Utilisateur", labelLeft, inputLeft, 150, inputWidth)
        ConfigureField(lblDatabasePassword, txtDatabasePassword, "Mot de passe", labelLeft, inputLeft, 188, 280)

        chkShowDatabasePassword.Text = "Afficher"
        chkShowDatabasePassword.AutoSize = True
        chkShowDatabasePassword.Location = New Point(inputLeft + 290, 190)

        chkUseSsh.Text = "Utiliser un tunnel SSH sécurisé"
        chkUseSsh.Font = New Font("Segoe UI Semibold", 9.0F)
        chkUseSsh.Location = New Point(contentLeft, 355)

        grpSsh.Text = "Connexion SSH"
        grpSsh.Font = New Font("Segoe UI Semibold", 9.0F)
        grpSsh.ForeColor = Color.FromArgb(31, 41, 55)
        grpSsh.Location = New Point(contentLeft, 385)
        grpSsh.Size = New Size(594, 225)
        grpSsh.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        grpSsh.Controls.Add(btnBrowsePrivateKey)

        ConfigureField(lblSshServer, txtSshServer, "Serveur SSH", labelLeft, inputLeft, 32, inputWidth)
        ConfigureField(lblSshPort, nudSshPort, "Port SSH", labelLeft, inputLeft, 70, 120)
        nudSshPort.Maximum = 65535
        ConfigureField(lblSshUser, txtSshUser, "Utilisateur SSH", labelLeft, inputLeft, 108, inputWidth)
        ConfigureField(lblSshPrivateKey, txtSshPrivateKey, "Clé privée", labelLeft, inputLeft, 146, inputWidth - 100)
        ConfigureField(lblSshPassphrase, txtSshPassphrase, "Phrase secrète", labelLeft, inputLeft, 184, 280)

        btnBrowsePrivateKey.Text = "Parcourir…"
        btnBrowsePrivateKey.Location = New Point(inputLeft + inputWidth - 88, 144)
        btnBrowsePrivateKey.Size = New Size(88, 28)

        chkShowSshPassphrase.Text = "Afficher"
        chkShowSshPassphrase.AutoSize = True
        chkShowSshPassphrase.Location = New Point(inputLeft + 290, 186)

        lblConnectionStatus.AutoEllipsis = True
        lblConnectionStatus.AutoSize = False
        lblConnectionStatus.BorderStyle = BorderStyle.FixedSingle
        lblConnectionStatus.ForeColor = Color.FromArgb(75, 85, 99)
        lblConnectionStatus.Location = New Point(contentLeft, 625)
        lblConnectionStatus.Padding = New Padding(10, 6, 10, 6)
        lblConnectionStatus.Size = New Size(594, 32)
        lblConnectionStatus.Text = ""
        lblConnectionStatus.Visible = False

        progressBar.Location = New Point(contentLeft, 663)
        progressBar.Size = New Size(594, 5)

        btnTestConnection.Text = "Tester la connexion"
        btnTestConnection.Location = New Point(contentLeft, 680)
        btnTestConnection.Size = New Size(155, 30)
        btnTestConnection.FlatStyle = FlatStyle.System

        btnSave.Text = "Enregistrer"
        btnSave.Location = New Point(445, 680)
        btnSave.Size = New Size(82, 30)
        btnSave.FlatStyle = FlatStyle.System

        btnCancel.Text = "Fermer"
        btnCancel.Location = New Point(535, 680)
        btnCancel.Size = New Size(87, 30)
        btnCancel.FlatStyle = FlatStyle.System
        AcceptButton = btnSave
        CancelButton = btnCancel

    End Sub


    Private Sub ConfigureField(
        label As Label,
        input As Control,
        labelText As String,
        labelX As Integer,
        inputX As Integer,
        y As Integer,
        width As Integer
    )

        label.Text = labelText
        label.AutoSize = True
        label.Font = New Font("Segoe UI", 9.0F)
        label.ForeColor = Color.FromArgb(55, 65, 81)
        label.Location = New Point(labelX, y + 5)

        input.Location = New Point(inputX, y)
        input.Size = New Size(width, 27)
        input.Font = New Font("Segoe UI", 9.0F)

    End Sub


    ''' <summary>
    ''' Active ou désactive la partie SSH.
    ''' </summary>
    Private Sub chkUseSsh_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkUseSsh.CheckedChanged

        UpdateSshControls()

    End Sub


    Private Sub UpdateSshControls()

        grpSsh.Enabled = chkUseSsh.Checked

    End Sub


    ''' <summary>
    ''' Affichage / masquage du mot de passe MariaDB.
    ''' </summary>
    Private Sub chkShowDatabasePassword_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkShowDatabasePassword.CheckedChanged

        txtDatabasePassword.UseSystemPasswordChar =
            Not chkShowDatabasePassword.Checked

    End Sub


    ''' <summary>
    ''' Affichage / masquage de la passphrase SSH.
    ''' </summary>
    Private Sub chkShowSshPassphrase_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkShowSshPassphrase.CheckedChanged

        txtSshPassphrase.UseSystemPasswordChar =
            Not chkShowSshPassphrase.Checked

    End Sub


    ''' <summary>
    ''' Sélection du fichier de clé privée SSH.
    ''' </summary>
    Private Sub btnBrowsePrivateKey_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnBrowsePrivateKey.Click

        Using dialog As New OpenFileDialog()

            dialog.Title = "Sélectionner la clé privée SSH"

            dialog.Filter =
                "Clés SSH (*.pem;*.ppk;*.key)|*.pem;*.ppk;*.key|" &
                "Tous les fichiers (*.*)|*.*"

            If dialog.ShowDialog(Me) = DialogResult.OK Then

                txtSshPrivateKey.Text =
                    dialog.FileName

            End If

        End Using

    End Sub


    ''' <summary>
    ''' Teste la connexion sans fermer le formulaire.
    ''' </summary>
    Private Async Sub btnTestConnection_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTestConnection.Click

        Await TestConnectionAsync()

    End Sub


    Private Async Function TestConnectionAsync() As Task

        Dim manager As DatabaseConnectionManager = Nothing

        Try

            ' Création des paramètres à partir du formulaire
            Dim settings As AppSettings =
                CreateSettingsFromForm()

            SetBusy(True)
            lblConnectionStatus.Visible = True

            lblConnectionStatus.Text =
                "Test de connexion en cours..."

            lblConnectionStatus.ForeColor =
                Color.DarkOrange


            ' Gestionnaire temporaire.
            ' Il sera fermé après le test.
            manager = New DatabaseConnectionManager()

            Dim connection =
                Await manager.ConnectAsync(settings)


            If connection IsNot Nothing AndAlso
               connection.State =
                   System.Data.ConnectionState.Open Then

                lblConnectionStatus.Text =
                    "✓ Connexion réussie"

                lblConnectionStatus.ForeColor =
                    Color.Green

            Else

                lblConnectionStatus.Text =
                    "✗ Connexion impossible"

                lblConnectionStatus.ForeColor =
                    Color.Red

            End If


        Catch ex As Exception

            lblConnectionStatus.Text =
                "✗ " & GetFriendlyErrorMessage(ex)

            lblConnectionStatus.ForeColor =
                Color.Red

        Finally

            ' Le test ne doit pas laisser une connexion
            ' ou un tunnel SSH ouvert.
            If manager IsNot Nothing Then
                manager.Dispose()
            End If

            SetBusy(False)

        End Try

    End Function


    ''' <summary>
    ''' Crée un AppSettings à partir des champs du formulaire.
    ''' </summary>
    Private Function CreateSettingsFromForm() As AppSettings

        Dim settings As New AppSettings()

        ' ========================================================
        ' MariaDB
        ' ========================================================

        settings.DatabaseServer =
            txtDatabaseServer.Text.Trim()

        settings.DatabasePort =
            CInt(nudDatabasePort.Value)

        settings.DatabaseName =
            txtDatabaseName.Text.Trim()

        settings.DatabaseUser =
            txtDatabaseUser.Text.Trim()

        settings.DatabasePassword =
            txtDatabasePassword.Text


        ' ========================================================
        ' SSH
        ' ========================================================

        settings.UseSshTunnel =
            chkUseSsh.Checked


        If chkUseSsh.Checked Then

            settings.SshServer =
                txtSshServer.Text.Trim()

            settings.SshPort =
                CInt(nudSshPort.Value)

            settings.SshUser =
                txtSshUser.Text.Trim()

            settings.SshPrivateKeyFile =
                txtSshPrivateKey.Text.Trim()

            settings.SshPassphrase =
                txtSshPassphrase.Text

        End If


        Return settings

    End Function


    ''' <summary>
    ''' Enregistre les valeurs saisies sans ouvrir de connexion réseau.
    ''' </summary>
    Private Sub btnSave_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSave.Click

        AppSettingsStore.Save(CreateSettingsFromForm())

    End Sub


    Private Sub ApplySettingsToForm(settings As AppSettings)

        txtDatabaseServer.Text = settings.DatabaseServer
        nudDatabasePort.Value = settings.DatabasePort
        txtDatabaseName.Text = settings.DatabaseName
        txtDatabaseUser.Text = settings.DatabaseUser
        txtDatabasePassword.Text = settings.DatabasePassword
        chkUseSsh.Checked = settings.UseSshTunnel
        txtSshServer.Text = settings.SshServer
        nudSshPort.Value = settings.SshPort
        txtSshUser.Text = settings.SshUser
        txtSshPrivateKey.Text = settings.SshPrivateKeyFile
        txtSshPassphrase.Text = settings.SshPassphrase

    End Sub
    ''' <summary>
    ''' Active ou désactive les contrôles pendant une opération réseau.
    ''' </summary>
    Private Sub SetBusy(busy As Boolean)

        progressBar.Visible = busy

        btnTestConnection.Enabled = Not busy
        btnSave.Enabled = Not busy
        btnCancel.Enabled = Not busy

        chkUseSsh.Enabled = Not busy

        If busy Then
            Cursor = Cursors.WaitCursor
        Else
            Cursor = Cursors.Default
        End If

    End Sub


    ''' <summary>
    ''' Transforme les exceptions techniques en messages
    ''' compréhensibles par l'utilisateur.
    ''' </summary>
    Private Function GetFriendlyErrorMessage(
        ex As Exception
    ) As String

        If TypeOf ex Is FileNotFoundException Then

            Return ex.Message

        End If


        If TypeOf ex Is MySqlConnector.MySqlException Then

            Return "Erreur MariaDB : " & ex.Message

        End If


        If TypeOf ex Is Renci.SshNet.Common.SshAuthenticationException Then

            Return "Échec de l'authentification SSH. " &
                   "Vérifiez l'utilisateur, la clé privée et sa passphrase."

        End If


        If TypeOf ex Is Renci.SshNet.Common.SshConnectionException Then

            Return "Impossible d'établir la connexion SSH : " &
                   ex.Message

        End If


        If TypeOf ex Is TimeoutException Then

            Return "Délai d'attente dépassé. " &
                   "Le serveur est peut-être inaccessible."

        End If


        If TypeOf ex Is ArgumentException Then

            Return ex.Message

        End If


        Return ex.Message

    End Function


    ''' <summary>
    ''' Annulation.
    ''' </summary>
    Private Sub btnCancel_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnCancel.Click

        Me.DialogResult =
            DialogResult.Cancel

        Me.Close()

    End Sub


    Private Sub SettingsForm_FormClosing(
        sender As Object,
        e As FormClosingEventArgs
    ) Handles MyBase.FormClosing

        ' Si le formulaire est fermé sans connexion validée,
        ' le gestionnaire doit être fermé.
        '
        ' Si DialogResult = OK, le gestionnaire appartient
        ' désormais à MainForm et ne doit PAS être fermé ici.

        If Me.DialogResult <> DialogResult.OK Then

            If Me.ConnectionManager IsNot Nothing Then

                Me.ConnectionManager.Dispose()

                Me.ConnectionManager = Nothing

            End If

        End If

    End Sub

End Class
