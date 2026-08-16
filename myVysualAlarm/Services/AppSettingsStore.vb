Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.Win32
Imports System.Linq

''' <summary>
''' Enregistre les paramètres de connexion pour l'utilisateur Windows courant.
''' Les mots de passe sont protégés par Windows (DPAPI) avant leur écriture.
''' </summary>
Public NotInheritable Class AppSettingsStore

    Private Const RegistryPath As String = "Software\myVysualAlarm"

    Private Sub New()
    End Sub

    Public Shared Function Load() As AppSettings

        Dim settings As New AppSettings()

        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryPath)
            If key Is Nothing Then Return settings

            settings.UseSshTunnel = ReadBoolean(key, "UseSshTunnel", settings.UseSshTunnel)
            settings.DatabaseServer = ReadString(key, "DatabaseServer", settings.DatabaseServer)
            settings.DatabasePort = ReadInteger(key, "DatabasePort", settings.DatabasePort)
            settings.DatabaseName = ReadString(key, "DatabaseName", settings.DatabaseName)
            settings.DatabaseUser = ReadString(key, "DatabaseUser", settings.DatabaseUser)
            settings.DatabasePassword = Unprotect(ReadString(key, "DatabasePassword", ""))
            settings.SshServer = ReadString(key, "SshServer", settings.SshServer)
            settings.SshPort = ReadInteger(key, "SshPort", settings.SshPort)
            settings.SshUser = ReadString(key, "SshUser", settings.SshUser)
            settings.SshPrivateKeyFile = ReadString(key, "SshPrivateKeyFile", settings.SshPrivateKeyFile)
            settings.SshPassphrase = Unprotect(ReadString(key, "SshPassphrase", ""))
            settings.SshLocalPort = ReadInteger(key, "SshLocalPort", settings.SshLocalPort)
            settings.AlertAfterInactivityMinutes = ReadInteger(key, "AlertAfterInactivityMinutes", settings.AlertAfterInactivityMinutes)
        End Using

        Return settings

    End Function

    Public Shared Sub Save(settings As AppSettings)

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(RegistryPath)
            key.SetValue("UseSshTunnel", settings.UseSshTunnel)
            key.SetValue("DatabaseServer", settings.DatabaseServer)
            key.SetValue("DatabasePort", settings.DatabasePort)
            key.SetValue("DatabaseName", settings.DatabaseName)
            key.SetValue("DatabaseUser", settings.DatabaseUser)
            key.SetValue("DatabasePassword", Protect(settings.DatabasePassword))
            key.SetValue("SshServer", settings.SshServer)
            key.SetValue("SshPort", settings.SshPort)
            key.SetValue("SshUser", settings.SshUser)
            key.SetValue("SshPrivateKeyFile", settings.SshPrivateKeyFile)
            key.SetValue("SshPassphrase", Protect(settings.SshPassphrase))
            key.SetValue("SshLocalPort", settings.SshLocalPort)
            key.SetValue("AlertAfterInactivityMinutes", settings.AlertAfterInactivityMinutes)
        End Using

    End Sub

    Public Shared Function LoadSelectedClientIds(ByRef hasSavedSelection As Boolean) As HashSet(Of String)

        hasSavedSelection = False
        Dim selectedIds As New HashSet(Of String)(StringComparer.Ordinal)

        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryPath)
            If key Is Nothing OrElse key.GetValue("SelectedClientIds") Is Nothing Then
                Return selectedIds
            End If

            hasSavedSelection = True
            Dim value As String = Convert.ToString(key.GetValue("SelectedClientIds"))

            For Each valuePart As String In value.Split(","c, StringSplitOptions.RemoveEmptyEntries)
                selectedIds.Add(valuePart)
            Next
        End Using

        Return selectedIds

    End Function

    Public Shared Sub SaveSelectedClientIds(clientIds As IEnumerable(Of String))

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(RegistryPath)
            key.SetValue("SelectedClientIds", String.Join(",", clientIds.OrderBy(Function(id) id)))
        End Using

    End Sub

    Private Shared Function ReadString(key As RegistryKey, name As String, defaultValue As String) As String
        Return TryCast(key.GetValue(name, defaultValue), String)
    End Function

    Private Shared Function ReadInteger(key As RegistryKey, name As String, defaultValue As Integer) As Integer
        Return Convert.ToInt32(key.GetValue(name, defaultValue))
    End Function

    Private Shared Function ReadBoolean(key As RegistryKey, name As String, defaultValue As Boolean) As Boolean
        Return Convert.ToBoolean(key.GetValue(name, defaultValue))
    End Function

    Private Shared Function Protect(value As String) As String
        If String.IsNullOrEmpty(value) Then Return ""
        Return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(value), Nothing, DataProtectionScope.CurrentUser))
    End Function

    Private Shared Function Unprotect(value As String) As String
        If String.IsNullOrEmpty(value) Then Return ""
        Return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(value), Nothing, DataProtectionScope.CurrentUser))
    End Function

End Class
