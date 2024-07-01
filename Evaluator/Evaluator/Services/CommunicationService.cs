using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizApp.Services
{
    public class CommunicationService
    {
        private ClientWebSocket _clientWebSocket;

        public async Task ConnectAsync(Uri uri)
        {
            try
            {
                _clientWebSocket = new ClientWebSocket();
                await _clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Trace.WriteLine($"[DEBUG:ConnectAsync] Connected to {uri} @{DateTime.Now}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:ConnectAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_clientWebSocket.State == WebSocketState.Open)
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Trace.WriteLine($"[DEBUG:SendMessageAsync] Sent message: {message} @{DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:SendMessageAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        public async Task ListenAsync(Action<string> onMessageReceived)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    onMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:ListenAsync] {ex.Message} @{DateTime.Now}");
            }
        }
    }
}
