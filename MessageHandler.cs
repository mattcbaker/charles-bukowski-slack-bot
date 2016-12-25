namespace CharlesBukowskiSlackBot
{
    class MessageHandler
    {
        private IGetRandomBukowskiQuote getRandomBukowskiQuote;
        private ISendSlackMessage sendSlackMessage;
        private string botName;

        public MessageHandler(IGetRandomBukowskiQuote getRandomBukowskiQuote, ISendSlackMessage sendSlackMessage, string botName)
        {
            this.botName = botName;
            this.getRandomBukowskiQuote = getRandomBukowskiQuote;
            this.sendSlackMessage = sendSlackMessage;
        }

        public void Handle(ConsoleApplication.Program.IncomingMessage message)
        {
            if (message.text.Contains($"<@{botName}>"))
            {
                var quote = this.getRandomBukowskiQuote.Execute();
                this.sendSlackMessage.Execute(message.channel, quote);
            }
        }
    }
}