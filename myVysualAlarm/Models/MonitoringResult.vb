Public Class MonitoringResult

    Public Property IsMonitoringActive As Boolean
    Public Property IsHistoricalDemoMode As Boolean
    Public Property ReferenceDate As DateTime?
    Public Property AlertClients As New List(Of MonitoringAlertClient)()
    Public Property Message As String = ""

    Public ReadOnly Property AlertCount As Integer
        Get
            Return AlertClients.Count
        End Get
    End Property

End Class


Public Class MonitoringAlertClient

    Public Property ClientId As String = ""
    Public Property ClientName As String = ""
    Public Property LastActivity As DateTime?
    Public Property Delay As TimeSpan?

End Class
