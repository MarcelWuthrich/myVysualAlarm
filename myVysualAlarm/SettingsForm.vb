Imports System.ComponentModel
Imports System.IO
Imports MySqlConnector

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

    Private tabSettings As TabControl
    Private tabConnection As TabPage
    Private tabClients As TabPage
    Private tabParameters As TabPage
    Private clientsGrid As DataGridView
    Private btnSelectAllClients As Button
    Private btnDeselectAllClients As Button
    Private btnReloadClients As Button
    Private lblClientsInfo As Label
    Private _clientsLoaded As Boolean
    Private _isLoadingClients As Boolean
    Private txtAlertAfterInactivityMinutes As TextBox


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
        ConfigureTabs()

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

        lblConnectionStatus.BorderStyle = BorderStyle.FixedSingle
        lblConnectionStatus.ForeColor = Color.FromArgb(75, 85, 99)
        lblConnectionStatus.Location = New Point(contentLeft, 625)
        lblConnectionStatus.Size = New Size(594, 32)
        lblConnectionStatus.Multiline = True
        lblConnectionStatus.ReadOnly = True
        lblConnectionStatus.ScrollBars = ScrollBars.Vertical
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


    Private Sub ConfigureTabs()

        ClientSize = New Size(670, 820)
        tabSettings = New TabControl() With {.Location = New Point(10, 10), .Size = New Size(650, 750), .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right}
        tabConnection = New TabPage("Connexion DB") With {.BackColor = BackColor}
        tabClients = New TabPage("Clients") With {.BackColor = BackColor}
        tabParameters = New TabPage("Paramètres") With {.BackColor = BackColor}
        tabSettings.TabPages.AddRange(New TabPage() {tabConnection, tabClients, tabParameters})

        tabConnection.Controls.AddRange(New Control() {lblTitle, lblSubtitle, grpDatabase, chkUseSsh, grpSsh, lblConnectionStatus, progressBar, btnTestConnection})
        Controls.Add(tabSettings)

        btnSave.Location = New Point(465, 770)
        btnCancel.Location = New Point(555, 770)
        Controls.Add(btnSave)
        Controls.Add(btnCancel)

        clientsGrid = New DataGridView() With {.Location = New Point(24, 62), .Size = New Size(580, 575), .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right, .AllowUserToAddRows = False, .AllowUserToDeleteRows = False, .AllowUserToResizeRows = False, .AutoGenerateColumns = False, .BackgroundColor = Color.White, .BorderStyle = BorderStyle.FixedSingle, .RowHeadersVisible = False}
        clientsGrid.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "Selected", .HeaderText = "Surveiller", .Width = 85})
        clientsGrid.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ClientName", .HeaderText = "Client", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .ReadOnly = True})
        clientsGrid.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ClientId", .Visible = False})

        lblClientsInfo = New Label() With {.AutoSize = True, .Location = New Point(24, 24), .Text = "Sélectionnez les clients à inclure dans la surveillance."}
        btnSelectAllClients = New Button() With {.Text = "Tout sélectionner", .Location = New Point(24, 655), .Size = New Size(125, 30), .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left}
        btnDeselectAllClients = New Button() With {.Text = "Tout désélectionner", .Location = New Point(157, 655), .Size = New Size(140, 30), .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left}
        btnReloadClients = New Button() With {.Text = "Relire la configuration DB", .Location = New Point(315, 655), .Size = New Size(200, 30), .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right}
        tabClients.Controls.AddRange(New Control() {lblClientsInfo, clientsGrid, btnSelectAllClients, btnDeselectAllClients, btnReloadClients})

        Dim lblAlertAfterInactivityMinutes As New Label() With {.AutoSize = True, .Location = New Point(24, 32), .Text = "Alerte après x minutes d'inactivité"}
        txtAlertAfterInactivityMinutes = New TextBox() With {.Location = New Point(24, 58), .Size = New Size(120, 27), .Text = AppSettingsStore.Load().AlertAfterInactivityMinutes.ToString()}
        tabParameters.Controls.AddRange(New Control() {lblAlertAfterInactivityMinutes, txtAlertAfterInactivityMinutes})

        AddHandler tabSettings.SelectedIndexChanged, AddressOf tabSettings_SelectedIndexChanged
        AddHandler clientsGrid.CurrentCellDirtyStateChanged, AddressOf clientsGrid_CurrentCellDirtyStateChanged
        AddHandler clientsGrid.CellValueChanged, AddressOf clientsGrid_CellValueChanged
        AddHandler btnSelectAllClients.Click, AddressOf btnSelectAllClients_Click
        AddHandler btnDeselectAllClients.Click, AddressOf btnDeselectAllClients_Click
        AddHandler btnReloadClients.Click, AddressOf btnReloadClients_Click

    End Sub


    Private Async Sub tabSettings_SelectedIndexChanged(sender As Object, e As EventArgs)
        If tabSettings.SelectedTab Is tabClients AndAlso Not _clientsLoaded Then Await LoadClientsAsync(False)
    End Sub


    Private Async Function LoadClientsAsync(useDatabaseDefaults As Boolean) As Task

        _isLoadingClients = True
        lblClientsInfo.Text = "Chargement des clients..."
        clientsGrid.Rows.Clear()

        Try
            Dim hasSavedSelection As Boolean = False
            Dim savedIds As HashSet(Of String) = AppSettingsStore.LoadSelectedClientIds(hasSavedSelection)

            Using manager As New DatabaseConnectionManager()
                Dim connection As MySqlConnection = Await manager.ConnectAsync(CreateSettingsFromForm())
                Using command As New MySqlCommand("SELECT ety_id, ety_name, ety_alert_in_monitoring FROM gbl_entity WHERE parent_id IS NOT NULL ORDER BY ety_name", connection)
                    Using reader As MySqlDataReader = Await command.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim clientId As String = Convert.ToString(reader("ety_id"))
                            Dim isEnabledInDatabase As Boolean = IsMonitoringEnabled(reader("ety_alert_in_monitoring"))
                            ' La base fournit le choix initial ; une sélection locale existante
                            ' est prioritaire lors des ouvertures suivantes.
                            Dim selected As Boolean = If(useDatabaseDefaults OrElse Not hasSavedSelection, isEnabledInDatabase, savedIds.Contains(clientId))
                            clientsGrid.Rows.Add(selected, reader.GetString("ety_name"), clientId)
                        End While
                    End Using
                End Using
            End Using

            _clientsLoaded = True
            lblClientsInfo.Text = $"{clientsGrid.Rows.Count} client(s) chargé(s)."
            If useDatabaseDefaults OrElse Not hasSavedSelection Then
                _isLoadingClients = False
                SaveClientSelection()
            End If

        Catch ex As Exception
            lblClientsInfo.Text = "Impossible de charger les clients : " & GetFriendlyErrorMessage(ex)
        Finally
            _isLoadingClients = False
        End Try

    End Function


    Private Sub SaveClientSelection()
        If _isLoadingClients Then Return
        Dim selectedIds As New List(Of String)()
        For Each row As DataGridViewRow In clientsGrid.Rows
            If Convert.ToBoolean(row.Cells("Selected").Value) Then selectedIds.Add(Convert.ToString(row.Cells("ClientId").Value))
        Next
        AppSettingsStore.SaveSelectedClientIds(selectedIds)
    End Sub


    Private Function IsMonitoringEnabled(value As Object) As Boolean

        If value Is Nothing OrElse value Is DBNull.Value Then Return False

        Dim textValue As String = Convert.ToString(value).Trim()
        Return String.Equals(textValue, "1", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(textValue, "true", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(textValue, "yes", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(textValue, "y", StringComparison.OrdinalIgnoreCase)

    End Function


    Private Sub clientsGrid_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If clientsGrid.IsCurrentCellDirty Then clientsGrid.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub


    Private Sub clientsGrid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.ColumnIndex = 0 Then SaveClientSelection()
    End Sub


    Private Sub btnSelectAllClients_Click(sender As Object, e As EventArgs)
        SetAllClientsSelected(True)
    End Sub


    Private Sub btnDeselectAllClients_Click(sender As Object, e As EventArgs)
        SetAllClientsSelected(False)
    End Sub


    Private Sub SetAllClientsSelected(selected As Boolean)
        _isLoadingClients = True
        For Each row As DataGridViewRow In clientsGrid.Rows
            row.Cells("Selected").Value = selected
        Next
        _isLoadingClients = False
        SaveClientSelection()
    End Sub


    Private Async Sub btnReloadClients_Click(sender As Object, e As EventArgs)
        Await LoadClientsAsync(True)
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

        Dim alertAfterInactivityMinutes As Integer
        If Integer.TryParse(txtAlertAfterInactivityMinutes.Text, alertAfterInactivityMinutes) AndAlso alertAfterInactivityMinutes > 0 Then
            settings.AlertAfterInactivityMinutes = alertAfterInactivityMinutes
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
