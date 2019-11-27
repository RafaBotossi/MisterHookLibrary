Namespace MisterHook.Models

    ''' <summary>
    ''' Classe criada para encapsular os tipos diferentes de gravação
    ''' Usa o padrão de projeto Facade
    ''' </summary>
    Public Class Recorder
        Implements Interfaces.IRecorder

        'Objetos de Gravação de Mouse e Keyboard com Eventos
        Private WithEvents KeyboardRecorder As Proxies.KeyboardRecorderProxy
        Private WithEvents MouseRecorder As Proxies.MouseRecorderProxy

        Private _oPlayback As Models.PlaybackActions
        Private _hinstance As IntPtr = IntPtr.Zero

        Public Event GetRecordedActions(script As String)

        ''' <summary>
        ''' Construtor da Classe de gravação de Teclado e Mouse
        ''' </summary>
        ''' <param name="hinstance"></param>
        ''' <param name="oPlayback"></param>
        ''' <param name="LogAux"></param>
        Public Sub New(hinstance As IntPtr, Optional ByRef oPlayback As Models.PlaybackActions = Nothing, Optional LogAux As Boolean = False)

            _oPlayback = oPlayback
            _hinstance = hinstance

            _oPlayback.EraseData()
            KeyboardRecorder = New Proxies.KeyboardRecorderProxy(_hinstance, _oPlayback)
            MouseRecorder = New Proxies.MouseRecorderProxy(_hinstance, _oPlayback)

            Log.Log = LogAux

        End Sub

        ''' <summary>
        ''' Realiza a gravação de teclas e ações do Mouse
        ''' </summary>
        Public Sub StartRecording() Implements Interfaces.IRecorder.StartRecording

            KeyboardRecorder.StartRecording()
            MouseRecorder.StartRecording()

        End Sub

        ''' <summary>
        ''' Pára a gravação
        ''' </summary>
        Public Sub StopRecording() Implements Interfaces.IRecorder.StopRecording

            If KeyboardRecorder IsNot Nothing Then
                KeyboardRecorder.StopRecording()
            End If

            If MouseRecorder IsNot Nothing Then
                MouseRecorder.StopRecording()
            End If

            If _oPlayback IsNot Nothing Then
                RaiseEvent GetRecordedActions(_oPlayback.GetRecordedActions())
            End If

        End Sub

    End Class

End Namespace

