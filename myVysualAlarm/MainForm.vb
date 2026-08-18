Imports System.Runtime.InteropServices

Public Class MainForm

    Private _databaseConnectionManager As DatabaseConnectionManager

    Private _monitoringService As MonitoringService
    Private _defaultNotifyIcon As Icon
    Private _greenNotifyIcon As Icon
    Private _redNotifyIcon As Icon
    Private ReadOnly _blinkTimer As New Timer() With {.Interval = 500}
    Private ReadOnly _activeAlertClientIds As New HashSet(Of String)(StringComparer.Ordinal)
    Private ReadOnly _acknowledgedAlertClientIds As New HashSet(Of String)(StringComparer.Ordinal)
    Private _isBlinkVisible As Boolean

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function DestroyIcon(handle As IntPtr) As Boolean
    End Function

    ' Indique si l'application a réellement le droit de se fermer.
    ' Lorsque l'utilisateur clique sur X, nous voulons simplement
    ' masquer la fenêtre.
    Private _allowClose As Boolean = False


    Private Sub ShowSettings()

        Using form As New SettingsForm()

            AddHandler form.SettingsSaved,
                Sub()
                    If _monitoringService IsNot Nothing Then
                        _monitoringService.CheckNow()
                    End If
                End Sub

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


        Dim activeIds As New HashSet(Of String)(result.AlertClients.Select(Function(client) client.ClientId), StringComparer.Ordinal)
        _acknowledgedAlertClientIds.IntersectWith(activeIds)
        _activeAlertClientIds.Clear()
        _activeAlertClientIds.UnionWith(activeIds)

        Dim menu As ContextMenuStrip = myVysualAlarmNotifyIcon.ContextMenuStrip
        menu.Items.Clear()

        Dim hasAlarm As Boolean = result.AlertCount > 0 OrElse result.Message.StartsWith("Problème", StringComparison.OrdinalIgnoreCase)
        Dim statusItem As New ToolStripMenuItem(result.Message) With {.ForeColor = If(hasAlarm, Color.Red, Color.Green), .Enabled = False}
        menu.Items.Add(statusItem)

        For Each client As MonitoringAlertClient In result.AlertClients
            Dim clientName As String = If(String.IsNullOrWhiteSpace(client.ClientName), client.ClientId, client.ClientName)
            Dim detail As String = If(client.LastActivity.HasValue, $"Serveur indisponible depuis {Math.Max(1, CInt(Math.Floor((DateTime.Now - client.LastActivity.Value).TotalDays)))} jour(s). Dernière transmission : {client.LastActivity:dd.MM.yyyy HH:mm}", "Serveur indisponible : aucune donnée transmise")
            Dim alertItem As New ToolStripMenuItem($"🔴 Client en alarme : {clientName}") With {.ForeColor = Color.Red, .Font = New Font(SystemFonts.MenuFont, FontStyle.Bold), .ToolTipText = detail}
            AddHandler alertItem.Click, Sub()
                                            MessageBox.Show($"Client : {clientName}{Environment.NewLine}{detail}", "Alerte de surveillance", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        End Sub
            menu.Items.Add(alertItem)
        Next

        If activeIds.Count > 0 AndAlso activeIds.Any(Function(id) Not _acknowledgedAlertClientIds.Contains(id)) Then
            Dim acknowledgeItem As New ToolStripMenuItem("Acquitter toutes les nouvelles alarmes") With {.ForeColor = Color.DarkOrange}
            AddHandler acknowledgeItem.Click, AddressOf AcknowledgeAllAlarms
            menu.Items.Add(acknowledgeItem)
        End If

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

        SetBlinking(activeIds.Count > 0 AndAlso activeIds.Any(Function(id) Not _acknowledgedAlertClientIds.Contains(id)))
        If Not _blinkTimer.Enabled Then myVysualAlarmNotifyIcon.Icon = If(hasAlarm, _redNotifyIcon, _greenNotifyIcon)
        myVysualAlarmNotifyIcon.Text = If(hasAlarm, $"myVysualAlarm — {result.AlertCount} alarme(s)", "myVysualAlarm — Aucune alarme active")
    End Sub
    Private Sub AcknowledgeAllAlarms(sender As Object, e As EventArgs)
        _acknowledgedAlertClientIds.UnionWith(_activeAlertClientIds)
        SetBlinking(False)
        myVysualAlarmNotifyIcon.Icon = _redNotifyIcon
    End Sub


    Private Sub SetBlinking(shouldBlink As Boolean)
        If shouldBlink Then
            If Not _blinkTimer.Enabled Then
                _isBlinkVisible = True
                _blinkTimer.Start()
            End If
        Else
            _blinkTimer.Stop()
        End If
    End Sub


    Private Sub blinkTimer_Tick(sender As Object, e As EventArgs)
        _isBlinkVisible = Not _isBlinkVisible
        myVysualAlarmNotifyIcon.Icon = If(_isBlinkVisible, _redNotifyIcon, _greenNotifyIcon)
    End Sub


    Private Function CreateStatusIcon(color As Color) As Icon
        Using bitmap As New Bitmap(32, 32)
            Using graphics As Graphics = Graphics.FromImage(bitmap)
                graphics.Clear(Color.Transparent)
                Using brush As New SolidBrush(color)
                    graphics.FillEllipse(brush, 0, 0, 31, 31)
                End Using
                Using pen As New Pen(Color.White, 2)
                    graphics.DrawEllipse(pen, 1, 1, 29, 29)
                End Using
            End Using

            Dim handle As IntPtr = bitmap.GetHicon()
            Try
                Return CType(Icon.FromHandle(handle).Clone(), Icon)
            Finally
                DestroyIcon(handle)
            End Try
        End Using
    End Function


    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Configuration de l'icône dans la zone de notification
        If myVysualAlarmNotifyIcon.Icon IsNot Nothing Then
            Icon = myVysualAlarmNotifyIcon.Icon
            _defaultNotifyIcon = myVysualAlarmNotifyIcon.Icon
        End If

        _greenNotifyIcon = CreateStatusIcon(Color.FromArgb(34, 139, 34))
        _redNotifyIcon = CreateStatusIcon(Color.FromArgb(220, 53, 69))
        AddHandler _blinkTimer.Tick, AddressOf blinkTimer_Tick
        myVysualAlarmNotifyIcon.Icon = _greenNotifyIcon

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
