Imports System.IO
Public Class ET
    Dim filepath As String
    Dim file_counter As Integer
    Private _pT As Double
    Private _pT1 As Double

    Sub PlayVideo()
        AxWindowsMediaPlayer1.URL = filepath
        AxWindowsMediaPlayer1.Ctlcontrols.play()
    End Sub

    Private Sub open_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles open.Click
        Using bukaFile As New OpenFileDialog
            bukaFile.Filter = "Comma Separated Value|*.csv"
            If bukaFile.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
            FileInput.Text = bukaFile.FileName
        End Using
    End Sub
    Private Sub save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles save.Click
        Using simpanFile As New SaveFileDialog
            With simpanFile
                .Filter = "Comma Separated Value|*.csv"
                If .ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
                FileOutput.Text = .FileName
            End With
        End Using
    End Sub
    Private Sub Growth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Growth.Click
        Dim i As Integer = 0
        Dim jumlahData As Integer = 0
        Dim dataCSV() As String
        Dim hari As Double

        'membuka file dan mengambil data
        Using file1 As New IO.StreamReader(FileInput.Text)
            While Not (file1.EndOfStream)
                dataCSV = Split(file1.ReadLine, ",") 'memisahkan data iklim
                i = i + 1 'menambah indeks hari
                'memasukkan data iklim
                RN(i) = Val(dataCSV(0))
                suhu(i) = Val(dataCSV(1))
                RH(i) = Val(dataCSV(2))
                Tmin(i) = Val(dataCSV(3))
                Tmax(i) = Val(dataCSV(4))
                u(i) = Val(dataCSV(5))
            End While
        End Using
        jumlahData = i

        'Crop Growth Model
        TU1 = Val(txtTU1.Text)
        TU2 = Val(txtTU2.Text)
        TU3 = Val(txtTU3.Text)
        TU4 = Val(txtTU4.Text)
        Tb = Val(txtTb.Text)
        'inisialisasi
        TU(0) = 0
        indeks(0) = 0
        For hari = 1 To jumlahData
            TU(hari) = TU(hari - 1) + suhu(hari) - Tb 'ini buat thermal unit
            If (indeks(hari - 1) < 0.032) Then
                'fase 1
                indeks(hari) = (suhu(hari) - Tb) / TU1 * 0.032 + indeks(hari - 1)
                fase(hari) = "fase1"

                'correction
                If (indeks(hari) > 0.032) Then
                    indeks(hari) = 0.032
                    fase1 = hari
                End If
            ElseIf (indeks(hari - 1) < 0.524) Then
                'fase 2
                indeks(hari) = (suhu(hari) - Tb) / TU2 * 0.492 + indeks(hari - 1)
                fase(hari) = "fase2"

                'correction
                If (indeks(hari) > 0.524) Then
                    indeks(hari) = 0.524
                    fase2 = hari
                End If
            ElseIf (indeks(hari - 1) < 0.622) Then
                'fase 3
                indeks(hari) = (suhu(hari) - Tb) / TU3 * 0.098 + indeks(hari - 1)
                fase(hari) = "fase3"

                'correction
                If (indeks(hari) > 0.622) Then
                    indeks(hari) = 0.622
                    fase3 = hari
                End If
            Else
                'fase4
                indeks(hari) = (suhu(hari) - Tb) / TU4 * 0.378 + indeks(hari - 1)
                fase(hari) = "fase4"

                'correction
                If (indeks(hari) > 1) Then
                    indeks(hari) = 1
                    fase4 = hari
                End If
            End If

            'mengeluarkan proses jika indeks >= 1
            If (indeks(hari) >= 1) Then
                GoTo selesai
            End If
        Next

Selesai:
        'menambah data ke tabel
        day_growth = hari

        For j = 1 To day_growth
            Tabel1.Rows.Add(New String() {j, TU(j), fase(j)}) 'menambah baris
        Next



        'MsgBox("Magnificent Work... Well Done", vbInformation, "Message")
    End Sub

    Private Sub RPT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RPT.Click

        Dim hari As Double

        hari = day_growth

        'priestly-taylor
        k = Val(txtk.Text)
        gamma = Val(psikrometer.Text)
        lamda = Val(txtLamda.Text)

        For hari = 1 To day_growth
            delta(hari) = (4098 * (0.6108 * (exp ^ ((17.27 * suhu(hari) / (suhu(hari) + 273.3)) / (suhu(k) + 273.3) ^ 2))))
            PRT(hari) = k * (delta(k) / (delta(hari) + gamma)) * (RN(hari) / lamda)
            PRT_1(hari) = Math.Round(PRT(hari), 3)
        Next


        'Penman-Monteith FAO
        Dim kc1, kc2, kc3 As Double
        Dim Gday(100000), Gnight(100000), G(100000) As Double
        kc1 = Val(txtkc1.Text)
        kc2 = Val(txtkc2.Text)
        kc3 = Val(txtkc3.Text)

        For hari = 1 To day_growth
            es_Tmin(hari) = 0.6108 * exp ^ ((17.27 * Tmin(hari)) / (Tmin(hari) + 237.3))
            es_Tmax(hari) = 0.6108 * exp ^ ((17.27 * Tmax(hari)) / (Tmax(hari) + 237.3))
            es(hari) = (es_Tmin(hari) + es_Tmax(hari)) / 2
            ea(hari) = RH(hari) * es(hari) / 100
            Gday(hari) = 0.1 * RN(hari)
            Gnight(hari) = -(0.5 * RN(hari))
            G(hari) = Gday(hari) + Gnight(hari)

            PM(hari) = ((0.408 * delta(hari) * (RN(hari) - G(hari))) + (gamma * (900 / (suhu(hari) + 273)) * u(hari) * (es(hari) - ea(hari)))) / (delta(hari) + (gamma * (1 + 0.34 * u(hari))))

            If fase(hari) = "fase1" Then
                PM(hari) = kc1 * PM(hari)
            ElseIf fase(hari) = "fase2" Then
                PM(hari) = (kc1 + ((hari - (hari - 1)) / fase2) * (kc2 - kc1)) * PM(hari)
            Else
                PM(hari) = kc3 * PM(hari)
            End If

            PM_1(hari) = Math.Round(PM(hari), 3)
        Next

        'menambah data ke tabel
        For j = 1 To day_growth
            Tabel2.Rows.Add(New String() {j, PRT_1(j), PM_1(j)}) 'menambah baris
        Next

        Chart1.Series(0).Points.Clear()
        Chart1.Series(1).Points.Clear()

        'menambah data ke grafik
        For j = 1 To day_growth
            Chart1.Series(0).Points.AddXY(j, PRT_1(j))
            Chart1.Series(1).Points.AddXY(j, PM_1(j))
        Next

        'Menulis File
        Using file2 As New IO.StreamWriter(FileOutput.Text)
            file2.WriteLine("Hari" + "," + "TU" + "," + "Indeks" + "," + "Suhu" + "," + "ET-PT" + "," + "ET-PM") 'menulis header
            For j = 1 To day_growth
                file2.WriteLine(j & "," & TU(j) & "," & indeks(j) & "," & suhu(j) & "," & PRT_1(j) & "," & PM_1(j)) 'menulis ke dalam file
            Next j
        End Using

        'MsgBox("Magnificent Work... Well Done", vbInformation, "Message")
    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged

    End Sub

    Private Sub txtLamda_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLamda.TextChanged

    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub Chart1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chart1.Click

    End Sub

    Private Sub Label15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnMinimize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMinimize.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub btnMaximize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMaximize.Click
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
        Else
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub btnInput_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInput.Click
        lblSelected1.Visible = True
        lblSelected2.Visible = False
        lblSelected3.Visible = False
        lblSelected4.Visible = False
        lblSelected5.Visible = False
        lblSelected6.Visible = False

        pnlContent1.Visible = True
        pnlContent2.Visible = False
        pnlContent3.Visible = False
        pnlContent4.Visible = False
        pnlContent5.Visible = False
        Demo.Visible = False
    End Sub

    Private Sub btnParameterPerkembangan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnParameterPerkembangan.Click
        lblSelected1.Visible = False
        lblSelected2.Visible = True
        lblSelected3.Visible = False
        lblSelected4.Visible = False
        lblSelected5.Visible = False
        lblSelected6.Visible = False

        pnlContent1.Visible = False
        pnlContent2.Visible = True
        pnlContent3.Visible = False
        pnlContent4.Visible = False
        pnlContent5.Visible = False
        Demo.Visible = False
    End Sub

    Private Sub btnParameterKonstanta_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnParameterKonstanta.Click
        lblSelected1.Visible = False
        lblSelected2.Visible = False
        lblSelected3.Visible = True
        lblSelected4.Visible = False
        lblSelected5.Visible = False
        lblSelected6.Visible = False

        pnlContent1.Visible = False
        pnlContent2.Visible = False
        pnlContent3.Visible = True
        pnlContent4.Visible = False
        pnlContent5.Visible = False
        Demo.Visible = False
    End Sub

    Private Sub btnOutput_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOutput.Click
        lblSelected1.Visible = False
        lblSelected2.Visible = False
        lblSelected3.Visible = False
        lblSelected4.Visible = True
        lblSelected5.Visible = False
        lblSelected6.Visible = False

        pnlContent1.Visible = False
        pnlContent2.Visible = False
        pnlContent3.Visible = False
        pnlContent4.Visible = True
        pnlContent5.Visible = False
        Demo.Visible = False
    End Sub

    Private Sub btnTentangAplikasi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTentangAplikasi.Click
        lblSelected1.Visible = False
        lblSelected2.Visible = False
        lblSelected3.Visible = False
        lblSelected4.Visible = False
        lblSelected5.Visible = True
        lblSelected6.Visible = False

        pnlContent1.Visible = False
        pnlContent2.Visible = False
        pnlContent3.Visible = False
        pnlContent4.Visible = False
        pnlContent5.Visible = True
        Demo.Visible = False
    End Sub

    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        lblSelected1.Visible = False
        lblSelected2.Visible = False
        lblSelected3.Visible = False
        lblSelected4.Visible = False
        lblSelected5.Visible = False
        lblSelected6.Visible = True
        MsgBox("Silahkan klik tombol merah:)", vbInformation, "Message")
    End Sub

    Private Sub Label15_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label15.Click

    End Sub

    Private Sub pnlLeft_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlLeft.Paint

    End Sub

    Private Sub Label16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label16.Click

    End Sub

    Private Sub ET_1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ET_1.Click
        Chart1.Series(0).Points.Clear()
        Chart1.Series(1).Points.Clear()

        'menambah data ke grafik
        For j = 1 To fase1
            Chart1.Series(0).Points.AddXY(j, PRT_1(j))
            Chart1.Series(1).Points.AddXY(j, PM_1(j))
        Next
    End Sub

    Private Sub ET2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ET_2.Click
        Chart1.Series(0).Points.Clear()
        Chart1.Series(1).Points.Clear()

        'menambah data ke grafik
        For j = (fase1 + 1) To fase2
            Chart1.Series(0).Points.AddXY(j, PRT_1(j))
            Chart1.Series(1).Points.AddXY(j, PM_1(j))
        Next
    End Sub

    Private Sub ET_3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ET_3.Click

        Chart1.Series(0).Points.Clear()
        Chart1.Series(1).Points.Clear()

        'menambah data ke grafik
        For j = (fase2 + 1) To fase3
            Chart1.Series(0).Points.AddXY(j, PRT_1(j))
            Chart1.Series(1).Points.AddXY(j, PM_1(j))
        Next
    End Sub

    Private Sub ET_4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ET_4.Click

        Chart1.Series(0).Points.Clear()
        Chart1.Series(1).Points.Clear()

        'menambah data ke grafik
        For j = (fase3 + 1) To fase4
            Chart1.Series(0).Points.AddXY(j, PRT_1(j))
            Chart1.Series(1).Points.AddXY(j, PM_1(j))
        Next
    End Sub
    Private Sub ET_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs)
        Dim close = MessageBox.Show("Yakin Mau Keluar", "Form Closing", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
        If close = DialogResult.Cancel Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ET_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        DateTime.Text = Date.Now.ToString("dd-MM-yyyy hh:mm:ss")
    End Sub


    Private Sub Demo1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Demo1.Click
        pnlContent1.Visible = False
        pnlContent2.Visible = False
        pnlContent3.Visible = False
        pnlContent4.Visible = False
        pnlContent5.Visible = False
        Demo.Visible = True
    End Sub

    Private Sub play_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles play.Click
        AxWindowsMediaPlayer1.uiMode = "full"
        filepath = Path.Combine(Application.StartupPath, "Palmrizer.mp4")
        If (Not File.Exists(filepath)) Then
            File.WriteAllBytes(filepath, My.Resources.Palmrizer)
        End If
        file_counter = 1
        PlayVideo()
    End Sub
End Class

Module Module1
    Public suhu(100000), RN(100000), Tmin(100000), Tmax(100000), u(100000), RH(100000) As Double
    Public TU(100000), TU1, TU2, TU3, TU4, Tb As Double
    Public indeks(100000) As Double
    Public fase1, fase2, fase3, fase4 As Integer
    Public day_growth As Double
    Public fase(100000) As String
    Public k, gamma, lamda, delta(100000) As Double
    Public exp As Double = 2.7183
    Public PRT(100000), PM(100000), PRT_1(100000), PM_1(100000) As Double
    Public es(100000), ea(100000), es_Tmax(100000), es_Tmin(100000) As Double
End Module
