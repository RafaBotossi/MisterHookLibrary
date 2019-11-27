Namespace MisterHook.Interfaces

    ''' <summary>
    ''' Interface para Record de Keyboard e Mouse, serve como base para as outras interfaces
    ''' Marcada como Friend para visibilidade somente interna
    ''' </summary>
    Friend Interface IRecorder

        Sub StartRecording()
        Sub StopRecording()

    End Interface

End Namespace