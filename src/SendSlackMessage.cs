using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace CharlesBukowskiSlackBot
{
    interface ISendSlackMessage
    {
        void Execute(string channel, string message);
    }

    class SendSlackMessage :  ISendSlackMessage
    {
        private ClientWebSocket webSocket;

        public SendSlackMessage(ClientWebSocket webSocket)
        {
            this.webSocket = webSocket;
        }

        public async void Execute(string channel, string message)
        {
            var outbound = new
            {
                id = 1,
                type = "message",
                channel = channel,
                text = message
            };

            var outboundBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(outbound));
            var outboundBuffer = new ArraySegment<byte>(outboundBytes);

            await webSocket.SendAsync(outboundBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}