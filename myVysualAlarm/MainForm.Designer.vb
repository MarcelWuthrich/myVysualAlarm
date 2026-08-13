<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        myVysualAlarmNotifyIcon = New NotifyIcon(components)
        SuspendLayout()
        ' 
        ' myVysualAlarmNotifyIcon
        ' 
        myVysualAlarmNotifyIcon.Icon = CType(resources.GetObject("myVysualAlarmNotifyIcon.Icon"), Icon)
        myVysualAlarmNotifyIcon.Text = "myVysualAlarm"
        myVysualAlarmNotifyIcon.Visible = True
        ' 
        ' MainForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Name = "MainForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "myVysualAlarm"
        ResumeLayout(False)
    End Sub

    Friend WithEvents myVysualAlarmNotifyIcon As NotifyIcon

End Class
