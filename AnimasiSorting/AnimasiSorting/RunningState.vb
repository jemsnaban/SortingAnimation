Imports System.Threading
Public Class RunningState
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
        MsgBox("Already running!")
    End Sub

    Public Sub PauseAnimation() Implements State.Istate.PauseAnimation
        _control.setState(_control.getPausedState)
    End Sub

    Public Sub StartAnimation() Implements State.Istate.StartAnimation
        MsgBox("Already running!")
    End Sub

    Public Sub StopAnimation() Implements State.Istate.StopAnimation
        _control.setState(_control.getStoppedState)
    End Sub

    Public Overrides Function toString() As String Implements State.Istate.toString
        Return "Running"
    End Function
End Class
