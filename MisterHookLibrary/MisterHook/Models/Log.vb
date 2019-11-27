Imports System.Configuration.ConfigurationManager

Namespace MisterHook.Models

    ''' <summary>
    ''' Classe para log de eventos
    ''' </summary>
    Friend Class Log

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Propriedade para ativar o log ou não quando houver ação de Log
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property Log() As Boolean

        ''' <summary>
        ''' Ação de Log do evento
        ''' </summary>
        ''' <param name="Type">Tipo de log a ser criado</param>
        ''' <param name="msg">Mensagem para ser logada</param>
        Public Shared Sub LogAction(Type As enLogAction, msg As String)

            Try

                If Log Then

                    Dim file As String = String.Format("{0}\MisterHookLog {1}.txt", My.Computer.FileSystem.SpecialDirectories.Temp, Now.ToString("dd-MM-yyyy"))
                    Dim finalMsg As String = String.Format("{0}: {1} - {2} {3}", Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), Type.ToString, msg, vbCrLf)

                    My.Computer.FileSystem.WriteAllText(file, finalMsg, True, System.Text.Encoding.Default)

                End If

            Catch
            End Try

        End Sub

    End Class

End Namespace
