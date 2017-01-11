namespace CharlesBukowskiSlackBot
{
    public class IncomingMessage
    {
        public string type { get; set; }
        public string user { get; set; }
        public string text { get; set; }
        public string channel { get; set; }
    }

    class MessageHandler
    {
        private IGetRandomBukowskiQuote getRandomBukowskiQuote;
        private ISendSlackMessage sendSlackMessage;
        private string slackbotId;

        public MessageHandler(IGetRandomBukowskiQuote getRandomBukowskiQuote, ISendSlackMessage sendSlackMessage, string slackbotId)
        {
            this.slackbotId = slackbotId;
            this.getRandomBukowskiQuote = getRandomBukowskiQuote;
            this.sendSlackMessage = sendSlackMessage;
        }

        public void Handle(IncomingMessage message)
        {
            if (message.text != null && message.text.Contains($"<@{slackbotId}>"))
            {
                var quote = this.getRandomBukowskiQuote.GetNextQuote();
                this.sendSlackMessage.Execute(message.channel, quote);
            }
        }
    }
}