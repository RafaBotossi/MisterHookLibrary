Imports System.Globalization
Imports System.Runtime.InteropServices

Namespace MisterHook.Models

    ''' <summary>
    ''' Classe criada para o record de Mouse
    ''' Marcada como Friend para visibilidade somente interna, para uso do padrão de projeto Facade
    ''' </summary>
    Friend Class MouseRecorder
        Implements Interfaces.IMouseRecorder

#Region "DLL"

        'Realiza o Hook
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal HookProc As MouseHookProc, ByVal hInstance As IntPtr, ByVal wParam As Integer) As Integer
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
        Public Shared Event MouseLDown()
        Public Shared Event MouseLUp()
        Public Shared Event MouseRDown()
        Public Shared Event MouseRUp()
        Private Event IMouseRecorder_MouseLDown() Implements Interfaces.IMouseRecorder.MouseLDown
        Private Event IMouseRecorder_MouseLUp() Implements Interfaces.IMouseRecorder.MouseLUp
        Private Event IMouseRecorder_MouseRDown() Implements Interfaces.IMouseRecorder.MouseRDown
        Private Event IMouseRecorder_MouseRUp() Implements Interfaces.IMouseRecorder.MouseRUp
        Private Const HC_ACTION As Integer = 0

        'Constantes de Mouse
        Private Const WH_MOUSE As Integer = 7
        Private Const WH_MOUSE_LL As Integer = 14
        Private Const WM_MOUSEMOVE As Integer = &H200
        Private Const WM_LBUTTONDOWN As Integer = &H201
        Private Const WM_LBUTTONUP As Integer = &H202
        Private Const WM_LBUTTONDBLCLK As Integer = &H203
        Private Const WM_RBUTTONDOWN As Integer = &H204
        Private Const WM_RBUTTONUP As Integer = &H205
        Private Const WM_RBUTTONDBLCLK As Integer = &H206
        Private Const WM_MBUTTONDOWN As Integer = &H207
        Private Const WM_MBUTTONUP As Integer = &H208
        Private Const WM_MBUTTONDBLCLK As Integer = &H209
        Private Const WM_MOUSEWHEEL As Integer = &H20A
        Private Const WM_MOUSEHWHEEL As Integer = &H20E

        'Função que vai realizar a gravação de mouse
        Private Delegate Function MouseHookProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

        Private MouseHookProcDelegate As MouseHookProc = New MouseHookProc(AddressOf HookMouse)
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
        ''' Realiza a Gravação de Mouse
        ''' </summary>
        Public Sub StartRecording() Implements Interfaces.IMouseRecorder.StartRecording

            HHookID = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcDelegate, _hinstance, 0)
            If HHookID = IntPtr.Zero Then
                Throw New Exception("Não foi possível realizar o Hook de Mouse")
            End If

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Public Sub StopRecording() Implements Interfaces.IMouseRecorder.StopRecording

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
        ''' Função que grava a ação do Mouse
        ''' </summary>
        ''' <param name="nCode">Ação</param>
        ''' <param name="wParam">Código da ação do mouse</param>
        ''' <param name="lParam">Estrutura da ação do mouse</param>
        ''' <returns></returns>
        Private Function HookMouse(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

            Dim HookReturn As Integer = CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam) 'Faz a chamada antes de tudo, para evitar problemas
            If (nCode = HC_ACTION) Then

                Select Case wParam
                    Case WM_MOUSEMOVE, WM_LBUTTONDOWN, WM_LBUTTONUP, WM_RBUTTONDOWN, WM_RBUTTONUP 'Somente grava estes tipos

                        Dim MouseStruct As Structs.MouseHookStruct = CType(Marshal.PtrToStructure(lParam, MouseStruct.GetType()), Structs.MouseHookStruct)  'Transforma lparam na estrutura MouseHookStruct
                        Dim MouseHookModel As New Models.MouseHook
                        With MouseHookModel
                            .Code = wParam 'Salva código, pois não tem essa info no MouseHookStruct
                            .Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) 'Salva timestamp para uso no playback
                            .MouseStruct = MouseStruct
                        End With

                        LogAction(MouseHookModel)  'Realiza log da tecla pressionada
                        RaiseEvents(wParam)  'Chama Eventos para log

                End Select

            End If
            Return HookReturn

        End Function

        ''' <summary>
        ''' Chama eventos para Log
        ''' </summary>
        ''' <param name="wParam">Ação do Mouse</param>
        Private Sub RaiseEvents(wParam As IntPtr)

            If wParam = WM_LBUTTONDOWN Then
                RaiseEvent MouseLDown()
            ElseIf wParam = WM_LBUTTONUP Then
                RaiseEvent MouseLUp()
            ElseIf wParam = WM_RBUTTONDOWN Then
                RaiseEvent MouseRDown()
            ElseIf wParam = WM_RBUTTONUP Then
                RaiseEvent MouseRUp()
            End If

        End Sub

        ''' <summary>
        ''' Realiza Log da ação para o Playback
        ''' </summary>
        ''' <param name="MouseHookModel">Classe da ação realizada</param>
        Private Sub LogAction(MouseHookModel As Models.MouseHook)

            If _oPlayback IsNot Nothing Then
                _oPlayback.LogAction(MouseHookModel)
            End If

        End Sub

    End Class

End Namespace