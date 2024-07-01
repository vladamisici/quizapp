using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizApp.Services
{
    public class CommunicationService
    {
        private ClientWebSocket _webSocket;

        public CommunicationService()
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task ConnectAsync(Uri uri)
        {
            await _webSocket.ConnectAsync(uri, CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[1024 * 4];
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task ListenAsync(Action<string> onMessageReceived)
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessageAsync();
                onMessageReceived?.Invoke(message);
            }
        }
    }
}
