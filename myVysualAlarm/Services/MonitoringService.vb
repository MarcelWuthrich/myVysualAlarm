Imports System.Threading
Imports MySqlConnector

Public Class MonitoringService

    Private ReadOnly _timer As Timer
    Private ReadOnly _checkLock As New SemaphoreSlim(1, 1)

    Public Event StatusChanged(result As MonitoringResult)

    Public Sub New()
        _timer = New Timer(AddressOf TimerCallback, Nothing, Timeout.Infinite, Timeout.Infinite)
    End Sub

    Public Sub Start()
        Dim firstCheck As Task = CheckStatusAsync()
        _timer.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
    End Sub

    Public Sub [Stop]()
        _timer.Change(Timeout.Infinite, Timeout.Infinite)
        RaiseEvent StatusChanged(New MonitoringResult With {.IsMonitoringActive = False, .Message = "Surveillance inactive"})
    End Sub

    Private Async Sub TimerCallback(state As Object)
        Await CheckStatusAsync()
    End Sub

    Private Async Function CheckStatusAsync() As Task
        If Not Await _checkLock.WaitAsync(0) Then Return

        Try
            RaiseEvent StatusChanged(Await GetMonitoringResultAsync())
        Catch ex As Exception
            RaiseEvent StatusChanged(New MonitoringResult With {.IsMonitoringActive = True, .Message = "Erreur de surveillance : " & ex.Message})
        Finally
            _checkLock.Release()
        End Try
    End Function

    Private Async Function GetMonitoringResultAsync() As Task(Of MonitoringResult)
        Dim hasSelection As Boolean = False
        Dim selectedClientIds As HashSet(Of String) = AppSettingsStore.LoadSelectedClientIds(hasSelection)

        If Not hasSelection OrElse selectedClientIds.Count = 0 Then
            Return New MonitoringResult With {.IsMonitoringActive = True, .Message = "Aucun client sélectionné pour la surveillance."}
        End If

        Dim settings As AppSettings = AppSettingsStore.Load()
        Dim result As New MonitoringResult With {.IsMonitoringActive = True}
        Dim clients As New List(Of MonitoringAlertClient)()

        Using manager As New DatabaseConnectionManager()
            Dim connection As MySqlConnection = Await manager.ConnectAsync(settings)
            Using command As New MySqlCommand()
                command.Connection = connection
                Dim parameters As New List(Of String)()
                Dim index As Integer = 0
                For Each clientId As String In selectedClientIds
                    Dim parameterName As String = "@client" & index
                    parameters.Add(parameterName)
                    command.Parameters.AddWithValue(parameterName, clientId)
                    index += 1
                Next

                command.CommandText = "SELECT e.ety_id, e.ety_name, MAX(m.mon_created_date) AS last_activity " &
                    "FROM gbl_entity e LEFT JOIN sys_monitoring m ON m.ety_id = e.ety_id " &
                    "WHERE e.ety_id IN (" & String.Join(",", parameters) & ") AND e.parent_id IS NOT NULL " &
                    "GROUP BY e.ety_id, e.ety_name"

                Using reader As MySqlDataReader = Await command.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        Dim client As New MonitoringAlertClient With {.ClientId = Convert.ToString(reader("ety_id")), .ClientName = Convert.ToString(reader("ety_name"))}
                        If Not reader.IsDBNull(reader.GetOrdinal("last_activity")) Then client.LastActivity = reader.GetDateTime("last_activity")
                        clients.Add(client)
                    End While
                End Using
            End Using
        End Using

        Dim latestActivity As DateTime? = Nothing
        For Each client As MonitoringAlertClient In clients
            If client.LastActivity.HasValue AndAlso (Not latestActivity.HasValue OrElse client.LastActivity.Value > latestActivity.Value) Then latestActivity = client.LastActivity
        Next

        Dim referenceDate As DateTime = DateTime.Now
        If latestActivity.HasValue AndAlso latestActivity.Value < DateTime.Now.AddHours(-24) Then
            referenceDate = latestActivity.Value
            result.IsHistoricalDemoMode = True
        End If
        result.ReferenceDate = referenceDate

        Dim allowedDelay As TimeSpan = TimeSpan.FromMinutes(Math.Max(1, settings.AlertAfterInactivityMinutes))
        For Each client As MonitoringAlertClient In clients
            If Not client.LastActivity.HasValue Then
                result.AlertClients.Add(client)
            Else
                client.Delay = referenceDate - client.LastActivity.Value
                If client.Delay.Value > allowedDelay Then result.AlertClients.Add(client)
            End If
        Next

        result.Message = If(result.AlertCount = 0,
            If(result.IsHistoricalDemoMode, "Surveillance OK (mode démonstration : données historiques).", "Surveillance OK"),
            $"{result.AlertCount} alarme(s) : activité absente depuis plus de {settings.AlertAfterInactivityMinutes} min.")

        Return result
    End Function

End Class
