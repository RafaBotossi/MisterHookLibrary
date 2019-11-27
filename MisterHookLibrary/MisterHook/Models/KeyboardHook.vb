Namespace MisterHook.Models

    ''' <summary>
    ''' Modelo criado para também ter o Timestamp de quando foi pressionada a tecla
    ''' </summary>
    Public Class KeyboardHook

        Public Timestamp As Date 'Timestamp usado para fazer o timing entre as ações
        Public KeyboardStruct As Structs.KeyboardHookStruct 'Estrutura de hook do Teclado

    End Class

End Namespace