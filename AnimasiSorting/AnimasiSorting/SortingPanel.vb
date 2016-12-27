Imports System.Threading
Imports System.Windows.Forms
Public Class SortingPanel
    'Private variable t(ask) --> digunakan untuk menampung task yang digunakan untuk fungsi pause
    Dim t
    'Private variable stopped --> sebagai penanda apakah animasi dihentikan paksa
    Dim stopped As Boolean = False

    'Private void function untuk menjalankanthread fungsi animasi (RunAnimation) sebagai BackgroundProcess
    Private Sub backgroundworker1_dowork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Me.RunAnimation()
    End Sub

    'Private void function yang dipanggil oleh thread fungsi animasi ketika proses animasi selesai
    Private Sub backgroundworker1_runworkercompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Me.setstate(Me.getstoppedstate)
        If stopped Then
            MsgBox("Stopped!")
        Else
            MsgBox("Done!")
        End If
    End Sub

    'Attributes State
    Dim stoppedState As State.Istate 'merepresentasikan State animasi dalam kondisi berhenti
    Dim runningState As State.Istate 'merepresentasikan State animasi sedang berjalan
    Dim pausedState As State.Istate 'merepresentasikan State animasi dalam kondisi pause

    'Getter untuk masing-masing state
    Public Function getStoppedState()
        Return Me.stoppedState
    End Function

    Public Function getRunningState()
        Return Me.runningState
    End Function

    Public Function getPausedState()
        Return Me.pausedState
    End Function
    'End of getter

    'Public setter untuk mengubah state
    Public Sub setState(ByRef state As Object)
        Me._state = state
    End Sub

    'Property state --> untuk mengakses state animasi
    Dim _state As State.Istate
    Public ReadOnly Property State() As String
        Get
            Return _state.toString
        End Get
    End Property

    'Variabel tampungan defaut List
    Dim _defaultItems As New List(Of Integer)
    'Public funtion reset List
    Public Sub Reset()
        Me.setState(getStoppedState)

        Dim limit = 8
        If _defaultItems.Count < limit Then
            limit = Me.Items.Count
        End If

        Items.Clear()
        For i = 0 To limit - 1
            Items.Add(_defaultItems(i))
        Next

        print(limit)
    End Sub

    'Property items --> List yang menyimpan data
    Dim _items As List(Of Integer)
    Public Property Items() As List(Of Integer)
        Get
            Return _items
        End Get
        Set(ByVal value As List(Of Integer))
            _items = value
        End Set
    End Property

    'Property count --> untuk mengakses jumlah data
    Public ReadOnly Property Count() As Integer
        Get
            Return Items.Count
        End Get
    End Property

    'Property algoritma --> untuk memilih algoritma sorting yang akan dianimasikan
    Dim _algorithm As String
    Enum Algorithms As Short
        Bubble = 0
        Insertion = 1
        Selection = 2
    End Enum
    Public Property Algorithm() As Algorithms
        Get
            Return CType(_algorithm, Algorithms)
        End Get
        Set(value As Algorithms)
            _algorithm = value
        End Set
    End Property

    'Property order --> untuk memilih pengurutan sorting (ASC, DESC)
    Dim _order As String
    Enum Orders As Short
        ASC = 0
        DESC = 1
    End Enum
    Public Property Order() As Orders
        Get
            Return CType(_order, Orders)
        End Get
        Set(value As Orders)
            _order = value
        End Set
    End Property

    'Property speed --> untuk mengakses kecepatan animasi
    Dim _speed As String
    Enum Speeds As Short
        Fast = 0
        Medium = 1
        Slow = 2
    End Enum
    Public Property Speed() As Speeds
        Get
            Return CType(_speed, Speeds)
        End Get
        Set(value As Speeds)
            _speed = value
        End Set
    End Property

    'variabel defaultspeed dalam milisecond
    Dim defaultSpeed = 500
    'Private function untuk mendapatkan nilai speed dalam integer
    Private Function getSpeedInMilisecond() As Integer
        Dim multiplier As Integer

        If Me.Speed = Speeds.Fast Then
            multiplier = 1
        ElseIf Me.Speed = Speeds.Medium Then
            multiplier = 2
        ElseIf Me.Speed = Speeds.Slow Then
            multiplier = 3
        End If

        Return defaultSpeed * multiplier
    End Function

    'Constructor --> untuk inisialisasi object
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.stoppedState = New StoppedState(Me)
        Me.runningState = New RunningState(Me)
        Me.pausedState = New PausedState(Me)

        Me.Panel1.BackColor = BackgroundColor
        Me.Panel2.BackColor = BackgroundColor

        Me._items = New List(Of Integer)
        Me._state = Me.stoppedState
    End Sub

    'List untuk menyimpan label
    Dim labels As New List(Of Label)
    'List penampung
    Dim tempLabels As New List(Of Label)
    Public Sub InitializeAnimation()
        Dim x = 12

        Me._defaultItems.Clear()
        For i = 0 To 7
            If i < Me.Count Then
                Me._defaultItems.Add(Items(i))

                Me.tempLabels.Add(New Label)
                tempLabels(i).Visible = True
                tempLabels(i).Size = New Drawing.Size(50, 20)
                tempLabels(i).Location = New Drawing.Point(x, 12)
                tempLabels(i).BackColor = Panel1.BackColor
                tempLabels(i).ForeColor = Panel1.BackColor
                tempLabels(i).TextAlign = Drawing.ContentAlignment.MiddleCenter
                Me.Panel1.Controls.Add(tempLabels(i))

                Me.labels.Add(New Label)
                labels(i).Visible = True
                labels(i).Size = New Drawing.Size(50, 20)
                labels(i).Location = New Drawing.Point(x, 12)
                labels(i).BackColor = LabelColor
                labels(i).ForeColor = FontColor
                labels(i).Text = Items(i).ToString
                labels(i).TextAlign = Drawing.ContentAlignment.MiddleCenter
                Me.Panel2.Controls.Add(labels(i))
            End If

            x += 56
        Next
    End Sub

    'Public void function yang dipanggil untuk menghentikan animasi sementara
    Public Sub PauseAnimation()
        _state.PauseAnimation()
    End Sub

    'Private void function yang dipanggil ketika State = PausedState
    Private Sub pause()
        t = Tasks.Task.Run(Async Function()
                               Await Tasks.Task.Delay(TimeSpan.FromSeconds(1))
                               Return 42
                           End Function)
        t.Wait()
    End Sub

    'Public void function yang dipanggil untuk melanjutkan animasi
    Public Sub ContinueAnimation()
        _state.ContinueAnimation()
    End Sub

    'Public void function yang dipanggil untuk menghentikan animasi
    Public Sub StopAnimation()
        _state.StopAnimation()
    End Sub

    'Public void function yang dipanggil untuk memulai animasi
    Public Sub StartAnimation()
        _state.StartAnimation()
    End Sub

    'Private void function yang akan mengeksekusi perintah untuk memulai animasi
    Private Sub RunAnimation()
        If stopped Then
            Reset()
            setState(getRunningState)
        End If
        stopped = False

        Dim limit = 8
        If Me.Count < limit Then
            limit = Me.Count
        End If

        If Me.Algorithm = Algorithms.Insertion Then
            insertionSort(limit)
        ElseIf Me.Algorithm = Algorithms.Selection Then
            selectionSort(limit)
        ElseIf Me.Algorithm = Algorithms.Bubble Then
            bubbleSort(limit)
        End If
    End Sub

    'Fungsi algoritma dan animasi insertion sort
    Private Sub insertionSort(limit As Integer)
        For i = 1 To limit - 1
            Dim idx = i

            For j = i - 1 To 0 Step -1
                If Me.Order = Orders.ASC Then
                    If Items(i) <= Items(j) Then
                        idx = j
                    End If
                ElseIf Me.Order = Orders.DESC Then
                    If Items(i) >= Items(j) Then
                        idx = j
                    End If
                End If
            Next

            labels(i).Text = String.Empty
            labels(i).ForeColor = Panel2.BackColor
            labels(i).BackColor = Panel2.BackColor
            tempLabels(i).Text = Items(i)
            tempLabels(i).ForeColor = FontColor
            tempLabels(i).BackColor = PickedLabelColor
            Thread.Sleep(getSpeedInMilisecond)

            'Start of Codeblock --> Pause
            While Me.State = Me.getPausedState.ToString
                Me.pause()
            End While
            'End of Codeblock --> Pause

            'Start of Codeblock --> Stop
            If Me.State = Me.getStoppedState.ToString Then
                stopped = True
                backgroundworker1_runworkercompleted(Me, New System.EventArgs)
            End If
            'End of Codeblock --> Stop

            For j = i - 1 To 0 Step -1
                If Me.Order = Orders.ASC Then
                    If Items(i) > Items(j) Then
                        Exit For
                    End If
                ElseIf Me.Order = Orders.DESC Then
                    If Items(i) < Items(j) Then
                        Exit For
                    End If
                End If

                labels(j).Text = String.Empty
                labels(j).ForeColor = Panel2.BackColor
                labels(j).BackColor = Panel2.BackColor
                labels(j + 1).Text = Items(j)
                labels(j + 1).ForeColor = FontColor
                labels(j + 1).BackColor = LabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop
            Next

            tempLabels(i).Text = String.Empty
            tempLabels(i).ForeColor = Panel2.BackColor
            tempLabels(i).BackColor = Panel2.BackColor
            labels(idx).Text = Items(i)
            labels(idx).ForeColor = FontColor
            labels(idx).BackColor = LabelColor
            Thread.Sleep(getSpeedInMilisecond)

            Dim temp = Items(i)
            Items.RemoveAt(i)
            Items.Insert(idx, temp)

            'Start of Codeblock --> Pause
            While Me.State = Me.getPausedState.ToString
                Me.pause()
            End While
            'End of Codeblock --> Pause

            'Start of Codeblock --> Stop
            If Me.State = Me.getStoppedState.ToString Then
                stopped = True
                backgroundworker1_runworkercompleted(Me, New System.EventArgs)
            End If
            'End of Codeblock --> Stop
        Next
    End Sub

    'Fungsi algoritma dan animasi selection sort
    Private Sub selectionSort(limit As Integer)
        For i = 0 To limit - 2
            labels(i).BackColor = PivotLabelColor
            labels(i + 1).BackColor = FlaggedLabelColor
            Thread.Sleep(getSpeedInMilisecond)

            'Start of Codeblock --> Pause
            While Me.State = Me.getPausedState.ToString
                Me.pause()
            End While
            'End of Codeblock --> Pause

            'Start of Codeblock --> Stop
            If Me.State = Me.getStoppedState.ToString Then
                stopped = True
                backgroundworker1_runworkercompleted(Me, New System.EventArgs)
            End If
            'End of Codeblock --> Stop

            Dim idx = i

            For j = (i + 1) To limit - 1
                labels(j).BackColor = ComparingLabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop

                labels(j).BackColor = LabelColor
                If Me.Order = Orders.ASC Then
                    If Items(j) <= Items(idx) Then
                        If idx <> i Then
                            labels(idx).BackColor = LabelColor
                        End If
                        labels(j).BackColor = FlaggedLabelColor

                        idx = j
                    End If
                ElseIf Me.Order = Orders.DESC Then
                    If Items(j) >= Items(idx) Then
                        If idx <> i Then
                            labels(idx).BackColor = LabelColor
                        End If
                        labels(j).BackColor = FlaggedLabelColor

                        idx = j
                    End If
                End If
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop
            Next

            labels(idx).Text = String.Empty
            labels(idx).ForeColor = Panel2.BackColor
            labels(idx).BackColor = Panel2.BackColor
            tempLabels(idx).Text = Items(idx)
            tempLabels(idx).ForeColor = FontColor
            tempLabels(idx).BackColor = PickedLabelColor
            Thread.Sleep(getSpeedInMilisecond)

            'Start of Codeblock --> Pause
            While Me.State = Me.getPausedState.ToString
                Me.pause()
            End While
            'End of Codeblock --> Pause

            'Start of Codeblock --> Stop
            If Me.State = Me.getStoppedState.ToString Then
                stopped = True
                backgroundworker1_runworkercompleted(Me, New System.EventArgs)
            End If
            'End of Codeblock --> Stop

            If idx <> i Then
                labels(i).Text = String.Empty
                labels(i).ForeColor = Panel2.BackColor
                labels(i).BackColor = Panel2.BackColor
                labels(idx).Text = Items(i)
                labels(idx).ForeColor = FontColor
                labels(idx).BackColor = LabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop

            End If

            tempLabels(idx).Text = String.Empty
            tempLabels(idx).ForeColor = Panel1.BackColor
            tempLabels(idx).BackColor = Panel1.BackColor
            labels(i).Text = Items(idx)
            labels(i).ForeColor = FixedFontColor
            labels(i).BackColor = FixedLabelColor
            Thread.Sleep(getSpeedInMilisecond)

            Dim temp = Items(i)
            Items(i) = Items(idx)
            Items(idx) = temp

            'Start of Codeblock --> Pause
            While Me.State = Me.getPausedState.ToString
                Me.pause()
            End While
            'End of Codeblock --> Pause

            'Start of Codeblock --> Stop
            If Me.State = Me.getStoppedState.ToString Then
                stopped = True
                backgroundworker1_runworkercompleted(Me, New System.EventArgs)
            End If
            'End of Codeblock --> Stop
        Next

        Me.print(limit) 'Untuk mengembalikan warna label seperti semula
    End Sub

    'Fungsi algoritma dan animasi bubble sort
    Private Sub bubbleSort(limit As Integer)
        For i = 1 To limit
            For j = 1 To limit - i
                labels(j - 1).BackColor = ComparingLabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop

                labels(j).BackColor = ComparingLabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop

                If Me.Order = Orders.ASC Then
                    If Items(j - 1) >= Items(j) Then
                        labels(j).Text = String.Empty
                        labels(j).ForeColor = Panel2.BackColor
                        labels(j).BackColor = Panel2.BackColor
                        tempLabels(j).Text = Items(j)
                        tempLabels(j).ForeColor = FontColor
                        tempLabels(j).BackColor = PickedLabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        labels(j - 1).Text = String.Empty
                        labels(j - 1).ForeColor = Panel2.BackColor
                        labels(j - 1).BackColor = Panel2.BackColor
                        labels(j).Text = Items(j - 1)
                        labels(j).ForeColor = FontColor
                        labels(j).BackColor = LabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        tempLabels(j).Text = String.Empty
                        tempLabels(j).ForeColor = Panel1.BackColor
                        tempLabels(j).BackColor = Panel1.BackColor
                        labels(j - 1).Text = Items(j)
                        labels(j - 1).ForeColor = FontColor
                        labels(j - 1).BackColor = LabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        Dim temp = Items(j)
                        Items(j) = Items(j - 1)
                        Items(j - 1) = temp
                    End If
                ElseIf Me.Order = Orders.DESC Then
                    If Items(j - 1) <= Items(j) Then
                        labels(j).Text = String.Empty
                        labels(j).ForeColor = Panel2.BackColor
                        labels(j).BackColor = Panel2.BackColor
                        tempLabels(j).Text = Items(j)
                        tempLabels(j).ForeColor = FontColor
                        tempLabels(j).BackColor = PickedLabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        labels(j - 1).Text = String.Empty
                        labels(j - 1).ForeColor = Panel2.BackColor
                        labels(j - 1).BackColor = Panel2.BackColor
                        labels(j).Text = Items(j - 1)
                        labels(j).ForeColor = FontColor
                        labels(j).BackColor = LabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        tempLabels(j).Text = String.Empty
                        tempLabels(j).ForeColor = Panel1.BackColor
                        tempLabels(j).BackColor = Panel1.BackColor
                        labels(j - 1).Text = Items(j)
                        labels(j - 1).ForeColor = FontColor
                        labels(j - 1).BackColor = LabelColor
                        Thread.Sleep(getSpeedInMilisecond)

                        'Start of Codeblock --> Pause
                        While Me.State = Me.getPausedState.ToString
                            Me.pause()
                        End While
                        'End of Codeblock --> Pause

                        'Start of Codeblock --> Stop
                        If Me.State = Me.getStoppedState.ToString Then
                            stopped = True
                            backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                        End If
                        'End of Codeblock --> Stop

                        Dim temp = Items(j)
                        Items(j) = Items(j - 1)
                        Items(j - 1) = temp
                    End If
                End If

                labels(j - 1).BackColor = LabelColor
                labels(j).BackColor = LabelColor
                Thread.Sleep(getSpeedInMilisecond)

                'Start of Codeblock --> Pause
                While Me.State = Me.getPausedState.ToString
                    Me.pause()
                End While
                'End of Codeblock --> Pause

                'Start of Codeblock --> Stop
                If Me.State = Me.getStoppedState.ToString Then
                    stopped = True
                    backgroundworker1_runworkercompleted(Me, New System.EventArgs)
                End If
                'End of Codeblock --> Stop
            Next

        Next
    End Sub

    'Private void function untuk mencetak data dan mengembalikan warna label dan tulisan seperti semula
    Private Sub print(limit As Integer)
        For i = 0 To limit - 1
            labels(i).Text = Items(i)
            labels(i).ForeColor = FontColor
            labels(i).BackColor = LabelColor
        Next
    End Sub

    'Public function untuk mengembalikan List data
    'sinonim dengan memanggil Property Items
    Public Function Result() As List(Of Integer)
        Return Items
    End Function

    'Property warna teks global
    Dim _fontColor As Drawing.Color = Drawing.Color.Black
    Public Property FontColor() As Drawing.Color
        Get
            Return _fontColor
        End Get
        Set(value As Drawing.Color)
            _fontColor = value
        End Set
    End Property

    'Property warna background komponen
    Dim _bgColor As Drawing.Color = Control.DefaultBackColor
    Public Property BackgroundColor() As Drawing.Color
        Get
            Return _bgColor
        End Get
        Set(value As Drawing.Color)
            _bgColor = value
            Panel1.BackColor = value
            Panel2.BackColor = value
        End Set
    End Property

    'Property warna label global
    Dim _labelColor As Drawing.Color = Drawing.Color.LightSalmon
    Public Property LabelColor() As Drawing.Color
        Get
            Return _labelColor
        End Get
        Set(value As Drawing.Color)
            _labelColor = value
        End Set
    End Property

    'Property warna label saat melakukan perbandingan
    Dim _comparingLabelColor As Drawing.Color = Drawing.Color.Blue
    Private Property ComparingLabelColor() As Drawing.Color
        Get
            Return _comparingLabelColor
        End Get
        Set(value As Drawing.Color)
            _comparingLabelColor = value
        End Set
    End Property

    'Property warna label saat menandai index tertentu, hanya digunakan pada selection sort
    Dim _flaggedLabelColor As Drawing.Color = Drawing.Color.Red
    Private Property FlaggedLabelColor() As Drawing.Color
        Get
            Return _flaggedLabelColor
        End Get
        Set(value As Drawing.Color)
            _flaggedLabelColor = value
        End Set
    End Property

    'Property warna label saat label dipindah ke kotak atas
    Dim _pickedLabelColor As Drawing.Color = Drawing.Color.White
    Private Property PickedLabelColor() As Drawing.Color
        Get
            Return _pickedLabelColor
        End Get
        Set(value As Drawing.Color)
            _pickedLabelColor = value
        End Set
    End Property

    'Property warna label saat label dalam kondisi fix, hanya digunakan pada selection sort
    Dim _fixedFontColor As Drawing.Color = Drawing.Color.White
    Private Property FixedFontColor() As Drawing.Color
        Get
            Return _fixedFontColor
        End Get
        Set(value As Drawing.Color)
            _fixedFontColor = value
        End Set
    End Property

    'Property warna label saat label dalam kondisi fix, hanya digunakan pada selection sort
    Dim _fixedLabelColor As Drawing.Color = Drawing.Color.Black
    Private Property FixedLabelColor() As Drawing.Color
        Get
            Return _fixedLabelColor
        End Get
        Set(value As Drawing.Color)
            _fixedLabelColor = value
        End Set
    End Property

    'Property warna label yang ditandai sebagai pivot, hanya digunakan pada selection sort
    Dim _pivotLabelColor As Drawing.Color = Drawing.Color.Green
    Private Property PivotLabelColor() As Drawing.Color
        Get
            Return _pivotLabelColor
        End Get
        Set(value As Drawing.Color)
            _pivotLabelColor = value
        End Set
    End Property
End Class