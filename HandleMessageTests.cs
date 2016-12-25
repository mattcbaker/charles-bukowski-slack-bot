﻿using NUnit.Framework;

namespace CharlesBukowskiSlackBot
{
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

    [TestFixture]
    public class HandleMessageTests
    {
        [Test]
        public void when_handing_message_directed_at_bot_it_should_send_random_quote()
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
        public void when_handling_message_not_directed_at_bot_it_should_not_send_random_quote()
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
