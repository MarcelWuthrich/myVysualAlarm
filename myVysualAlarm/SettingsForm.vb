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

        ' Valeurs par défaut
        nudDatabasePort.Value = 3306
        nudSshPort.Value = 22

        ' Masquer les mots de passe
        txtDatabasePassword.UseSystemPasswordChar = True
        txtSshPassphrase.UseSystemPasswordChar = True

        chkShowDatabasePassword.Checked = False
        chkShowSshPassphrase.Checked = False

        ' État de la connexion
        lblConnectionStatus.Text = ""
        lblConnectionStatus.ForeColor = SystemColors.ControlText

        ' Barre de progression
        progressBar.Visible = False

        ' Activer/désactiver la partie SSH
        UpdateSshControls()

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


    Private Async Sub btnSave_Click(
    sender As Object,
    e As EventArgs
) Handles btnSave.Click

        Await ConnectAndSaveAsync()

    End Sub

    Private Async Function ConnectAndSaveAsync() As Task

        Dim manager As DatabaseConnectionManager = Nothing

        Try

            Dim settings As AppSettings =
            CreateSettingsFromForm()

            SetBusy(True)

            lblConnectionStatus.Text =
            "Connexion en cours..."

            lblConnectionStatus.ForeColor =
            Color.DarkOrange

            manager = New DatabaseConnectionManager()

            Await manager.ConnectAsync(settings)

            ' La connexion est valide.
            Me.Settings = settings
            Me.ConnectionManager = manager

            ' Le gestionnaire est maintenant transféré
            ' à MainForm.
            manager = Nothing

            lblConnectionStatus.Text =
            "✓ Connexion réussie"

            lblConnectionStatus.ForeColor =
            Color.Green

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception

            If manager IsNot Nothing Then
                manager.Dispose()
                manager = Nothing
            End If

            lblConnectionStatus.Text =
            "✗ " & GetFriendlyErrorMessage(ex)

            lblConnectionStatus.ForeColor =
            Color.Red

        Finally

            If manager IsNot Nothing Then
                manager.Dispose()
            End If

            SetBusy(False)

        End Try

    End Function
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
