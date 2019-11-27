Namespace MisterHook.Models

    ''' <summary>
    ''' Classe usada para retornar apenas um objeto Playback, utilização do padrão de projeto Singleton
    ''' </summary>
    Public Class PlaybackSingleton

        Private Shared _instance As PlaybackActions

        ''' <summary>
        ''' Construtor vazio pois o uso da função será Shared
        ''' </summary>
        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Retorna objeto Singleton
        ''' </summary>
        ''' <param name="hinstance">Instância do sistema que chamou a library</param>
        ''' <param name="PathToSave">Caminho para salvar o Playback</param>
        ''' <param name="FileName">Nome do arquivo para salvar o Playback</param>
        ''' <returns>Objeto Singleton</returns>
        Public Shared Function Instance(hinstance As IntPtr, Optional PathToSave As String = "", Optional FileName As String = "") As PlaybackActions

            If _instance Is Nothing Then
                _instance = New PlaybackActions(hinstance, PathToSave, FileName)
            End If

            Return _instance

        End Function

    End Class

End Namespace