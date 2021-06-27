Public Class Sudoku
    Public Cells As New Dictionary(Of String, Cell)
    Public Rows As New List(Of Cluster)
    Public Columns As New List(Of Cluster)
    Public Boxes As New List(Of Cluster)
    Public Type As String

    Public method(6) As Integer

    Public swBuildOptions As New Stopwatch
    Public swSingleOption As New Stopwatch
    Public swSingleOccurence As New Stopwatch
    Public swSimplePairs As New Stopwatch
    Public swSuperSolve1 As New Stopwatch
    Public swSuperSolve2 As New Stopwatch

    Public Function Clusters() As List(Of Cluster)
        Dim l = New List(Of Cluster)
        l.AddRange(Rows)
        l.AddRange(Columns)
        l.AddRange(Boxes)
        Return l
    End Function

    Public Sub Build(Data As String)
        Dim rowdata = Data.Split(vbCrLf)
        Dim rid As Integer = 0
        Dim cid As Integer = 0
        For Each rs In rowdata
            rid = rid + 1
            If rid <= 9 Then
                Dim celldata = rs.Split(vbTab)
                Dim row = New Cluster
                row.Id = "R" & rid.ToString("00")
                row.Type = "row"
                cid = 0
                For Each cs In celldata
                    cid = cid + 1
                    Dim cell = New Cell
                    cell.Fixed = Mid(cs & " ", 1, 1)
                    cell.Id = cid.ToString("0") & "," & rid.ToString("0")
                    Cells.Add(cell.Id, cell)
                    row.Cells.Add(cell)
                    cell.Rows.Add(row)
                Next
                Rows.Add(row)
            End If
        Next

        'build columns
        cid = 0
        For i = 0 To 8
            cid = cid + 1
            Dim column = New Cluster
            column.Id = "C" & cid.ToString("00")
            column.Type = "column"
            For j = 0 To 8
                Dim cell = Rows(j).Cells(i)
                column.Cells.Add(cell)
                cell.Columns.Add(column)
            Next
            Columns.Add(column)
        Next

        'build boxes
        For i = 0 To 2
            For j = 0 To 2
                Dim box = New Cluster
                box.Type = "box"
                box.Id = "B" & (i * 3 + j).ToString("0")
                For k = 0 To 2
                    For m = 0 To 2
                        Dim cell = Rows(i * 3 + k).Cells(j * 3 + m)
                        box.Cells.Add(cell)
                        cell.Boxes.add(box)
                    Next
                Next
                Boxes.Add(Box)
            Next
        Next
        Type = "Simple"
    End Sub

    Public Sub BuildSamurai(Data As String)
        Dim rowdata = Data.Split(vbCrLf)
        Dim rid As Integer = 0
        Dim cid As Integer = 0
        Dim bid As Integer = 0

        For i = 1 To 5
            Dim ioff = 0
            Dim joff = 0
            If i = 3 Then
                ioff = 6
            ElseIf i = 2 Or i = 5 Then
                ioff = 12
            End If
            If i = 3 Then
                joff = 6
            ElseIf i = 4 Or i = 5 Then
                joff = 12
            End If

            For j = joff To joff + 8
                Dim rs = rowdata(j)
                rid = rid + 1
                Dim celldata = rs.Split(vbTab)
                Dim row = New Cluster
                row.Id = "R" & rid.ToString("00")
                cid = 0

                For k = ioff To ioff + 8
                    Dim cs = celldata(k)
                    cid = cid + 1
                    Dim id = (k + 1).ToString & "," & (j + 1).ToString
                    Dim cell As Cell
                    If Cells.ContainsKey(id) Then
                        cell = Cells(id)
                    Else
                        cell = New Cell
                        cell.Id = id
                        Cells.Add(cell.Id, cell)
                    End If
                    cell.Fixed = Mid(cs & " ", 1, 1)
                    row.Cells.Add(cell)
                    cell.Rows.Add(row)
                Next
                row.Type = "row"
                Rows.Add(row)
            Next

            'build columns
            For j = ioff To ioff + 8
                cid = cid + 1
                Dim column = New Cluster
                column.Id = "C" & cid.ToString("00")
                For k = joff To joff + 8
                    Dim id = (j + 1).ToString & "," & (k + 1).ToString
                    Dim cell = Cells(id)
                    column.Cells.Add(cell)
                    cell.Columns.Add(column)
                Next
                column.Type = "column"
                Columns.Add(column)
            Next

            'build boxes
            For j = 0 To 2
                For k = 0 To 2
                    If i <> 3 Or (j = 1 Or k = 1) Then
                        bid = bid + 1
                        Dim box = New Cluster
                        box.Id = "B" & bid.ToString("00")
                        For m = 0 To 2
                            For n = 0 To 2
                                Dim ii = ioff + j * 3 + m
                                Dim jj = joff + k * 3 + n
                                Dim id = (ii + 1).ToString + "," + (jj + 1).ToString
                                Dim cell = Cells(id)
                                box.Cells.Add(cell)
                                cell.Boxes.Add(box)
                            Next
                        Next
                        box.Type = "box"
                        Boxes.Add(box)
                    End If
                Next
            Next
        Next

        Type = "Samurai"
    End Sub

    Public Function Solve(First As Boolean) As Integer
        Dim celllist = Cells.Values

        If First Then
            swBuildOptions.Start()
            BuildOptions()
            swBuildOptions.Stop()
        End If

        swSingleOption.Start()
        SetSingleOptions()
        swSingleOption.Stop()

        If Not First Then
            swSingleOccurence.Start()
            SetSingleOccurrence()
            swSingleOccurence.Stop()
        End If

        If Not First Then
            swSimplePairs.Start()
            SinglePairs()
            swSimplePairs.Stop()
        End If

        Dim unsolved As Integer
        For Each cell In celllist
            If cell.Value = " " Then
                unsolved = unsolved + 1
            End If
        Next
        Return unsolved
    End Function

    Public Sub PrintStopwatches()
        Debug.Print("BuildOptions =" & swBuildOptions.ElapsedMilliseconds.ToString)
        Debug.Print("SingleOption =" & swSingleOption.ElapsedMilliseconds.ToString)
        Debug.Print("SingleOccurance=" & swSingleOccurence.ElapsedMilliseconds.ToString)
        Debug.Print("SimplePairs=" & swSimplePairs.ElapsedMilliseconds.ToString)
        Debug.Print("Supersolve1=" & swSuperSolve1.ElapsedMilliseconds.ToString)
        Debug.Print("Supersolve2=" & swSuperSolve2.ElapsedMilliseconds.ToString)

    End Sub


    Public Sub BuildOptions()
        For Each cell In Cells.Values
            cell.Options = New List(Of Char)
            If cell.Value = " " Then
                For i = 1 To 9
                    Dim s As Char = i.ToString("0")
                    Dim found = False
                    For Each cluster In cell.Clusters
                        For Each c In cluster.Cells
                            If s = c.Value Then
                                found = True
                                Exit For
                            End If
                        Next
                        If found Then Exit For
                    Next
                    If Not found Then
                        cell.Options.Add(s)
                    End If
                Next
            Else
                cell.Options.Add(cell.Value)
            End If
        Next

    End Sub

    Public Sub SetSingleOptions()
        For Each cell In Cells.Values
            If cell.Options.Count = 1 Then
                If cell.Value = " " Then
                    SetCell(cell)
                    method(0) = method(0) + 1
                End If
            End If
        Next

    End Sub

    Public Sub SetSingleOccurrence()
        For Each cluster In Clusters()
            For i = 1 To 9
                Dim ii = i.ToString("0")
                Dim c = 0
                Dim cc As New Cell
                For Each cell In cluster.Cells
                    If cell.Options.Contains(ii) Then
                        cc = cell
                        c = c + 1
                    End If
                Next
                If c = 1 Then
                    If cc.Value = " " Then
                        cc.Options.Clear()
                        cc.Options.Add(ii)
                        SetCell(cc)
                        method(1) = method(1) + 1
                    End If
                End If
            Next
        Next
    End Sub

    Public Sub SinglePairs()
        For Each cluster In Clusters()
            For Each c1 In cluster.Cells
                If c1.Options.Count = 2 Then
                    For Each c2 In cluster.Cells
                        If c1.Id <> c2.Id Then
                            If c2.Options.Count = 2 Then
                                If c1.OptionList = c2.OptionList Then
                                    Dim s1 = c1.Options(0)
                                    Dim s2 = c1.Options(1)
                                    For Each c3 In cluster.Cells
                                        If c3.Value = " " Then
                                            If c3.Id <> c1.Id And c3.Id <> c2.Id Then
                                                If c3.Options.Contains(s1) Then
                                                    c3.Options.Remove(s1)
                                                End If
                                                If c3.Options.Contains(s2) Then
                                                    c3.Options.Remove(s2)
                                                End If
                                                If c3.Options.Count = 1 Then
                                                    If c3.Value = " " Then
                                                        SetCell(c3)
                                                        method(2) = method(2) + 1
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    Next
                End If
            Next
        Next
    End Sub

    Public Function SuperSolve1() As Integer
        'check for digit in multiple cells in a row or column, in a single box, but not elsewhere in box.
        'remove digit from elsewhere in box since must be in row or column

        swSuperSolve1.Start()

        Dim celllist = Cells.Values
        If True Then
            For Each c In Clusters()
                If c.Type <> "box" Then
                    'get 3 boxes
                    Dim boxlist As New List(Of Cluster)
                    For Each cell In c.Cells
                        If Not boxlist.Contains(cell.Box) Then
                            boxlist.Add(cell.Box)
                        End If
                    Next
                    For Each box In boxlist
                        For i = 1 To 9
                            Dim ii = i.ToString("0")
                            Dim incluster = 0
                            Dim outcluster = 0
                            For Each cell In c.Cells
                                If cell.Options.Contains(ii) Then
                                    If cell.Box Is box Then
                                        incluster = incluster + 1
                                    Else
                                        outcluster = outcluster + 1
                                    End If
                                End If
                            Next
                            If incluster > 1 And outcluster = 0 Then
                                Dim includelist As New List(Of Cell)
                                For Each cell In c.Cells
                                    If cell.Box Is box Then
                                        includelist.Add(cell)
                                    End If
                                Next

                                For Each cell In box.Cells
                                    If Not includelist.Contains(cell) Then
                                        If cell.Options.Contains(ii) Then
                                            cell.Options.Remove(ii)
                                            If cell.Options.Count = 1 Then
                                                If cell.Value = " " Then
                                                    SetCell(cell)
                                                    method(3) = method(3) + 1
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                End If
            Next
        End If

        If False Then
            For Each cl In Clusters()
                For i = 1 To 8
                    Dim ii = i.ToString("0")
                    For j = i + 1 To 9
                        Dim jj = j.ToString("0")
                        Dim countwithboth = 0
                        Dim countwitheither = 0
                        For Each cell In cl.Cells
                            If cell.Options.Contains(ii) And cell.Options.Contains(jj) Then
                                countwithboth = countwithboth + 1
                            ElseIf cell.Options.Contains(ii) Or cell.Options.Contains(jj) Then
                                countwitheither = countwitheither + 1
                            End If
                        Next
                        If countwithboth = 2 And countwitheither = 0 Then
                            For Each cell In cl.Cells
                                If cell.Value = " " Then
                                    If cell.Options.Contains(ii) And cell.Options.Contains(jj) Then
                                        If cell.Options.Count > 2 Then
                                            cell.Options = New List(Of Char)
                                            cell.Options.Add(ii)
                                            cell.Options.Add(jj)
                                        End If
                                    Else
                                        If cell.Options.Contains(ii) Then
                                            cell.Options.Remove(ii)
                                        End If
                                        If cell.Options.Contains(jj) Then
                                            cell.Options.Remove(jj)
                                        End If
                                        If cell.Options.Count = 1 Then
                                            If cell.Value = " " Then
                                                SetCell(cell)
                                                method(4) = method(4) + 1
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next
            Next
        End If

        swSuperSolve1.Stop()


        Dim unsolved As Integer
        For Each cell In celllist
            If cell.Value = " " Then
                unsolved = unsolved + 1
            End If
        Next
        Return unsolved
    End Function

    Public Function SuperSolve2() As Integer
        'check for digit in multiple cells in a row or column, in a single box, but not elsewhere in box.
        'remove digit from elsewhere in box since must be in row or column

        swSuperSolve2.Start()

        Dim celllist = Cells.Values
        If False Then
            For Each c In Clusters()
                If c.Type <> "box" Then
                    'get 3 boxes
                    Dim boxlist As New List(Of Cluster)
                    For Each cell In c.Cells
                        If Not boxlist.Contains(cell.Box) Then
                            boxlist.Add(cell.Box)
                        End If
                    Next
                    For Each box In boxlist
                        For i = 1 To 9
                            Dim ii = i.ToString("0")
                            Dim incluster = 0
                            Dim outcluster = 0
                            For Each cell In c.Cells
                                If cell.Options.Contains(ii) Then
                                    If cell.Box Is box Then
                                        incluster = incluster + 1
                                    Else
                                        outcluster = outcluster + 1
                                    End If
                                End If
                            Next
                            If incluster > 1 And outcluster = 0 Then
                                Dim includelist As New List(Of Cell)
                                For Each cell In c.Cells
                                    If cell.Box Is box Then
                                        includelist.Add(cell)
                                    End If
                                Next

                                For Each cell In box.Cells
                                    If Not includelist.Contains(cell) Then
                                        If cell.Options.Contains(ii) Then
                                            cell.Options.Remove(ii)
                                            If cell.Options.Count = 1 Then
                                                If cell.Value = " " Then
                                                    SetCell(cell)
                                                    method(3) = method(3) + 1
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                End If
            Next
        End If

        If True Then
            For Each cl In Clusters()
                For i = 1 To 8
                    Dim ii = i.ToString("0")
                    For j = i + 1 To 9
                        Dim jj = j.ToString("0")
                        Dim countwithboth = 0
                        Dim countwitheither = 0
                        For Each cell In cl.Cells
                            If cell.Options.Contains(ii) And cell.Options.Contains(jj) Then
                                countwithboth = countwithboth + 1
                            ElseIf cell.Options.Contains(ii) Or cell.Options.Contains(jj) Then
                                countwitheither = countwitheither + 1
                            End If
                        Next
                        If countwithboth = 2 And countwitheither = 0 Then
                            For Each cell In cl.Cells
                                If cell.Value = " " Then
                                    If cell.Options.Contains(ii) And cell.Options.Contains(jj) Then
                                        If cell.Options.Count > 2 Then
                                            cell.Options = New List(Of Char)
                                            cell.Options.Add(ii)
                                            cell.Options.Add(jj)
                                        End If
                                    Else
                                        If cell.Options.Contains(ii) Then
                                            cell.Options.Remove(ii)
                                        End If
                                        If cell.Options.Contains(jj) Then
                                            cell.Options.Remove(jj)
                                        End If
                                        If cell.Options.Count = 1 Then
                                            If cell.Value = " " Then
                                                SetCell(cell)
                                                method(4) = method(4) + 1
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next
            Next
        End If

        swSuperSolve2.Stop()


        Dim unsolved As Integer
        For Each cell In celllist
            If cell.Value = " " Then
                unsolved = unsolved + 1
            End If
        Next
        Return unsolved
    End Function


    Public Function Brute() As Integer

        'Print()

        'brute force tries
        Dim celllist = Cells.Values
        Dim count = 0
        'find a pair
        Dim test As New Cell
        Dim unsolved = Solve(False)
        For Each cell In celllist
            If cell.Value = " " Then
                If cell.Options.Count = 2 Then
                    test = cell
                    count = count + 1
                    Save()
                    test.Options.RemoveAt(0)
                    SetCell(test)
                    unsolved = Solve(False)
                    'unsolved = SuperSolve()

                    If unsolved > 0 Then
                        Read()
                        test.Options.RemoveAt(1)
                        SetCell(test)
                        unsolved = Solve(False)
                        'unsolved = SuperSolve()
                        If unsolved > 0 Then
                            Read()
                        End If
                    End If
                    If unsolved = 0 Then Exit For
                End If
            End If
        Next

        Debug.Print("brute " & count.ToString)
        Return unsolved
    End Function

    Public Function OK() As Boolean
        Dim isok = True
        For Each c In Clusters()
            If Not c.OK Then
                isok = False
            End If
        Next
        Return isok
    End Function

    Private Sub SetCell(Cell As Cell)
        'if one option, set, adjust options in clusters and recursively set cells with one option
        If Cell.Options.Count = 1 Then
            If Cell.Value = " " Then
                Cell.Deduced = Cell.Options(0)
                For Each cluster In Cell.Clusters
                    For Each c2 In cluster.Cells
                        If Cell IsNot c2 Then
                            If c2.Options.Contains(Cell.Deduced) Then
                                c2.Options.Remove(Cell.Deduced)
                                If c2.Options.Count = 1 Then
                                    SetCell(c2)
                                    method(0) = method(0) + 1
                                End If
                            End If
                        End If
                    Next
                Next
            End If
        End If
    End Sub

    Public Sub Print()
        If Type = "Simple" Then
            PrintSimple()
        ElseIf Type = "Samurai" Then
            PrintSamurai()
        Else
            Debug.Print("Not valid paste data")
        End If
    End Sub

    Public Sub PrintSimple()
        For Each row In Rows
            Debug.Print(row.ToString)
        Next
        Debug.Print("")
    End Sub

    Public Sub PrintSamurai()
        Dim s(21, 21) As String
        For i = 0 To 20
            For j = 0 To 20
                s(i, j) = " * "
            Next
        Next


        For Each cell In Cells.Values
            Dim sss = cell.Id.Split(",")
            Dim i As Integer = Int(sss(0)) - 1
            Dim j As Integer = Int(sss(1)) - 1
            If cell.Value <> " " Then
                s(i, j) = " " & cell.Value & " "
            ElseIf cell.Options.Count = 2 Then
                s(i, j) = cell.Options(0) & "+" & cell.Options(1)
                'ElseIf cell.Options.Count = 3 Then
                '    s(i, j) = cell.Options(0) & cell.Options(1) & cell.Options(2)
            Else
                s(i, j) = "   "
            End If
        Next

        For j = 0 To 20
            Dim ss = ""
            For i = 0 To 20
                If ss <> "" Then
                    ss = ss & ","
                End If
                ss = ss & s(i, j)
            Next
            Debug.Print(ss)
        Next

        Debug.Print("")
    End Sub

    Public Sub Save()
        For Each cell In Cells.Values
            cell.Save()
        Next
    End Sub

    Public Sub Read()
        For Each cell In Cells.Values
            cell.Read()
        Next
    End Sub

End Class

Public Class Cache
    Public Deduced As String
    Public OptionList As String
End Class

Public Class Cell
    Public Fixed As Char
    Public Deduced As Char = " "
    Public Options As New List(Of Char)
    Public Id As String

    Public Rows As New List(Of Cluster)
    Public Columns As New List(Of Cluster)
    Public Boxes As New List(Of Cluster)

    Private mClusters As List(Of Cluster)
    Private cache As Cache

    Public Function Clusters() As List(Of Cluster)
        If mClusters Is Nothing Then
            mClusters = New List(Of Cluster)
            mClusters.AddRange(Rows)
            mClusters.AddRange(Columns)
            mClusters.AddRange(Boxes)
        End If
        Return mClusters
    End Function

    Public Function Value() As Char
        If Fixed <> " " Then
            Return Fixed
        ElseIf Deduced <> " " Then
            Return Deduced
        Else
            Return " "
        End If
    End Function

    Public Function OptionList() As String
        Dim s = ""
        For Each o In Options
            s = s & o
        Next
        Return s
    End Function

    Public Function Box() As Cluster
        For Each c In Clusters()
            If c.Type = "box" Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    Public Sub Save()
        cache = New Cache
        cache.Deduced = Deduced
        cache.OptionList = OptionList()
    End Sub

    Public Sub Read()
        Deduced = cache.Deduced
        Options = New List(Of Char)
        For Each c In cache.OptionList.ToCharArray
            Options.Add(c)
        Next
    End Sub
End Class

Public Class Cluster
    Public Id As String
    Public Cells As New List(Of Cell)
    Public Type As String

    Public Overrides Function ToString() As String
        Dim s As String = ""
        For Each cell In Cells
            If s <> "" Then
                s = s & ","
            End If
            Dim v = " " & cell.Value & " "
            If v = "   " Then
                If cell.Options.Count = 2 Then
                    v = cell.Options(0) & "+" & cell.Options(1)
                ElseIf cell.Options.Count = 3 Then
                    v = cell.Options(0) & cell.Options(1) & cell.Options(2)
                End If
            End If
            s = s & v
        Next
        Return s
    End Function

    Public Function OK() As Boolean
        Dim isok As Boolean = True
        For i = 1 To 9
            Dim found = False
            For Each cell In Cells
                If cell.Value = i.ToString Then
                    found = True
                End If
            Next
            If Not found Then
                isok = False
            End If
        Next
        Return isok
    End Function
End Class


