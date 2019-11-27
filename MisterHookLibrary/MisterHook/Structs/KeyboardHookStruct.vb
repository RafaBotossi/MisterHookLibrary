Imports System.Runtime.InteropServices

Namespace MisterHook.Structs

    ''' <summary>
    ''' Estrutura do hook de keyboard
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure KeyboardHookStruct
        Public vkCode As UInt32 'Código da Tecla pressionada
        Public scanCode As UInt32
        Public flags As KeyboardHookFlagsStruct 'Flags da tecla pressionada
        Public time As UInt32
        Public dwExtraInfo As UIntPtr
    End Structure

End Namespace