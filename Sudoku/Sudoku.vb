Public Class Sudoku
    Public Cells As New Dictionary(Of String, Cell)
    Public Rows As New List(Of Row)
    Public Columns As New List(Of Column)
    Public Boxes As New List(Of Box)
    Public Type As String


    Public Sub Build(Data As String)
        Dim rowdata = Data.Split(vbCrLf)
        Dim rid As Integer = 0
        Dim cid As Integer = 0
        For Each rs In rowdata
            rid = rid + 1
            If rid <= 9 Then
                Dim celldata = rs.Split(vbTab)
                Dim row = New Row
                row.Id = rid.ToString("0")
                cid = 0
                For Each cs In celldata
                    cid = cid + 1
                    Dim cell = New Cell
                    cell.Fixed = Mid(cs & " ", 1, 1)
                    cell.Id = row.Id & "," & cid.ToString("0")
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
            Dim column = New Column
            column.Id = cid.ToString("0")
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
                Dim box = New Box
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
                Dim row = New Row
                row.Id = rid.ToString("0")
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
                Rows.Add(row)
            Next

            'build columns
            For j = ioff To ioff + 8
                cid = cid + 1
                Dim column = New Column
                column.Id = cid.ToString("0")
                For k = joff To joff + 8
                    Dim id = (j + 1).ToString & "," & (k + 1).ToString
                    Dim cell = Cells(id)
                    column.Cells.Add(cell)
                    cell.Columns.Add(column)
                Next
                Columns.Add(column)
            Next

            'build boxes
            For j = 0 To 2
                For k = 0 To 2
                    Dim box = New Box
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
                    Boxes.Add(box)
                Next
            Next
        Next

        Type = "Samurai"

    End Sub

    Public Function Solve(Deep As Boolean) As Integer
        For Each cell In Cells.Values
            cell.Options = New List(Of String)
            If cell.Value = " " Then
                For i = 1 To 9
                    Dim s = i.ToString("0")
                    Dim found = False
                    For Each row In cell.Rows
                        For Each c In row.Cells
                            If s = c.Value Then
                                found = True
                            End If
                        Next
                    Next
                    For Each col In cell.Columns
                        For Each c In col.Cells
                            If s = c.Value Then
                                found = True
                            End If
                        Next
                    Next
                    For Each box In cell.Boxes
                        For Each c In box.Cells
                            If s = c.Value Then
                                found = True
                            End If
                        Next
                    Next
                    If Not found Then
                        cell.Options.Add(s)
                    End If
                Next
            End If
            If cell.Options.Count = 1 Then
                cell.Deduced = cell.Options(0)
            End If
        Next

        'PrintSamurai()

        'check unique option in row
        If True Then
            For Each row In Rows
                For i = 1 To 9
                    Dim c = 0
                    Dim cc As New Cell
                    Dim ii = i.ToString("0")
                    For Each cell In row.Cells
                        For Each o In cell.Options
                            If ii = o Then
                                cc = cell
                                c = c + 1
                            End If
                        Next
                    Next
                    If c = 1 Then
                        cc.Deduced = ii
                    End If
                Next
            Next
        End If

        'check unique option in column
        If True Then
            For Each col In Columns
                For i = 1 To 9
                    Dim c = 0
                    Dim cc As New Cell
                    Dim ii = i.ToString("0")
                    For Each cell In col.Cells
                        For Each o In cell.Options
                            If ii = o Then
                                cc = cell
                                c = c + 1
                            End If
                        Next
                    Next
                    If c = 1 Then
                        cc.Deduced = ii
                    End If
                Next
            Next
        End If

        'check unique option in box
        If True Then
            For Each box In Columns
                For i = 1 To 9
                    Dim c = 0
                    Dim cc As New Cell
                    Dim ii = i.ToString("0")
                    For Each cell In box.Cells
                        For Each o In cell.Options
                            If ii = o Then
                                cc = cell
                                c = c + 1
                            End If
                        Next
                    Next
                    If c = 1 Then
                        cc.Deduced = ii
                    End If
                Next
            Next
        End If

        'check rows for identical pairs
        If Deep Then
            For Each row In Rows
                For Each c1 In row.Cells
                    If c1.Options.Count = 2 Then
                        For Each c2 In row.Cells
                            If c2.Options.Count = 2 Then
                                If c1.Id <> c2.Id Then
                                    If c1.OptionList = c2.OptionList Then
                                        Dim s1 = c1.Options(0)
                                        Dim s2 = c1.Options(1)
                                        For Each c3 In row.Cells
                                            If c3.Value = " " Then
                                                If c3.Id <> c1.Id And c3.Id <> c2.Id Then
                                                    If c3.Options.Contains(s1) Then
                                                        c3.Options.Remove(s1)
                                                    End If
                                                    If c3.Options.Contains(s2) Then
                                                        c3.Options.Remove(s2)
                                                    End If
                                                    If c3.Options.Count = 1 Then
                                                        c3.Deduced = c3.Options(0)
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
        End If

        'check columns for identical pairs
        If True Then
            For Each col In Columns
                For Each c1 In col.Cells
                    If c1.Options.Count = 2 Then
                        For Each c2 In col.Cells
                            If c2.Options.Count = 2 Then
                                If c1.Id <> c2.Id Then
                                    If c1.OptionList = c2.OptionList Then
                                        Dim s1 = c1.Options(0)
                                        Dim s2 = c1.Options(1)
                                        For Each c3 In col.Cells
                                            If c3.Value = " " Then
                                                If c3.Id <> c1.Id And c3.Id <> c2.Id Then
                                                    If c3.Options.Contains(s1) Then
                                                        c3.Options.Remove(s1)
                                                    End If
                                                    If c3.Options.Contains(s2) Then
                                                        c3.Options.Remove(s2)
                                                    End If
                                                    If c3.Options.Count = 1 Then
                                                        c3.Deduced = c3.Options(0)
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
        End If

        'check boxes for identical pairs
        If True Then
            For Each box In Boxes
                For Each c1 In box.Cells
                    If c1.Options.Count = 2 Then
                        For Each c2 In box.Cells
                            If c2.Options.Count = 2 Then
                                If c1.Id <> c2.Id Then
                                    If c1.OptionList = c2.OptionList Then
                                        Dim s1 = c1.Options(0)
                                        Dim s2 = c1.Options(1)
                                        For Each c3 In box.Cells
                                            If c3.Value = " " Then
                                                If c3.Id <> c1.Id And c3.Id <> c2.Id Then
                                                    If c3.Options.Contains(s1) Then
                                                        c3.Options.Remove(s1)
                                                    End If
                                                    If c3.Options.Contains(s2) Then
                                                        c3.Options.Remove(s2)
                                                    End If
                                                    If c3.Options.Count = 1 Then
                                                        c3.Deduced = c3.Options(0)
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
        End If


        Dim unsolved As Integer
        For Each cell In Cells.Values
            If cell.Value = " " Then
                unsolved = unsolved + 1
            End If
        Next
        Return unsolved
    End Function

    Public Sub Print()
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
End Class

Public Class Cell
    Public Fixed As String
    Public Deduced As String = ""
    Public Options As New List(Of String)
    Public Id As String

    Public Rows As New List(Of Row)
    Public Columns As New List(Of Column)
    Public Boxes As New List(Of Box)

    Public Function Value() As String
        If Fixed <> " " Then
            Return Fixed
        ElseIf Deduced <> "" Then
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

    'Public Function Id() As String
    '    Return Row.Id & "," & Column.Id
    'End Function
End Class

Public Class Row
    Public Id As String
    Public Cells As New List(Of Cell)

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
                End If
            End If
            s = s & v
        Next
        Return s
    End Function
End Class

Public Class Column
    Public Id As String
    Public Cells As New List(Of Cell)

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
                End If
            End If
            s = s & v
        Next
        Return s
    End Function
End Class

Public Class Box
    Public Id As String
    Public Cells As New List(Of Cell)

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
                End If
            End If
            s = s & v
        Next
        Return s
    End Function
End Class
