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
        private Uri websocketUri;

        public SlackSocketConnection(Uri websocketUri)
        {
            this.websocketUri = websocketUri;
        }

        public async void Connect()
        {
            var websocket = new System.Net.WebSockets.ClientWebSocket();
            await websocket.ConnectAsync(this.websocketUri, CancellationToken.None);

            var receiveBytes = new byte[4096];
            var receiveBuffer = new ArraySegment<byte>(receiveBytes);
            while (websocket.State == WebSocketState.Open)
            {
                var receivedMessage = await websocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                if (receivedMessage.MessageType == WebSocketMessageType.Close)
                {
                    await
                        websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing websocket",
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
                            new MessageHandler(new GetRandomBukowskiQuote(), new SendSlackMessage(websocket),
                                Program.Configuration["slackbot-id"]).Handle(message);
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