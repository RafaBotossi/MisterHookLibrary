Imports System.Runtime.InteropServices

Namespace MisterHook.Structs

    ''' <summary>
    ''' Estrutura do hook de mouse
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)> Public Structure MouseHookStruct
        Public pt As MouseHookPointStruct 'Ponto do mouse na tela
        Public hwnd As Integer 'Handle da Janela (É possível pegar o nome da janela usando o hwnd)
        Public wHitTestCode As Integer
        Public dwExtraInfo As Integer
    End Structure

End Namespace