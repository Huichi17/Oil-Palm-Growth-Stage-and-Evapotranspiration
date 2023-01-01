Public Class Loading


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        ProgressBar1.Increment(1)
        If ProgressBar1.Value = 100 Then
            ET.Show()
            Me.Hide()
            Timer1.Enabled = False
        End If
    End Sub
End Class