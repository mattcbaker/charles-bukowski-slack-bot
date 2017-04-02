using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Slackbot;
using System.Linq;

namespace CharlesBukowskiSlackBot
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var bot = new Bot(Configuration["slackbot-token"], "charles-bukowski");
            var getRandomBukowskiQuote = new GetRandomBukowskiQuote();

            bot.OnMessage += (sender, message) =>
            {
                Console.WriteLine($"{DateTimeOffset.UtcNow}: {message.RawMessage}");
                if (message.MentionedUsers.Any(user => user == "charles-bukowski"))
                {
                    bot.SendMessage(message.Channel, getRandomBukowskiQuote.GetNextQuote());
                }
            };

            while(true){
                Console.ReadKey();
            }
        }
    }
}
