Imports System.Windows.Forms

Namespace MisterHook.Proxies

    ''' <summary>
    ''' Classe criada para o record de ações de Keyboard usando o padrão de projetos Proxy, para separar a camada de criação de log
    ''' </summary>
    Friend Class KeyboardRecorderProxy
        Implements Interfaces.IKeyboardRecorder

        'Eventos usados para Log
        Public Shared Event KeyDown(ByVal Key As Keys)
        Public Shared Event KeyUp(ByVal Key As Keys)
        Private Event IKeyboardRecorder_KeyDown(Key As Keys) Implements Interfaces.IKeyboardRecorder.KeyDown
        Private Event IKeyboardRecorder_KeyUp(Key As Keys) Implements Interfaces.IKeyboardRecorder.KeyUp

        Private WithEvents _KeyboardRecorder As Models.KeyboardRecorder

        ''' <summary>
        ''' Construtor da Classe de gravação de Teclado
        ''' </summary>
        ''' <param name="hinstance">É necessário passar o handle do programa que está realizando a gravação, senão não funciona.</param>
        ''' <param name="oPlayback">Se quiser realizar o playback depois, é necessário passar este objeto</param>
        Public Sub New(hinstance As IntPtr, Optional ByRef oPlayback As Models.PlaybackActions = Nothing)

            _KeyboardRecorder = New Models.KeyboardRecorder(hinstance, oPlayback)

        End Sub

        ''' <summary>
        ''' Realiza a gravação de teclas
        ''' </summary>
        Public Sub StartRecording() Implements Interfaces.IKeyboardRecorder.StartRecording

            LogAction(Models.enLogAction.KeyboardStartRecording, "init")
            _KeyboardRecorder.StartRecording()
            LogAction(Models.enLogAction.KeyboardStartRecording, "end")

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Public Sub StopRecording() Implements Interfaces.IKeyboardRecorder.StopRecording

            LogAction(Models.enLogAction.KeyboardStopRecording, "init")
            _KeyboardRecorder.StopRecording()
            LogAction(Models.enLogAction.KeyboardStopRecording, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de pressionar uma tecla
        ''' </summary>
        ''' <param name="Key">Tecla</param>
        Private Sub KeyboardRecorder_KeyDown(Key As Keys) Handles _KeyboardRecorder.KeyDown

            LogAction(Models.enLogAction.KeyboardKeyDown, "init")
            RaiseEvent KeyDown(Key)
            LogAction(Models.enLogAction.KeyboardKeyDown, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de soltar a tecla pressionada
        ''' </summary>
        ''' <param name="Key">Tecla</param>
        Private Sub KeyboardRecorder_KeyUp(Key As Keys) Handles _KeyboardRecorder.KeyUp

            LogAction(Models.enLogAction.KeyboardKeyUp, "init")
            RaiseEvent KeyUp(Key)
            LogAction(Models.enLogAction.KeyboardKeyUp, "end")

        End Sub

        ''' <summary>
        ''' Log da ação
        ''' </summary>
        ''' <param name="logType">Tipo de Log</param>
        ''' <param name="msg">Mensagem</param>
        Private Sub LogAction(logType As Models.enLogAction, msg As String)

            Models.Log.LogAction(logType, msg)

        End Sub

    End Class

End Namespace