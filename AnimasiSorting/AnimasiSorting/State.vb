Imports System.Threading
Module State
    Interface Istate
        Sub StartAnimation()
        Sub PauseAnimation()
        Sub ContinueAnimation()
        Sub StopAnimation()

        Function toString() As String
    End Interface
End Module
