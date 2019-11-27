Namespace MisterHook.Proxies

    ''' <summary>
    ''' Classe criada para o record de ações de Mouse usando o padrão de projetos Proxy, para separar a camada de criação de log
    ''' </summary>
    Friend Class MouseRecorderProxy
        Implements Interfaces.IMouseRecorder

        'Eventos usados para Log
        Public Shared Event MouseLDown()
        Public Shared Event MouseLUp()
        Public Shared Event MouseRDown()
        Public Shared Event MouseRUp()
        Private Event IMouseRecorder_MouseLDown() Implements Interfaces.IMouseRecorder.MouseLDown
        Private Event IMouseRecorder_MouseLUp() Implements Interfaces.IMouseRecorder.MouseLUp
        Private Event IMouseRecorder_MouseRUp() Implements Interfaces.IMouseRecorder.MouseRUp
        Private Event IMouseRecorder_MouseRDown() Implements Interfaces.IMouseRecorder.MouseRDown

        Private WithEvents _MouseRecorder As Models.MouseRecorder

        ''' <summary>
        ''' Construtor da Classe de gravação de Teclado
        ''' </summary>
        ''' <param name="hinstance">É necessário passar o handle do programa que está realizando a gravação, senão não funciona.</param>
        ''' <param name="oPlayback">Se quiser realizar o playback depois, é necessário passar este objeto</param>
        Public Sub New(hinstance As IntPtr, Optional ByRef oPlayback As Models.PlaybackActions = Nothing)

            _MouseRecorder = New Models.MouseRecorder(hinstance, oPlayback)

        End Sub

        ''' <summary>
        ''' Realiza a gravação de teclas
        ''' </summary>
        Public Sub StartRecording() Implements Interfaces.IMouseRecorder.StartRecording

            LogAction(Models.enLogAction.MouseStartRecording, "init")
            _MouseRecorder.StartRecording()
            LogAction(Models.enLogAction.MouseStartRecording, "end")

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Public Sub StopRecording() Implements Interfaces.IMouseRecorder.StopRecording

            LogAction(Models.enLogAction.MouseStopRecording, "init")
            _MouseRecorder.StopRecording()
            LogAction(Models.enLogAction.MouseStopRecording, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de pressionar o botão esquerdo do mouse
        ''' </summary>
        Private Sub _MouseRecorder_MouseLDown() Handles _MouseRecorder.MouseLDown

            LogAction(Models.enLogAction.MouseLeftClickDown, "init")
            RaiseEvent MouseLDown()
            LogAction(Models.enLogAction.MouseLeftClickDown, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de soltar o botão esquerdo do mouse
        ''' </summary>
        Private Sub _MouseRecorder_MouseLUp() Handles _MouseRecorder.MouseLUp

            LogAction(Models.enLogAction.MouseLeftClickUp, "init")
            RaiseEvent MouseLUp()
            LogAction(Models.enLogAction.MouseLeftClickUp, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de pressionar o botão direito do mouse
        ''' </summary>
        Private Sub _MouseRecorder_MouseRDown() Handles _MouseRecorder.MouseRDown

            LogAction(Models.enLogAction.MouseRightClickDown, "init")
            RaiseEvent MouseRDown()
            LogAction(Models.enLogAction.MouseRightClickDown, "end")

        End Sub

        ''' <summary>
        ''' Evento para indicar a ação de soltar o botão direito do mouse
        ''' </summary>
        Private Sub _MouseRecorder_MouseRUp() Handles _MouseRecorder.MouseRUp

            LogAction(Models.enLogAction.MouseRightClickUp, "init")
            RaiseEvent MouseRUp()
            LogAction(Models.enLogAction.MouseRightClickUp, "end")

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