using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CharlesBukowskiSlackBot;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }

        class HelloRTMSession
        {
            public string url { get; set; }
        }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            Console.WriteLine(Configuration["test"]);

            ConnectToWebsocket().Wait();
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
                        var handler = new MessageHandler(new GetRandomBukowskiQuote(), new SendSlackMessage(webSocket), Configuration["slackbot-id"]);
                        handler.Handle(message);
                    }

                    Console.WriteLine(messageRaw);
                }
            }
        }

        static async Task<string> GetWebsocketUrl()
        {
            var token = Configuration["slackbot-token"];
            var startWebsocketUri = Configuration["slack-rtm-url"];
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
