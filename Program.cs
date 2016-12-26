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
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json"); //TODO: this doesn't work if app is started outside of root directory.

            Configuration = builder.Build();
            ConnectToWebsocket().Wait();
            resetEvent.WaitOne();
        }

        static async Task ConnectToWebsocket()
        {
            var websocketUri = new Uri(await GetWebsocketUrl());
            var socketConnection = new SlackSocketConnection(websocketUri);

            socketConnection.OnMessage((msg) =>
            {
                new MessageHandler(new GetRandomBukowskiQuote(), new SendSlackMessage(socketConnection.ClientWebSocket),
                    Configuration["slackbot-id"]).Handle(msg);
            });

            socketConnection.Connect();
        }

        class HelloRTMSession
        {
            public string url { get; set; }
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
