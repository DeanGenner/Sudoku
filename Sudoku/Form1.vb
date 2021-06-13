Public Class frmMain
    Public Sudoku As Sudoku

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub cmdPaste_Click(sender As Object, e As EventArgs) Handles cmdPaste.Click
        Dim s = Clipboard.GetText
        Dim rowdata = s.Split(vbCrLf)
        Sudoku = New Sudoku
        If rowdata.Count = 10 Then
            Sudoku.Type = "Simple"
            Sudoku.Build(s)
            Sudoku.Print()
        ElseIf rowdata.Count = 22 Then
            Sudoku.Type = "Simple"
            Sudoku.BuildSamurai(s)
            Sudoku.PrintSamurai()
        End If


    End Sub

    Private Sub cmdSolve_Click(sender As Object, e As EventArgs) Handles cmdSolve.Click
        Dim sw = New Stopwatch
        sw.Start()

        Dim unsolved As Integer = Sudoku.Cells.Count
        Dim count = 0
        Dim first As Boolean = True
        While unsolved > 0
            If Sudoku.Type = "Simple" Then
                unsolved = Sudoku.Solve(first)
            Else
                unsolved = Sudoku.Solve(first)
            End If
            first = False
            'Debug.Print("unsolved=" & unsolved.ToString)

            count = count + 1
            If count > 10 Then
                Debug.Print("Not Solved")
                Exit While
            End If
        End While

        sw.Stop()
        Debug.Print(sw.ElapsedMilliseconds & "ms")

        If Sudoku.Type = "Simple" Then
            Sudoku.Print()
        Else
            Sudoku.PrintSamurai()
        End If

        For i = 0 To 2
            Debug.Print(Sudoku.method(i))
        Next
        Debug.Print("Solve Count = " & count.ToString)
    End Sub
End Class
