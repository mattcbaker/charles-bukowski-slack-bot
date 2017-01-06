using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        //TODO: implement websocket reconnect, slack will sometimes close socket connection.
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
            new SlackSocketConnection(websocketUri).Connect();
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
            using (var response = await client.GetAsync(uri))
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<HelloRTMSession>(responseContent).url;
            }
        }
    }
}
