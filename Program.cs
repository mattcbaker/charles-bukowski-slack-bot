using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CharlesBukowskiSlackBot;

namespace ConsoleApplication
{
    public class Program
    {
        class HelloRTMSession
        {
            public string url { get; set; }
        }

        //TODO: move this somewhere that makes sense
        public class IncomingMessage
        {
            public string type { get; set; }
            public string user { get; set; }
            public string text { get; set; }
            public string channel { get; set; }
        }

        public static void Main(string[] args)
        {
            ConnectToWebsocket().Wait();
            //Console.ReadKey(); //TODO: stop exiting
        }

        static async Task ConnectToWebsocket()
        {
            var websocketUri = new Uri(await GetWebsocketUrl());
            var webSocket = new System.Net.WebSockets.ClientWebSocket();
            await webSocket.ConnectAsync(websocketUri, CancellationToken.None);

            var receiveBytes = new byte[4096];
            var receiveBuffer = new ArraySegment<byte>(receiveBytes);
            var transmitted = string.Empty;
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing websocket", CancellationToken.None);
                }
                else
                {
                    var messageBytes = receiveBuffer.Skip(receiveBuffer.Offset).Take(result.Count).ToArray();

                    var messageRaw = new UTF8Encoding().GetString(messageBytes);
                    var message = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingMessage>(messageRaw);

                    if (message.type == "message")
                    {
                        //var sender = new SendSlackMessage(webSocket);
                        //sender.Execute(message.channel, "hey there friend");                        

                        var handler = new MessageHandler(new GetRandomBukowskiQuote(), new SendSlackMessage(webSocket), "U3K9DE8ES");
                        handler.Handle(message);
                    }

                    Console.WriteLine(messageRaw);
                }
            }
        }

        static async Task<string> GetWebsocketUrl()
        {
            var token = "xoxb-121319484502-7ZkSMJrKt7qS37d6wg4cCjrI"; //TODO: move to app settings
            var startWebsocketUri = "https://slack.com/api/rtm.start";
            var uri = $"{startWebsocketUri}?token={token}";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(uri))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<HelloRTMSession>(responseContent).url;
                }
            }
        }
    }
}
