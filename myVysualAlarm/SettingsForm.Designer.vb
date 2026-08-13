<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsForm
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        lblDatabaseServer = New Label()
        txtDatabaseServer = New TextBox()
        lblDatabasePort = New Label()
        nudDatabasePort = New NumericUpDown()
        lblDatabaseName = New Label()
        txtDatabaseName = New TextBox()
        txtDatabaseUser = New TextBox()
        lblDatabaseUser = New Label()
        txtDatabasePassword = New TextBox()
        lblDatabasePassword = New Label()
        btnTestConnection = New Button()
        btnSave = New Button()
        btnCancel = New Button()
        lblConnectionStatus = New Label()
        chkUseSsh = New CheckBox()
        grpSsh = New GroupBox()
        chkShowSshPassphrase = New CheckBox()
        txtSshPassphrase = New TextBox()
        lblSshPassphrase = New Label()
        txtSshUser = New TextBox()
        txtSshPrivateKey = New TextBox()
        lblSshPrivateKey = New Label()
        lblSshUser = New Label()
        nudSshPort = New NumericUpDown()
        lblSshPort = New Label()
        txtSshServer = New TextBox()
        lblSshServer = New Label()
        progressBar = New ProgressBar()
        chkShowDatabasePassword = New CheckBox()
        btnBrowsePrivateKey = New Button()
        CType(nudDatabasePort, ComponentModel.ISupportInitialize).BeginInit()
        grpSsh.SuspendLayout()
        CType(nudSshPort, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lblDatabaseServer
        ' 
        lblDatabaseServer.AutoSize = True
        lblDatabaseServer.Location = New Point(25, 25)
        lblDatabaseServer.Name = "lblDatabaseServer"
        lblDatabaseServer.Size = New Size(73, 15)
        lblDatabaseServer.TabIndex = 0
        lblDatabaseServer.Text = "Serveur / IP :"
        ' 
        ' txtDatabaseServer
        ' 
        txtDatabaseServer.Location = New Point(154, 22)
        txtDatabaseServer.Name = "txtDatabaseServer"
        txtDatabaseServer.Size = New Size(216, 23)
        txtDatabaseServer.TabIndex = 1
        ' 
        ' lblDatabasePort
        ' 
        lblDatabasePort.AutoSize = True
        lblDatabasePort.Location = New Point(25, 60)
        lblDatabasePort.Name = "lblDatabasePort"
        lblDatabasePort.Size = New Size(35, 15)
        lblDatabasePort.TabIndex = 2
        lblDatabasePort.Text = "Port :"
        ' 
        ' nudDatabasePort
        ' 
        nudDatabasePort.Location = New Point(154, 58)
        nudDatabasePort.Maximum = New Decimal(New Integer() {50000, 0, 0, 0})
        nudDatabasePort.Name = "nudDatabasePort"
        nudDatabasePort.Size = New Size(216, 23)
        nudDatabasePort.TabIndex = 3
        ' 
        ' lblDatabaseName
        ' 
        lblDatabaseName.AutoSize = True
        lblDatabaseName.Location = New Point(25, 97)
        lblDatabaseName.Name = "lblDatabaseName"
        lblDatabaseName.Size = New Size(101, 15)
        lblDatabaseName.TabIndex = 4
        lblDatabaseName.Text = "Base de données :"
        ' 
        ' txtDatabaseName
        ' 
        txtDatabaseName.Location = New Point(154, 94)
        txtDatabaseName.Name = "txtDatabaseName"
        txtDatabaseName.Size = New Size(216, 23)
        txtDatabaseName.TabIndex = 5
        ' 
        ' txtDatabaseUser
        ' 
        txtDatabaseUser.Location = New Point(154, 129)
        txtDatabaseUser.Name = "txtDatabaseUser"
        txtDatabaseUser.Size = New Size(216, 23)
        txtDatabaseUser.TabIndex = 7
        ' 
        ' lblDatabaseUser
        ' 
        lblDatabaseUser.AutoSize = True
        lblDatabaseUser.Location = New Point(25, 132)
        lblDatabaseUser.Name = "lblDatabaseUser"
        lblDatabaseUser.Size = New Size(66, 15)
        lblDatabaseUser.TabIndex = 6
        lblDatabaseUser.Text = "Utilisateur :"
        ' 
        ' txtDatabasePassword
        ' 
        txtDatabasePassword.Location = New Point(154, 165)
        txtDatabasePassword.Name = "txtDatabasePassword"
        txtDatabasePassword.Size = New Size(216, 23)
        txtDatabasePassword.TabIndex = 9
        txtDatabasePassword.UseSystemPasswordChar = True
        ' 
        ' lblDatabasePassword
        ' 
        lblDatabasePassword.AutoSize = True
        lblDatabasePassword.Location = New Point(25, 168)
        lblDatabasePassword.Name = "lblDatabasePassword"
        lblDatabasePassword.Size = New Size(83, 15)
        lblDatabasePassword.TabIndex = 8
        lblDatabasePassword.Text = "Mot de passe :"
        ' 
        ' btnTestConnection
        ' 
        btnTestConnection.Location = New Point(154, 250)
        btnTestConnection.Name = "btnTestConnection"
        btnTestConnection.Size = New Size(216, 23)
        btnTestConnection.TabIndex = 10
        btnTestConnection.Text = "Tester la connexion"
        btnTestConnection.UseVisualStyleBackColor = True
        ' 
        ' btnSave
        ' 
        btnSave.Location = New Point(154, 290)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(75, 23)
        btnSave.TabIndex = 11
        btnSave.Text = "Enregistrer"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.Location = New Point(295, 290)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(75, 23)
        btnCancel.TabIndex = 12
        btnCancel.Text = "Annuler"
        btnCancel.UseVisualStyleBackColor = True
        ' 
        ' lblConnectionStatus
        ' 
        lblConnectionStatus.AutoSize = True
        lblConnectionStatus.Location = New Point(154, 211)
        lblConnectionStatus.Name = "lblConnectionStatus"
        lblConnectionStatus.Size = New Size(124, 15)
        lblConnectionStatus.TabIndex = 13
        lblConnectionStatus.Text = "Statut de la connexion"
        ' 
        ' chkUseSsh
        ' 
        chkUseSsh.AutoSize = True
        chkUseSsh.Location = New Point(25, 345)
        chkUseSsh.Name = "chkUseSsh"
        chkUseSsh.Size = New Size(140, 19)
        chkUseSsh.TabIndex = 14
        chkUseSsh.Text = "Utiliser un tunnel SSH"
        chkUseSsh.UseVisualStyleBackColor = True
        ' 
        ' grpSsh
        ' 
        grpSsh.Controls.Add(chkShowSshPassphrase)
        grpSsh.Controls.Add(txtSshPassphrase)
        grpSsh.Controls.Add(lblSshPassphrase)
        grpSsh.Controls.Add(txtSshUser)
        grpSsh.Controls.Add(txtSshPrivateKey)
        grpSsh.Controls.Add(lblSshPrivateKey)
        grpSsh.Controls.Add(lblSshUser)
        grpSsh.Controls.Add(nudSshPort)
        grpSsh.Controls.Add(lblSshPort)
        grpSsh.Controls.Add(txtSshServer)
        grpSsh.Controls.Add(lblSshServer)
        grpSsh.Location = New Point(25, 415)
        grpSsh.Name = "grpSsh"
        grpSsh.Size = New Size(356, 252)
        grpSsh.TabIndex = 15
        grpSsh.TabStop = False
        grpSsh.Text = "Paramètres SSH"
        ' 
        ' chkShowSshPassphrase
        ' 
        chkShowSshPassphrase.AutoSize = True
        chkShowSshPassphrase.Location = New Point(25, 214)
        chkShowSshPassphrase.Name = "chkShowSshPassphrase"
        chkShowSshPassphrase.Size = New Size(84, 19)
        chkShowSshPassphrase.TabIndex = 10
        chkShowSshPassphrase.Text = "CheckBox1"
        chkShowSshPassphrase.UseVisualStyleBackColor = True
        ' 
        ' txtSshPassphrase
        ' 
        txtSshPassphrase.Location = New Point(152, 180)
        txtSshPassphrase.Name = "txtSshPassphrase"
        txtSshPassphrase.Size = New Size(100, 23)
        txtSshPassphrase.TabIndex = 9
        ' 
        ' lblSshPassphrase
        ' 
        lblSshPassphrase.AutoSize = True
        lblSshPassphrase.Location = New Point(17, 179)
        lblSshPassphrase.Name = "lblSshPassphrase"
        lblSshPassphrase.Size = New Size(96, 15)
        lblSshPassphrase.TabIndex = 8
        lblSshPassphrase.Text = "lblSshPassphrase"
        ' 
        ' txtSshUser
        ' 
        txtSshUser.Location = New Point(157, 90)
        txtSshUser.Name = "txtSshUser"
        txtSshUser.Size = New Size(100, 23)
        txtSshUser.TabIndex = 7
        ' 
        ' txtSshPrivateKey
        ' 
        txtSshPrivateKey.Location = New Point(162, 114)
        txtSshPrivateKey.Name = "txtSshPrivateKey"
        txtSshPrivateKey.Size = New Size(100, 23)
        txtSshPrivateKey.TabIndex = 6
        ' 
        ' lblSshPrivateKey
        ' 
        lblSshPrivateKey.AutoSize = True
        lblSshPrivateKey.Location = New Point(43, 117)
        lblSshPrivateKey.Name = "lblSshPrivateKey"
        lblSshPrivateKey.Size = New Size(93, 15)
        lblSshPrivateKey.TabIndex = 5
        lblSshPrivateKey.Text = "lblSshPrivateKey"
        ' 
        ' lblSshUser
        ' 
        lblSshUser.AutoSize = True
        lblSshUser.Location = New Point(43, 93)
        lblSshUser.Name = "lblSshUser"
        lblSshUser.Size = New Size(61, 15)
        lblSshUser.TabIndex = 4
        lblSshUser.Text = "lblSshUser"
        ' 
        ' nudSshPort
        ' 
        nudSshPort.Location = New Point(110, 63)
        nudSshPort.Name = "nudSshPort"
        nudSshPort.Size = New Size(120, 23)
        nudSshPort.TabIndex = 3
        ' 
        ' lblSshPort
        ' 
        lblSshPort.AutoSize = True
        lblSshPort.Location = New Point(28, 62)
        lblSshPort.Name = "lblSshPort"
        lblSshPort.Size = New Size(60, 15)
        lblSshPort.TabIndex = 2
        lblSshPort.Text = "lblSshPort"
        ' 
        ' txtSshServer
        ' 
        txtSshServer.Location = New Point(104, 26)
        txtSshServer.Name = "txtSshServer"
        txtSshServer.Size = New Size(100, 23)
        txtSshServer.TabIndex = 1
        ' 
        ' lblSshServer
        ' 
        lblSshServer.AutoSize = True
        lblSshServer.Location = New Point(16, 29)
        lblSshServer.Name = "lblSshServer"
        lblSshServer.Size = New Size(70, 15)
        lblSshServer.TabIndex = 0
        lblSshServer.Text = "lblSshServer"
        ' 
        ' progressBar
        ' 
        progressBar.Location = New Point(118, 376)
        progressBar.Name = "progressBar"
        progressBar.Size = New Size(100, 23)
        progressBar.Style = ProgressBarStyle.Marquee
        progressBar.TabIndex = 16
        progressBar.Visible = False
        ' 
        ' chkShowDatabasePassword
        ' 
        chkShowDatabasePassword.AutoSize = True
        chkShowDatabasePassword.Location = New Point(460, 169)
        chkShowDatabasePassword.Name = "chkShowDatabasePassword"
        chkShowDatabasePassword.Size = New Size(84, 19)
        chkShowDatabasePassword.TabIndex = 17
        chkShowDatabasePassword.Text = "CheckBox1"
        chkShowDatabasePassword.UseVisualStyleBackColor = True
        ' 
        ' btnBrowsePrivateKey
        ' 
        btnBrowsePrivateKey.Location = New Point(522, 121)
        btnBrowsePrivateKey.Name = "btnBrowsePrivateKey"
        btnBrowsePrivateKey.Size = New Size(75, 23)
        btnBrowsePrivateKey.TabIndex = 18
        btnBrowsePrivateKey.Text = "btnBrowsePrivateKey"
        btnBrowsePrivateKey.UseVisualStyleBackColor = True
        ' 
        ' SettingsForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(747, 742)
        Controls.Add(btnBrowsePrivateKey)
        Controls.Add(chkShowDatabasePassword)
        Controls.Add(progressBar)
        Controls.Add(grpSsh)
        Controls.Add(chkUseSsh)
        Controls.Add(lblConnectionStatus)
        Controls.Add(btnCancel)
        Controls.Add(btnSave)
        Controls.Add(btnTestConnection)
        Controls.Add(txtDatabasePassword)
        Controls.Add(lblDatabasePassword)
        Controls.Add(txtDatabaseUser)
        Controls.Add(lblDatabaseUser)
        Controls.Add(txtDatabaseName)
        Controls.Add(lblDatabaseName)
        Controls.Add(nudDatabasePort)
        Controls.Add(lblDatabasePort)
        Controls.Add(txtDatabaseServer)
        Controls.Add(lblDatabaseServer)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "SettingsForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Paramètres"
        CType(nudDatabasePort, ComponentModel.ISupportInitialize).EndInit()
        grpSsh.ResumeLayout(False)
        grpSsh.PerformLayout()
        CType(nudSshPort, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblDatabaseServer As Label
    Friend WithEvents txtDatabaseServer As TextBox
    Friend WithEvents lblDatabasePort As Label
    Friend WithEvents nudDatabasePort As NumericUpDown
    Friend WithEvents lblDatabaseName As Label
    Friend WithEvents txtDatabaseName As TextBox
    Friend WithEvents txtDatabaseUser As TextBox
    Friend WithEvents lblDatabaseUser As Label
    Friend WithEvents txtDatabasePassword As TextBox
    Friend WithEvents lblDatabasePassword As Label
    Friend WithEvents btnTestConnection As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblConnectionStatus As Label
    Friend WithEvents chkUseSsh As CheckBox
    Friend WithEvents grpSsh As GroupBox
    Friend WithEvents lblSshPort As Label
    Friend WithEvents txtSshServer As TextBox
    Friend WithEvents lblSshServer As Label
    Friend WithEvents lblSshPassphrase As Label
    Friend WithEvents txtSshUser As TextBox
    Friend WithEvents txtSshPrivateKey As TextBox
    Friend WithEvents lblSshPrivateKey As Label
    Friend WithEvents lblSshUser As Label
    Friend WithEvents nudSshPort As NumericUpDown
    Friend WithEvents chkShowSshPassphrase As CheckBox
    Friend WithEvents txtSshPassphrase As TextBox
    Friend WithEvents progressBar As ProgressBar
    Friend WithEvents chkShowDatabasePassword As CheckBox
    Friend WithEvents btnBrowsePrivateKey As Button
End Class
