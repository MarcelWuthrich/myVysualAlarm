Public Class AppSettings

    ' ============================================================
    ' Mode de connexion
    ' ============================================================

    Public Property UseSshTunnel As Boolean = False


    ' ============================================================
    ' MariaDB
    ' ============================================================

    Public Property DatabaseServer As String = "127.0.0.1"

    Public Property DatabasePort As Integer = 3306

    Public Property DatabaseName As String = ""

    Public Property DatabaseUser As String = ""

    Public Property DatabasePassword As String = ""


    ' ============================================================
    ' SSH
    ' ============================================================

    Public Property SshServer As String = ""

    Public Property SshPort As Integer = 22

    Public Property SshUser As String = ""

    Public Property SshPrivateKeyFile As String = ""

    Public Property SshPassphrase As String = ""


    ' ============================================================
    ' Paramètres du tunnel
    ' ============================================================

    ' Port local utilisé pour le tunnel.
    ' 0 = choisir automatiquement un port libre.
    Public Property SshLocalPort As Integer = 0

    ' Délai sans activité avant le déclenchement d'une alarme.
    Public Property AlertAfterInactivityDays As Integer = 10

End Class
