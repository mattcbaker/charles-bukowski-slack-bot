using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using CharlesBukowskiSlackBot;

namespace ConsoleApplication
{
    class SlackSocketConnection
    {
        public ClientWebSocket ClientWebSocket { get; set; }
        private Uri websocketUri;
        private Action<IncomingMessage> onMessage;

        public SlackSocketConnection(Uri websocketUri)
        {
            this.websocketUri = websocketUri;
        }

        public void OnMessage(Action<IncomingMessage> handler)
        {
            this.onMessage = handler;
        }

        public async void Connect()
        {
            ClientWebSocket = new System.Net.WebSockets.ClientWebSocket();
            await ClientWebSocket.ConnectAsync(this.websocketUri, CancellationToken.None);

            var receiveBytes = new byte[4096];
            var receiveBuffer = new ArraySegment<byte>(receiveBytes);
            while (ClientWebSocket.State == WebSocketState.Open)
            {
                var receivedMessage = await ClientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                if (receivedMessage.MessageType == WebSocketMessageType.Close)
                {
                    await
                        ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing websocket",
                            CancellationToken.None);
                }
                else
                {
                    var messageBytes = receiveBuffer.Skip(receiveBuffer.Offset).Take(receivedMessage.Count).ToArray();

                    var messageRaw = new UTF8Encoding().GetString(messageBytes);
                    Console.WriteLine(messageRaw);
                    try
                    {
                        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingMessage>(messageRaw);

                        if (message.type == "message")
                        {
                            onMessage(message);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Unable to process message.");
                    }
                }
            }
        }
    }
}