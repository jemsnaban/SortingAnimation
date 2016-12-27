Imports System.Threading
Public Class StoppedState
    Implements State.Istate

    Dim _control As SortingPanel
    Default Property Control(ByVal sender As SortingPanel) As SortingPanel
        Get
            Return _control
        End Get
        Set(value As SortingPanel)
            _control = value
        End Set
    End Property

    Public Sub New(ByRef sender As SortingPanel)
        Me.Control(sender) = sender
    End Sub

    Public Sub ContinueAnimation() Implements State.Istate.ContinueAnimation
        MsgBox("Already stopped!")
    End Sub

    Public Sub PauseAnimation() Implements State.Istate.PauseAnimation
        MsgBox("Already stopped!")
    End Sub

    Public Sub StartAnimation() Implements State.Istate.StartAnimation
        _control.setState(_control.getRunningState)
        _control.BackgroundWorker1.RunWorkerAsync()
    End Sub

    Public Sub StopAnimation() Implements State.Istate.StopAnimation
        MsgBox("Already stopped!")
    End Sub

    Public Overrides Function toString() As String Implements State.Istate.toString
        Return "Stopped"
    End Function
End Class
