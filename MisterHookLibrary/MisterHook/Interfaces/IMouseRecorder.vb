Namespace MisterHook.Interfaces

    ''' <summary>
    ''' Interface para Record de Mouse, herda da interface-base IRecorder
    ''' Marcada como Friend para visibilidade somente interna, para uso do padrão de projeto Facade
    ''' Interface criada para uso de padrão de projeto Proxy
    ''' </summary>
    Friend Interface IMouseRecorder
        Inherits IRecorder

        'Eventos usados para Log
        Event MouseLDown()
        Event MouseLUp()
        Event MouseRDown()
        Event MouseRUp()

    End Interface

End Namespace