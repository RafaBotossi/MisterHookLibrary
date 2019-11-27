Namespace MisterHook.Models

    ''' <summary>
    ''' Classe usada para retornar apenas um objeto ActionStructs, utilizando o padrão de projeto Singleton
    ''' </summary>
    Friend Class ActionStructSingleton

        Private Shared _ActionStructs As List(Of Object)

        ''' <summary>
        ''' Construtor vazio pois o uso da função será Shared
        ''' </summary>
        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Retorna objeto Singleton
        ''' </summary>
        ''' <returns>Objeto Singleton</returns>
        Public Shared Function ActionStructs() As List(Of Object)

            If _ActionStructs Is Nothing Then
                _ActionStructs = New List(Of Object)
            End If

            Return _ActionStructs

        End Function

    End Class

End Namespace
