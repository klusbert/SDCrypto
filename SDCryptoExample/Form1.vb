Public Class Form1
    Dim sdCrypto As New SDCrypto.SDCrypto()
    
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim key As String = TextBox1.Text
        Dim cipherText As String = TextBox2.Text
        
        If (key.Length > 0 Or cipherText.Length > 0) Then
            TextBox3.Text = sdCrypto.DecryptDataAES(cipherText, key)
        Else
            MessageBox.Show("Key or ciphertext cannot be empty.")
        End If
    End Sub
    
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim key As String = TextBox6.Text
        Dim plainText As String = TextBox5.Text
        
        If (key.Length > 0 Or plainText.Length > 0) Then
            TextBox4.Text = sdCrypto.EncryptDataAES(plainText, key)
        End If
    End Sub
End Class
