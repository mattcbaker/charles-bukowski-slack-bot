using NUnit.Framework;

namespace CharlesBukowskiSlackBot
{
    interface IGetRandomBukowskiQuote
    {
        string Execute();
    }

    class TestableGetRandomBukowskiQuote : IGetRandomBukowskiQuote
    {
        public bool ExecuteCalled { get; set; }
        public string ExecuteReturnValue { get; set; }
        public string Execute()
        {
            ExecuteCalled = true;
            return ExecuteReturnValue;
        }
    }

    interface ISendSlackMessage
    {
        void Execute(string channel, string message);
    }

    class TestableSendSlackMessage : ISendSlackMessage
    {
        public string ExecuteCalledWithChannel { get; set; }
        public string ExecuteCalledWithMessage { get; set; }
        public bool ExecuteCalled { get; set; }

        public void Execute(string channel, string message)
        {
            ExecuteCalledWithChannel = channel;
            ExecuteCalledWithMessage = message;
            ExecuteCalled = true;
        }
    }

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

    [TestFixture]
    public class HandleMessageTests
    {
        [Test]
        public void when_handing_valid_message_directed_at_bot_it_should_send_random_quote()
        {
            var botIdentifier = "U3W";
            var message = new ConsoleApplication.Program.IncomingMessage
            {
                text = $"<@{botIdentifier}> some test message",
                channel = "test-channel"
            };
            var getRandomBukowskiQuote = new TestableGetRandomBukowskiQuote();
            getRandomBukowskiQuote.ExecuteReturnValue = "some random quote";
            var sendSlackMessage = new TestableSendSlackMessage();

            new MessageHandler(getRandomBukowskiQuote, sendSlackMessage, botIdentifier).Handle(message);

            Assert.That(sendSlackMessage.ExecuteCalledWithChannel, Is.EqualTo(message.channel));
            Assert.That(sendSlackMessage.ExecuteCalledWithMessage, Is.EqualTo(getRandomBukowskiQuote.ExecuteReturnValue));
        }

        [Test]
        public void when_handling_valid_message_not_directed_at_bot_it_should_not_send_random_quote()
        {
            var botIdentifier = "U3W";
            var message = new ConsoleApplication.Program.IncomingMessage
            {
                text = $"<@some-other-user> some test message",
                channel = "test-channel"
            };
            var getRandomBukowskiQuote = new TestableGetRandomBukowskiQuote();
            var sendSlackMessage = new TestableSendSlackMessage();

            new MessageHandler(getRandomBukowskiQuote, sendSlackMessage, botIdentifier).Handle(message);

            Assert.That(sendSlackMessage.ExecuteCalled, Is.EqualTo(false));
        }
    }
}
