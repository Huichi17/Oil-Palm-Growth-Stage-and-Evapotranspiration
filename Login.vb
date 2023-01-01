Imports System.Data.Odbc
Public Class Login
    Private Sub log_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles log.Click
        If TextBox1.Text = "" Or TextBox2.Text = "" Then
            MessageBox.Show("Please Input Your Username and Password!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf (TextBox1.Text = My.Settings.Admin And TextBox2.Text = My.Settings.AdminPass) Or (TextBox1.Text = My.Settings.Admin1 And TextBox2.Text = My.Settings.AdminPass1) Or (TextBox1.Text = My.Settings.Admin2 And TextBox2.Text = My.Settings.AdminPass2) Or (TextBox1.Text = My.Settings.Admin3 And TextBox2.Text = My.Settings.AdminPass3) Then
            Call Admin()
        ElseIf (TextBox1.Text <> My.Settings.Admin) Then
            If TextBox1.Text = My.Settings.Admin1 Then
                MessageBox.Show("Password is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Username is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If


        ElseIf TextBox1.Text <> My.Settings.Admin1 Then
            If TextBox1.Text = My.Settings.Admin1 Then
                MessageBox.Show("Password is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Username is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        ElseIf (TextBox1.Text <> My.Settings.Admin2) Then
            If TextBox1.Text = My.Settings.Admin2 Then
                MessageBox.Show("Password is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Username is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        ElseIf TextBox1.Text <> My.Settings.Admin3 Then
            If TextBox1.Text = My.Settings.Admin3 Then
                MessageBox.Show("Password is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Username is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Else
            MessageBox.Show("Password is not CORRECT!", "Oopps", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Admin()
        Loading.Show()
        Me.Hide()
    End Sub

    Private Sub lblPALMRIZER_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Login_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class