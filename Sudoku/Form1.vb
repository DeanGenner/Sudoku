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
        ElseIf rowdata.Count = 22 Then
            Sudoku.Type = "Samurai"
            Sudoku.BuildSamurai(s)
        End If
        Sudoku.Print()
    End Sub

    Private Sub cmdSolve_Click(sender As Object, e As EventArgs) Handles cmdSolve.Click
        Dim bruteused As Boolean
        Dim sw = New Stopwatch
        sw.Start()

        Dim unsolved As Integer = Sudoku.Cells.Count
        Dim previous = unsolved
        Dim count = 0
        Dim first As Boolean = True
        While unsolved > 0
            unsolved = Sudoku.Solve(first)
            If unsolved = previous And count > 0 Then
                unsolved = Sudoku.SuperSolve1()
            End If
            If unsolved = previous And count > 0 Then
                unsolved = Sudoku.SuperSolve2()
            End If
            first = False
            If unsolved = previous Then
                bruteused = True
                unsolved = Sudoku.Brute()
            End If
            previous = unsolved
            count = count + 1
            If count > 10 Then
                Debug.Print("Not Solved")
                Exit While
            End If
        End While

        sw.Stop()

        Debug.Print(sw.ElapsedMilliseconds & "ms")
        Debug.Print(IIf(Sudoku.OK, "Checked OK", "Error!!!"))

        Sudoku.Print()

        Debug.Print("Single option = " & Sudoku.method(0).ToString)
        Debug.Print("Once in custer = " & Sudoku.method(1).ToString)
        Debug.Print("Simple Pair Elimination = " & Sudoku.method(2).ToString)
        Debug.Print("Deduced Pair Elimination = " & Sudoku.method(3).ToString)
        Debug.Print("??? = " & Sudoku.method(4).ToString)


        Debug.Print("Loop Count = " & count.ToString)

        Sudoku.PrintStopwatches()
    End Sub
End Class
