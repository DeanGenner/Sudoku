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
        Dim bruteused As Boolean
        Dim sw = New Stopwatch
        sw.Start()

        Dim unsolved As Integer = Sudoku.Cells.Count
        Dim previous = unsolved
        Dim count = 0
        Dim first As Boolean = True
        While unsolved > 0
            unsolved = Sudoku.Solve(first)
            If unsolved > 0 Then
                unsolved = Sudoku.SuperSolve()
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

        If Sudoku.Type = "Simple" Then
            Sudoku.Print()
        Else
            Sudoku.PrintSamurai()
        End If

        For i = 0 To 4
            Debug.Print(Sudoku.method(i))
        Next
        If bruteused Then
            Debug.Print("brute")
        End If
        Debug.Print("Solve Count = " & count.ToString)
    End Sub
End Class
