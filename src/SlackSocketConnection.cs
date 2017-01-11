using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CharlesBukowskiSlackBot;

namespace ConsoleApplication
{
    class KeepSocketConnectionAlive
    {
        private ClientWebSocket webSocket;

        public KeepSocketConnectionAlive(ClientWebSocket webSocket)
        {
            this.webSocket = webSocket;
        }

        public async void KeepAlive()
        {
            var ping = new
            {
                id = 1234,
                type = "ping",
                timestamp = DateTimeOffset.UtcNow
            };

            var outboundBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ping));
            var outboundBuffer = new ArraySegment<byte>(outboundBytes);


            while (true)
            {
                await webSocket.SendAsync(outboundBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(2000);
            }
        }
    }

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

            new KeepSocketConnectionAlive(websocket).KeepAlive();

            var receiveBytes = new byte[4096];
            var receiveBuffer = new ArraySegment<byte>(receiveBytes);

            //TODO: move the handling out to the main thread
            var handler = new MessageHandler(new GetRandomBukowskiQuote(), new SendSlackMessage(websocket),
                Program.Configuration["slackbot-id"]);

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
                            handler.Handle(message);
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