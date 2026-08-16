Public Class MainForm

    Private _databaseConnectionManager As DatabaseConnectionManager

    Private _monitoringService As MonitoringService
    Private _defaultNotifyIcon As Icon

    ' Indique si l'application a réellement le droit de se fermer.
    ' Lorsque l'utilisateur clique sur X, nous voulons simplement
    ' masquer la fenêtre.
    Private _allowClose As Boolean = False


    Private Sub ShowSettings()

        Using form As New SettingsForm()

            If form.ShowDialog(Me) = DialogResult.OK Then

                ' Pour l'instant, on conserve le gestionnaire
                ' dans MainForm.
                ' Nous l'utiliserons ensuite pour MonitoringService.

                If form.ConnectionManager IsNot Nothing Then

                    ' À ajouter en haut de MainForm :
                    '
                    ' Private _databaseConnectionManager
                    '     As DatabaseConnectionManager

                    _databaseConnectionManager =
                    form.ConnectionManager

                End If

            End If

        End Using

    End Sub
    Private Sub MonitoringStatusChanged(result As MonitoringResult)

        If Me.InvokeRequired Then

            Me.Invoke(
            New Action(
                Sub()
                    MonitoringStatusChanged(result)
                End Sub
            )
        )

            Return

        End If


        If myVysualAlarmNotifyIcon.ContextMenuStrip Is Nothing Then
            Return
        End If


        Dim menu As ContextMenuStrip = myVysualAlarmNotifyIcon.ContextMenuStrip
        menu.Items.Clear()

        Dim hasAlarm As Boolean = result.AlertCount > 0 OrElse result.Message.StartsWith("Erreur", StringComparison.OrdinalIgnoreCase)
        Dim statusItem As New ToolStripMenuItem(result.Message) With {.ForeColor = If(hasAlarm, Color.Red, Color.Green), .Enabled = False}
        menu.Items.Add(statusItem)

        If result.IsHistoricalDemoMode Then
            menu.Items.Add(New ToolStripMenuItem("Mode démonstration : données historiques recalées") With {.Enabled = False, .ForeColor = Color.DarkOrange})
        End If

        For Each client As MonitoringAlertClient In result.AlertClients
            Dim clientName As String = If(String.IsNullOrWhiteSpace(client.ClientName), client.ClientId, client.ClientName)
            Dim detail As String = If(client.LastActivity.HasValue, $"Dernière transmission : {client.LastActivity:dd.MM.yyyy HH:mm}", "Aucune donnée transmise")
            Dim alertItem As New ToolStripMenuItem($"🔴 Client en alarme : {clientName}") With {.ForeColor = Color.Red, .Font = New Font(SystemFonts.MenuFont, FontStyle.Bold), .ToolTipText = detail}
            AddHandler alertItem.Click, Sub()
                                            MessageBox.Show($"Client : {clientName}{Environment.NewLine}{detail}", "Alerte de surveillance", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        End Sub
            menu.Items.Add(alertItem)
        Next

        menu.Items.Add(New ToolStripSeparator())
        Dim openItem As New ToolStripMenuItem("Ouvrir myVysualAlarm")
        AddHandler openItem.Click, Sub() ShowMainWindow()
        menu.Items.Add(openItem)
        Dim exitItem As New ToolStripMenuItem("Quitter")
        AddHandler exitItem.Click, Sub()
                                       _allowClose = True
                                       Application.Exit()
                                   End Sub
        menu.Items.Add(exitItem)

        myVysualAlarmNotifyIcon.Icon = If(hasAlarm, SystemIcons.Error, _defaultNotifyIcon)
        myVysualAlarmNotifyIcon.Text = If(hasAlarm, $"myVysualAlarm — {result.AlertCount} alarme(s)", "myVysualAlarm — Surveillance OK")
    End Sub
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Configuration de l'icône dans la zone de notification
        If myVysualAlarmNotifyIcon.Icon IsNot Nothing Then
            Icon = myVysualAlarmNotifyIcon.Icon
            _defaultNotifyIcon = myVysualAlarmNotifyIcon.Icon
        End If

        myVysualAlarmNotifyIcon.Visible = True
        myVysualAlarmNotifyIcon.Text = "myVysualAlarm"

        ' Création du menu contextuel
        Dim menu As New ContextMenuStrip()

        ' Élément indiquant l'état actuel
        Dim statusItem As New ToolStripMenuItem("🟢 Surveillance active")
        'statusItem.Enabled = False


        ' Séparateur
        Dim separator As New ToolStripSeparator()

        ' Élément permettant d'ouvrir la fenêtre
        Dim openItem As New ToolStripMenuItem("Ouvrir myVysualAlarm")

        AddHandler openItem.Click,
            Sub()
                ShowMainWindow()
            End Sub

        ' Élément permettant de quitter
        Dim exitItem As New ToolStripMenuItem("Quitter")

        AddHandler exitItem.Click,
            Sub()
                _allowClose = True
                Application.Exit()
            End Sub

        ' Construction du menu
        menu.Items.Add(statusItem)
        menu.Items.Add(separator)
        menu.Items.Add(openItem)
        menu.Items.Add(exitItem)

        ' Association du menu à l'icône
        myVysualAlarmNotifyIcon.ContextMenuStrip = menu

        ' Création et démarrage du service de surveillance
        _monitoringService = New MonitoringService()

        AddHandler _monitoringService.StatusChanged,
            AddressOf MonitoringStatusChanged

        _monitoringService.Start()

    End Sub


    Private Sub settingsMenuItem_Click(
        sender As Object,
        e As EventArgs
    ) Handles settingsMenuItem.Click

        ShowSettings()

    End Sub


    Private Sub exitMenuItem_Click(
        sender As Object,
        e As EventArgs
    ) Handles exitMenuItem.Click

        _allowClose = True
        Application.Exit()

    End Sub


    Private Sub myVysualAlarmNotifyIcon_DoubleClick(
        sender As Object,
        e As EventArgs
    ) Handles myVysualAlarmNotifyIcon.DoubleClick

        ShowMainWindow()

    End Sub


    Private Sub MainForm_FormClosing(
        sender As Object,
        e As FormClosingEventArgs
    ) Handles MyBase.FormClosing

        If Not _allowClose Then

            ' L'utilisateur a cliqué sur X.
            ' On annule réellement la fermeture et on masque simplement
            ' la fenêtre.
            e.Cancel = True
            Me.Hide()

        End If

    End Sub


    Private Sub ShowMainWindow()

        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.Activate()

    End Sub

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Me.Hide()

    End Sub
End Class
