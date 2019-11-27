Imports System.Windows.Forms

Namespace MisterHook.Interfaces

    ''' <summary>
    ''' Interface para Record de Keyboard, herda da interface-base IRecorder
    ''' Marcada como Friend para visibilidade somente interna, para uso do padrão de projeto Facade
    ''' Interface criada para uso de padrão de projeto Proxy
    ''' </summary>
    Friend Interface IKeyboardRecorder
        Inherits IRecorder

        'Eventos usados para Log
        Event KeyDown(ByVal Key As Keys)
        Event KeyUp(ByVal Key As Keys)

    End Interface

End Namespace