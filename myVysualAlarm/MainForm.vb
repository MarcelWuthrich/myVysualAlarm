Public Class MainForm

    Private _databaseConnectionManager As DatabaseConnectionManager

    Private _monitoringService As MonitoringService

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
    Private Sub MonitoringStatusChanged(status As String)

        If Me.InvokeRequired Then

            Me.Invoke(
            New Action(
                Sub()
                    MonitoringStatusChanged(status)
                End Sub
            )
        )

            Return

        End If


        If myVysualAlarmNotifyIcon.ContextMenuStrip Is Nothing Then
            Return
        End If


        Dim statusItem As ToolStripMenuItem =
        TryCast(
            myVysualAlarmNotifyIcon.ContextMenuStrip.Items(0),
            ToolStripMenuItem
        )


        If statusItem IsNot Nothing Then

            statusItem.Text = status

            If status.Contains("PAS OK") Then
                statusItem.ForeColor = Color.Red

            ElseIf status.Contains("OK") Then
                statusItem.ForeColor = Color.Green

            Else
                statusItem.ForeColor = SystemColors.MenuText
            End If

        End If
    End Sub
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Configuration de l'icône dans la zone de notification
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

        Dim settingsItem As New ToolStripMenuItem("Paramètres")

        AddHandler settingsItem.Click,
            Sub()
                ShowSettings()
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
        menu.Items.Add(settingsItem)
        menu.Items.Add(exitItem)

        ' Association du menu à l'icône
        myVysualAlarmNotifyIcon.ContextMenuStrip = menu

        ' Création et démarrage du service de surveillance
        _monitoringService = New MonitoringService()

        AddHandler _monitoringService.StatusChanged,
            AddressOf MonitoringStatusChanged

        _monitoringService.Start()

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