Public Class frmMain
    Public Sudoku As Sudoku

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    'Private Function Loaddata() As String
    '    Dim s = ""
    '    s = s & " ,5, ,2,3,1, , , " & vbCr
    '    s = s & " , ,7, , , , , , " & vbCr
    '    s = s & "3, , ,5, , , , ,4" & vbCr
    '    s = s & " , ,4, ,6,2,7, ,5" & vbCr
    '    s = s & " , ,9, ,1, ,8,2, " & vbCr
    '    s = s & " , ,1,7, ,3, ,4, " & vbCr
    '    s = s & " ,8, , , ,4, ,7, " & vbCr
    '    s = s & "2,4, ,8, , ,3,9, " & vbCr
    '    s = s & " , , ,1, , , ,5, "

    '    Return s
    'End Function

    Private Sub cmdPaste_Click(sender As Object, e As EventArgs) Handles cmdPaste.Click
        Dim s = Clipboard.GetText
        Sudoku = New Sudoku
        'Sudoku.Build(s)
        Sudoku.BuildSamurai(s)
        'Sudoku.Print()
        Sudoku.PrintSamurai()
    End Sub

    Private Sub cmdSolve_Click(sender As Object, e As EventArgs) Handles cmdSolve.Click
        Dim sw = New Stopwatch
        sw.Start()

        Dim unsolved As Integer = Sudoku.Cells.Count
        Dim previous As Integer = unsolved
        Dim count = 0
        Dim deep As Boolean = False
        While unsolved > 0
            If Sudoku.Type = "Simple" Then
                unsolved = Sudoku.Solve(deep)
                'Sudoku.Print()
            Else
                unsolved = Sudoku.Solve(deep)
                'Sudoku.PrintSamurai()
            End If
            If unsolved = previous Then
                'deep = True
            End If
            previous = unsolved
            Debug.Print("unsolved=" & unsolved.ToString)

            count = count + 1
            If count > 20 Then
                Debug.Print("Not Solved")
                Exit While
            End If
        End While

        sw.Stop()
        Debug.Print(sw.ElapsedMilliseconds & "ms")

        Sudoku.PrintSamurai()
    End Sub
End Class
