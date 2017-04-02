using Xunit;

namespace CharlesBukowskiSlackBot
{
    class TestableGetRandomBukowskiQuote : IGetRandomBukowskiQuote
    {
        public bool GetNextQuoteCalled { get; set; }
        public string GetNextQuoteReturnValue { get; set; }
        public string GetNextQuote()
        {
            GetNextQuoteCalled = true;
            return GetNextQuoteReturnValue;
        }
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

    public class HandleMessageTests
    {
        [Fact]
        public void when_handing_message_directed_at_bot_it_should_send_random_quote()
        {
            var botIdentifier = "U3W";
            var message = new IncomingMessage
            {
                text = $"<@{botIdentifier}> some test message",
                channel = "test-channel"
            };
            var getRandomBukowskiQuote = new TestableGetRandomBukowskiQuote();
            getRandomBukowskiQuote.GetNextQuoteReturnValue = "some random quote";
            var sendSlackMessage = new TestableSendSlackMessage();

            new MessageHandler(getRandomBukowskiQuote, sendSlackMessage, botIdentifier).Handle(message);

            Assert.Equal(sendSlackMessage.ExecuteCalledWithChannel, message.channel);
            Assert.Equal(sendSlackMessage.ExecuteCalledWithMessage, getRandomBukowskiQuote.GetNextQuoteReturnValue);
        }

        [Fact]
        public void when_handling_message_not_directed_at_bot_it_should_not_send_random_quote()
        {
            var botIdentifier = "U3W";
            var message = new IncomingMessage
            {
                text = $"<@some-other-user> some test message",
                channel = "test-channel"
            };
            var getRandomBukowskiQuote = new TestableGetRandomBukowskiQuote();
            var sendSlackMessage = new TestableSendSlackMessage();

            new MessageHandler(getRandomBukowskiQuote, sendSlackMessage, botIdentifier).Handle(message);

            Assert.False(sendSlackMessage.ExecuteCalled);
        }
    }
}
