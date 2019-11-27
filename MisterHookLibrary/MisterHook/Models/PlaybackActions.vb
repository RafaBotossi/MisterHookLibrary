Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms
Imports MisterHookLibrary.MisterHook.Structs

Namespace MisterHook.Models

    ''' <summary>
    ''' Classe usada para realizar playback da gravação realizada
    ''' </summary>
    Public Class PlaybackActions

        Private _StopPlayback As Boolean 'Se é necessário dar Stop no Playback

        Private WithEvents KeyboardRecorder As Proxies.KeyboardRecorderProxy 'Usamos uma gravação de teclas para verificar se a pessoa clicou na tecla Pause para parar a gravação

        Private _HInstance As IntPtr = IntPtr.Zero 'É necessário passar o handle da aplicação que chamou a gravação/playback, se não o hook não funciona
        Private _PathToSave As String, _Filename As String 'Locais para salvar o script se indicado

        ''' <summary>
        ''' Construtor da classe de Playback
        ''' </summary>
        ''' <param name="hinstance">É necessário passar o handle da aplicação que chamou a gravação/playback, se não o hook não funciona</param>
        ''' <param name="PathToSave">Qual o caminho do script para rodar o playback</param>
        ''' <param name="FileName">Qual o nome do arquivo do script para rodar o playback</param>
        Public Sub New(hinstance As IntPtr, Optional PathToSave As String = "", Optional FileName As String = "")

            _HInstance = hinstance
            _PathToSave = PathToSave
            _Filename = FileName

        End Sub

        ''' <summary>
        ''' Realiza o Playback gravado anteriormente
        ''' </summary>
        Public Sub Playback(Optional PathAndFilename As String = "")

            If Not String.IsNullOrEmpty(PathAndFilename) Then
                FillActionStructs(PathAndFilename)
            End If

            _StopPlayback = False

            If ActionStructSingleton.ActionStructs.Count > 0 Then

                'Cria uma gravação de teclado para ver se a pessoa vai apertar a tecla Pause para parar o playback
                KeyboardRecorder = New Proxies.KeyboardRecorderProxy(_HInstance)
                KeyboardRecorder.StartRecording()

                Dim lastTimestamp As Date = "1899-01-01"
                For Each oStruct As Object In ActionStructSingleton.ActionStructs

                    If _StopPlayback Then Exit Sub

                    'Verifica se faz playback de teclado ou mouse
                    Sleep(oStruct, lastTimestamp)
                    If oStruct.GetType() Is GetType(KeyboardHook) Then
                        KeyboardPlayback(oStruct)
                    ElseIf oStruct.GetType() Is GetType(MouseHook) Then
                        MousePlayback(oStruct)
                    End If

                Next

                'Finaliza verificação de teclas do playback
                If KeyboardRecorder IsNot Nothing Then
                    KeyboardRecorder.StopRecording()
                    KeyboardRecorder = Nothing
                End If

            End If

        End Sub

        Private Sub FillActionStructs(PathAndFilename As String)

            If System.IO.File.Exists(PathAndFilename) Then

                Dim reader As IO.StreamReader = My.Computer.FileSystem.OpenTextFileReader(PathAndFilename)

                EraseData()

                Dim json As String = String.Empty
                Do

                    json = reader.ReadLine

                    If json IsNot Nothing Then
                        If json.Contains("MouseStruct") Then
                            Dim MouseHookModel As New MisterHook.Models.MouseHook
                            Dim MS As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
                            Dim ser As New Runtime.Serialization.Json.DataContractJsonSerializer(MouseHookModel.GetType())
                            MouseHookModel = ser.ReadObject(MS)
                            MS.Close()
                            ActionStructSingleton.ActionStructs.Add(MouseHookModel)
                        Else
                            Dim KeyboardHookModel As New MisterHook.Models.KeyboardHook
                            Dim MS As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
                            Dim ser As New Runtime.Serialization.Json.DataContractJsonSerializer(KeyboardHookModel.GetType())
                            KeyboardHookModel = ser.ReadObject(MS)
                            MS.Close()
                            ActionStructSingleton.ActionStructs.Add(KeyboardHookModel)
                        End If
                    End If

                Loop Until json Is Nothing

                reader.Close()

            End If

        End Sub

        ''' <summary>
        ''' Pára o playback
        ''' </summary>
        Public Sub StopPlayback()

            _StopPlayback = True

        End Sub

        ''' <summary>
        ''' Entre as ações é necessário esperar o tempo que o usuário esperou para realizar a ação. Utiliza os timestamps para isso
        ''' </summary>
        ''' <param name="oStruct">Estrutura</param>
        ''' <param name="lastTimestamp">Último Timestamp para cálculo do tempo entre ações</param>
        Private Sub Sleep(oStruct As Object, ByRef lastTimestamp As Date)

            Dim timestamp As Date = oStruct.timestamp
            If lastTimestamp = "1899-01-01" Then
                lastTimestamp = timestamp
            End If
            Dim span As TimeSpan = timestamp - lastTimestamp
            Dim ms As Integer = span.TotalMilliseconds
            lastTimestamp = timestamp
            Threading.Thread.Sleep(ms) 'Espera ms de tempo antes da próxima ação

        End Sub

        ''' <summary>
        ''' Realiza o Log da ação nos objetos de gravação de Teclado e Mouse
        ''' </summary>
        ''' <param name="KeyStruct">Pode ser KeyboardHookModel ou MouseHookModel</param>
        Public Sub LogAction(KeyStruct As Object)

            ActionStructSingleton.ActionStructs.Add(KeyStruct)

        End Sub

        Public Sub EraseData()

            ActionStructSingleton.ActionStructs.Clear()

        End Sub

        ''' <summary>
        ''' Salva o que foi gravado em arquivo
        ''' </summary>
        ''' <param name="PathToSave">Caminho para salvar o arquivo de script</param>
        ''' <param name="Filename">Nome do arquivo de script para salvar</param>
        Public Sub SaveRecordToFile(PathToSave As String, Filename As String)

            If ActionStructSingleton.ActionStructs.Count > 0 Then

                If Not String.IsNullOrEmpty(PathToSave) Then

                    If String.IsNullOrEmpty(Filename) Then
                        Filename = String.Format("MisterHook Recording {0}.txt", Now.ToString("yyyy-MM-dd hhmmss"))
                    End If

                    If System.IO.Directory.Exists(PathToSave) Then

                        Dim PathAndFilename As String = PathToSave & "\" & Filename
                        Dim objWriter As New System.IO.StreamWriter(PathAndFilename)

                        Dim x As Integer = 0
                        For Each oStruct As Object In ActionStructSingleton.ActionStructs

                            x += 1

                            If oStruct.GetType() Is GetType(KeyboardHook) Then

                                Dim KeyboardHookModel As KeyboardHook = oStruct

                                Dim ms As New System.IO.MemoryStream()
                                Dim serializer As New System.Runtime.Serialization.Json.DataContractJsonSerializer(KeyboardHookModel.GetType())
                                serializer.WriteObject(ms, KeyboardHookModel)
                                Dim json() As Byte = ms.ToArray()
                                ms.Close()
                                Dim str As String = System.Text.Encoding.UTF8.GetString(json, 0, json.Length)
                                objWriter.WriteLine(str)

                            ElseIf oStruct.GetType() Is GetType(MouseHook) Then

                                Dim MouseHookModel As MouseHook = oStruct

                                Dim ms As New System.IO.MemoryStream()
                                Dim serializer As New System.Runtime.Serialization.Json.DataContractJsonSerializer(MouseHookModel.GetType())
                                serializer.WriteObject(ms, MouseHookModel)
                                Dim json() As Byte = ms.ToArray()
                                ms.Close()
                                Dim str As String = System.Text.Encoding.UTF8.GetString(json, 0, json.Length)
                                objWriter.WriteLine(str)

                            End If

                        Next

                        objWriter.Close()

                    End If

                End If

            End If

        End Sub

        Public Function GetRecordedActions() As String

            Dim r As New StringBuilder

            If ActionStructSingleton.ActionStructs.Count > 0 Then

                For Each oStruct As Object In ActionStructSingleton.ActionStructs

                    If oStruct.GetType() Is GetType(KeyboardHook) Then

                        Dim KeyboardHookModel As KeyboardHook = oStruct

                        Dim ms As New System.IO.MemoryStream()
                        Dim serializer As New System.Runtime.Serialization.Json.DataContractJsonSerializer(KeyboardHookModel.GetType())
                        serializer.WriteObject(ms, KeyboardHookModel)
                        Dim json() As Byte = ms.ToArray()
                        ms.Close()
                        Dim str As String = System.Text.Encoding.UTF8.GetString(json, 0, json.Length)
                        r.AppendLine(str)

                    ElseIf oStruct.GetType() Is GetType(MouseHook) Then

                        Dim MouseHookModel As MouseHook = oStruct

                        Dim ms As New System.IO.MemoryStream()
                        Dim serializer As New System.Runtime.Serialization.Json.DataContractJsonSerializer(MouseHookModel.GetType())
                        serializer.WriteObject(ms, MouseHookModel)
                        Dim json() As Byte = ms.ToArray()
                        ms.Close()
                        Dim str As String = System.Text.Encoding.UTF8.GetString(json, 0, json.Length)
                        r.AppendLine(str)

                    End If

                Next

            End If

            Return r.ToString()

        End Function

        ''' <summary>
        ''' Checa se o usuário apertou a tecla Pause para parar o Playback
        ''' </summary>
        ''' <param name="Key">Tecla Pressionada</param>
        Private Sub KeyboardRecorder_KeyDown(Key As Keys) Handles KeyboardRecorder.KeyDown

            If Key = Keys.Pause Then
                KeyboardRecorder.StopRecording()
                KeyboardRecorder = Nothing
                StopPlayback()
            End If

        End Sub

#Region "Keyboard"

        Private Const KEYEVENTF_KEYUP = &H2 'Constante de tecla solta

        Private Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Long, ByVal dwExtraInfo As Long) 'Usado para realizar o 'send keys' para o playback

        ''' <summary>
        ''' Realiza o Playback do Teclado
        ''' </summary>
        ''' <param name="oStruct">Estrutura do tipo KeyboardHookModel</param>
        Private Sub KeyboardPlayback(ByRef oStruct As Object)

            Dim KeyboardHookModel As KeyboardHook = oStruct

            'Na ação de keyup tivemos que forçar usando a const KEYEVENTF_KEYUP, pois enviar como a versão abaixo não funcionou
            If KeyboardHookModel.KeyboardStruct.flags = KeyboardHookFlagsStruct.LLKHF_UP Then
                keybd_event(KeyboardHookModel.KeyboardStruct.vkCode, 0, KEYEVENTF_KEYUP, 0)
            Else
                keybd_event(KeyboardHookModel.KeyboardStruct.vkCode, KeyboardHookModel.KeyboardStruct.scanCode, KeyboardHookModel.KeyboardStruct.flags, KeyboardHookModel.KeyboardStruct.dwExtraInfo)
            End If

        End Sub

#End Region

#Region "Mouse"

        'Constantes usadas no playback. Essas de cima são diferentes das realizadas na gravação, por isso a necessidade de um De/Para
        Private Const MOUSEEVENTF_LEFTDOWN As UInt32 = &H2
        Private Const MOUSEEVENTF_LEFTUP As UInt32 = &H4
        Private Const MOUSEEVENTF_RIGHTDOWN As UInt32 = &H8
        Private Const MOUSEEVENTF_RIGHTUP As UInt32 = &H10
        Private Const MOUSEEVENTF_MIDDLEDOWN As UInt32 = &H20
        Private Const MOUSEEVENTF_MIDDLEUP As UInt32 = &H40
        Private Const MOUSEEVENTF_XDOWN As UInt32 = &H80
        Private Const MOUSEEVENTF_XUP As UInt32 = &H100
        Private Const MOUSEEVENTF_WHEEL As UInt32 = &H800
        Private Const MOUSEEVENTF_VIRTUALDESK As UInt32 = &H4000
        Private Const MOUSEEVENTF_ABSOLUTE As UInt32 = &H8000
        Private Const INPUT_MOUSE As UInt32 = 0

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

        Public Declare Auto Function SetCursorPos Lib "User32.dll" (ByVal X As Integer, ByVal Y As Integer) As Long 'Posiciona o Mouse para o playback
        Private Declare Sub mouse_event Lib "user32.dll" (dwFlags As UInteger, dx As UInteger, dy As UInteger, dwData As UInteger, dwExtraInfo As Integer) 'Realiza o Click do Mouse para o playback

        ''' <summary>
        ''' Realiza o Playback do Mouse
        ''' </summary>
        ''' <param name="oStruct">Estrutura do tipo MouseHookModel</param>
        Private Sub MousePlayback(ByRef oStruct As Object)

            Dim MouseHookModel As MouseHook = oStruct

            'Se for movimento de mouse, usa Cursor.Position(), se for click, então usa mouse_event
            If MouseHookModel.Code = WM_MOUSEMOVE Then
                Cursor.Position() = New Point(MouseHookModel.MouseStruct.pt.x, MouseHookModel.MouseStruct.pt.y)
            Else
                mouse_event(GetMouseEventCode(MouseHookModel.Code), 0, 0, 0, 0)
            End If

        End Sub

        'Função De/Para dos códigos de gravação para os de playback
        Private Function GetMouseEventCode(Code As IntPtr)

            Select Case Code
                Case WM_LBUTTONDOWN
                    Return MOUSEEVENTF_LEFTDOWN
                Case WM_LBUTTONUP
                    Return MOUSEEVENTF_LEFTUP
                Case WM_RBUTTONDOWN
                    Return MOUSEEVENTF_RIGHTDOWN
                Case WM_RBUTTONUP
                    Return MOUSEEVENTF_RIGHTUP
                Case Else
                    Return Nothing
            End Select

        End Function

#End Region

    End Class

End Namespace