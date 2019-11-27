Namespace MisterHook.Models

    ''' <summary>
    ''' Modelo criado para também ter o Timestamp de quando houver ação de mouse
    ''' </summary>
    Public Class MouseHook

        Public Code As IntPtr
        Public Timestamp As Date
        Public MouseStruct As Structs.MouseHookStruct

    End Class

End Namespace