Imports System.Windows.Forms
Imports System.Globalization
Imports System.Runtime.InteropServices

Namespace MisterHook.Models

    ''' <summary>
    ''' Classe criada para o record de Keyboard
    ''' Marcada como Friend para visibilidade somente interna, para uso do padrão de projeto Facade
    ''' </summary>
    Friend Class KeyboardRecorder
        Implements Interfaces.IKeyboardRecorder

#Region "DLL"

        'Realiza o Hook
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal HookProc As KeyboardHookProcedure, ByVal hInstance As IntPtr, ByVal wParam As Integer) As Integer
        End Function

        'Chama próxima Ação
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function CallNextHookEx(ByVal idHook As Integer, ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
        End Function

        'Remove o Hook
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function UnhookWindowsHookEx(ByVal idHook As Integer) As Boolean
        End Function

#End Region

        'Eventos usados para Log
        Public Shared Event KeyDown(ByVal Key As Keys)
        Public Shared Event KeyUp(ByVal Key As Keys)
        Private Event IKeyboardRecorder_KeyDown(Key As Keys) Implements Interfaces.IKeyboardRecorder.KeyDown
        Private Event IKeyboardRecorder_KeyUp(Key As Keys) Implements Interfaces.IKeyboardRecorder.KeyUp
        Private Const HC_ACTION As Integer = 0

        'Constantes de Keyboard
        Private Const WH_KEYBOARD_LL As Integer = 13
        Private Const WM_KEYDOWN = &H100
        Private Const WM_KEYUP = &H101
        Private Const WM_SYSKEYDOWN = &H104
        Private Const WM_SYSKEYUP = &H105

        'Função que vai realizar a gravação de teclas
        Private Delegate Function KeyboardHookProcedure(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

        Private KeyboardHookStructDelegate As KeyboardHookProcedure = New KeyboardHookProcedure(AddressOf HookKey)
        Private HHookID As IntPtr = IntPtr.Zero

        Private _oPlayback As Models.PlaybackActions
        Private _hinstance As IntPtr = IntPtr.Zero

        ''' <summary>
        ''' Construtor da Classe de gravação de Teclado
        ''' </summary>
        ''' <param name="hinstance">É necessário passar o handle do programa que está realizando a gravação, senão não funciona.</param>
        ''' <param name="oPlayback">Se quiser realizar o playback depois, é necessário passar este objeto</param>
        Public Sub New(hinstance As IntPtr, Optional ByRef oPlayback As Models.PlaybackActions = Nothing)

            _oPlayback = oPlayback
            _hinstance = hinstance

        End Sub

        ''' <summary>
        ''' Realiza a gravação de teclas
        ''' </summary>
        Public Sub StartRecording() Implements Interfaces.IKeyboardRecorder.StartRecording

            HHookID = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookStructDelegate, _hinstance, 0)
            If HHookID = IntPtr.Zero Then
                Throw New Exception("Não foi possível realizar o Hook de Teclado")
            End If

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Public Sub StopRecording() Implements Interfaces.IKeyboardRecorder.StopRecording

            Finalize()

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Protected Overrides Sub Finalize()

            If Not HHookID = IntPtr.Zero Then
                UnhookWindowsHookEx(HHookID)
            End If
            MyBase.Finalize()

        End Sub

        ''' <summary>
        ''' Função que grava a tecla
        ''' </summary>
        ''' <param name="nCode">Ação</param>
        ''' <param name="wParam">Código da Tecla</param>
        ''' <param name="lParam">Estrutura de tecla pressionada</param>
        ''' <returns></returns>
        Private Function HookKey(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

            Dim HookReturn As Integer = CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam) 'Faz a chamada antes de tudo, para evitar problemas
            If (nCode = HC_ACTION) Then

                Select Case wParam
                    Case WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, WM_SYSKEYUP 'Somente grava estes tipos

                        Dim KeyboardStruct As Structs.KeyboardHookStruct = CType(Marshal.PtrToStructure(lParam, KeyboardStruct.GetType()), Structs.KeyboardHookStruct) 'Transforma lparam na estrutura KeyboardHookStruct
                        Dim KeyboardHookModel As New Models.KeyboardHook
                        With KeyboardHookModel
                            .Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) 'Salva timestamp para uso no playback
                            .KeyboardStruct = KeyboardStruct
                        End With

                        LogAction(KeyboardHookModel) 'Realiza log da tecla pressionada
                        RaiseEvents(wParam, KeyboardStruct) 'Chama Eventos para log

                End Select
            End If
            Return HookReturn

        End Function

        ''' <summary>
        ''' Chama eventos para log
        ''' </summary>
        ''' <param name="wParam">Tecla Pressionada</param>
        ''' <param name="KeyboardStruct">Estrutura de Tecla para passar pros eventos</param>
        Private Sub RaiseEvents(wParam As IntPtr, KeyboardStruct As Structs.KeyboardHookStruct)

            Dim Key As Keys = CType(KeyboardStruct.vkCode, Keys)

            If wParam = WM_KEYDOWN OrElse wParam = WM_SYSKEYDOWN Then
                RaiseEvent KeyDown(Key)
            ElseIf wParam = WM_KEYUP OrElse wParam = WM_SYSKEYUP Then
                RaiseEvent KeyUp(Key)
            End If

        End Sub

        ''' <summary>
        ''' Realiza Log da ação para o Playback
        ''' </summary>
        ''' <param name="KeyboardHookModel">Classe da ação realizada</param>
        Private Sub LogAction(KeyboardHookModel As Models.KeyboardHook)

            If _oPlayback IsNot Nothing Then
                _oPlayback.LogAction(KeyboardHookModel)
            End If

        End Sub

    End Class

End Namespace