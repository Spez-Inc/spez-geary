Public Class Form1
    Private Const WM_NCHITTEST As Integer = 132
    Private Const HTCLIENT As Integer = 1
    Private Const HTCAPTION As Integer = 2
    Private m_aeroEnabled As Boolean
    Private Const CS_DROPSHADOW As Integer = 131072
    Private Const WM_NCPAINT As Integer = 133
    Private Const WM_ACTIVATEAPP As Integer = 28

    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmExtendFrameIntoClientArea(ByVal hWnd As IntPtr, ByRef pMarInset As MARGINS) As Integer
    End Function

    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmSetWindowAttribute(ByVal hwnd As IntPtr, ByVal attr As Integer, ByRef attrValue As Integer, ByVal attrSize As Integer) As Integer
    End Function

    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmIsCompositionEnabled(ByRef pfEnabled As Integer) As Integer
    End Function

    <System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint:="CreateRoundRectRgn")>
    Private Shared Function CreateRoundRectRgn(ByVal nLeftRect As Integer, ByVal nTopRect As Integer, ByVal nRightRect As Integer, ByVal nBottomRect As Integer, ByVal nWidthEllipse As Integer, ByVal nHeightEllipse As Integer) As IntPtr
    End Function

    Public Structure MARGINS

        Public leftWidth As Integer

        Public rightWidth As Integer

        Public topHeight As Integer

        Public bottomHeight As Integer
    End Structure

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            m_aeroEnabled = CheckAeroEnabled()
            Dim cp As CreateParams = MyBase.CreateParams
            If Not m_aeroEnabled Then cp.ClassStyle = cp.ClassStyle Or CS_DROPSHADOW
            Return cp
        End Get
    End Property

    Private Function CheckAeroEnabled() As Boolean
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim enabled As Integer = 0
            DwmIsCompositionEnabled(enabled)
            Return If((enabled = 1), True, False)
        End If

        Return False
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_NCPAINT
                If m_aeroEnabled Then
                    Dim v = 2
                    DwmSetWindowAttribute(Me.Handle, 2, v, 4)
                    Dim margins As MARGINS = New MARGINS() With {.bottomHeight = 1, .leftWidth = 0, .rightWidth = 0, .topHeight = 0}
                    DwmExtendFrameIntoClientArea(Me.Handle, margins)
                End If

            Case Else
        End Select

        MyBase.WndProc(m)
        If m.Msg = WM_NCHITTEST AndAlso CInt(m.Result) = HTCLIENT Then m.Result = CType(HTCAPTION, IntPtr)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        End
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
        TextBox1.Select()
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If TextBox1.Text.Count > 1 Then
            Button2.Enabled = True
        Else
            Button2.Enabled = False
        End If
        If TextBox2.Text.Count > 1 Then
            Button2.Enabled = True
        Else
            Button2.Enabled = False
        End If
        If TextBox3.Text.Count > 1 Then
            Button2.Enabled = True
        Else
            Button2.Enabled = False
        End If
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Login()
    End Sub

    Public Sub Login()
        Timer1.Stop()
        ComboBox1.Enabled = False
        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        Button1.Enabled = False
        Button2.Enabled = False
        Button3.Enabled = False
        Try
            'Ping To: https://google.com's IP Numbers
            Dim internet = My.Computer.Network.Ping("208.67.222.222")
            If internet = True Then
                Label7.ForeColor = Color.Goldenrod
                Label7.Text = "Logging in..."
                WebBrowser1.Document.GetElementById("Email").SetAttribute("value", TextBox2.Text)
                WebBrowser1.Document.GetElementById("signIn").InvokeMember("click")
                If WebBrowser1.Document IsNot Nothing Then
                    Dim element = WebBrowser1.Document.GetElementById("errormsg_0_Email")
                    If element IsNot Nothing Then
                        Label7.ForeColor = Color.FromArgb(254, 0, 0)
                        Label7.Text = "Failed To Login: Email is Not Recognized."
                        ComboBox1.Enabled = True
                        TextBox1.Enabled = True
                        TextBox2.Enabled = True
                        TextBox3.Enabled = True
                        Button1.Enabled = True
                        Button2.Enabled = True
                        Button3.Enabled = True
                    End If
                End If
                Threading.Thread.Sleep(1000)
                WebBrowser1.Document.GetElementById("Passwd").SetAttribute("value", TextBox3.Text)
                Threading.Thread.Sleep(1000)
                WebBrowser1.Document.GetElementById("signIn").InvokeMember("click")
                Threading.Thread.Sleep(1000)
                My.Computer.FileSystem.CreateDirectory(Application.LocalUserAppDataPath + "/accounts/" + TextBox1.Text.ToString)
                My.Computer.FileSystem.WriteAllText(Application.LocalUserAppDataPath + "/accounts/" + TextBox1.Text.ToString + "/accountname.txt", TextBox1.Text, True)
                My.Computer.FileSystem.WriteAllText(Application.LocalUserAppDataPath + "/accounts/" + TextBox1.Text.ToString + "/email.txt", TextBox2.Text, True)
                Threading.Thread.Sleep(1000)
                Application.Restart()
                Login()
            End If
            If internet = False Then
                ComboBox1.Enabled = True
                TextBox1.Enabled = True
                TextBox2.Enabled = True
                TextBox3.Enabled = True
                Button1.Enabled = True
                Button2.Enabled = True
                Button3.Enabled = True
                Label7.Text = "Unable to Login: Network is Not Connected."
                Label7.Visible = True
            End If
        Catch ex As Exception
            ComboBox1.Enabled = True
            TextBox1.Enabled = True
            TextBox2.Enabled = True
            TextBox3.Enabled = True
            Button1.Enabled = True
            Button2.Enabled = True
            Button3.Enabled = True
        End Try
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        Url.Text = WebBrowser1.Url.ToString
        Dim a As String
        Dim b As String
        a = "https://mail.google.com/mail/u/0/h/"
        b = InStr(Url.Text, a)
        If b Then
            Url.Focus()
            Url.SelectionStart = b - 1
            Url.SelectionLength = Len(a)
            Me.Hide()
            Form2.Show()
        Else
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        End
    End Sub
End Class
