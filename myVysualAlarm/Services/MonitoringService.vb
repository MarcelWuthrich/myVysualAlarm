Imports System.Threading

Public Class MonitoringService

    Private ReadOnly _timer As System.Threading.Timer

    Public Event StatusChanged(status As String)

    Public Sub New()

        ' Vérification toutes les 10 secondes
        _timer = New System.Threading.Timer(
            AddressOf TimerCallback,
            Nothing,
            Timeout.Infinite,
            Timeout.Infinite
        )

    End Sub

    Public Sub Start()

        ' Première vérification immédiatement
        CheckStatus()

        ' Puis vérification toutes les 10 secondes
        _timer.Change(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10)
        )

    End Sub

    Public Sub [Stop]()

        _timer.Change(
            Timeout.Infinite,
            Timeout.Infinite
        )

        RaiseEvent StatusChanged("🔴 Surveillance inactive")

    End Sub

    Private Sub TimerCallback(state As Object)

        CheckStatus()

    End Sub

    Private Sub CheckStatus()

        Dim isOk As Boolean = GetTestStatus()

        If isOk Then

            RaiseEvent StatusChanged("🟢 Surveillance active - OK")

        Else

            RaiseEvent StatusChanged("🔴 Surveillance active - PAS OK")

        End If

    End Sub

    Private Function GetTestStatus() As Boolean

        ' TEST TEMPORAIRE
        ' Minute paire = OK
        ' Minute impaire = PAS OK

        Dim currentMinute As Integer = DateTime.Now.Minute

        Return (currentMinute Mod 2 = 0)

    End Function

End Class