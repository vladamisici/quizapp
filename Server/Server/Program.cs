using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizServer
{
    class Program
    {
        static List<WebSocket> connectedSockets = new List<WebSocket>();
        static Dictionary<string, WebSocket> testTakers = new Dictionary<string, WebSocket>();
        static Dictionary<string, WebSocket> evaluators = new Dictionary<string, WebSocket>();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            Task.Run(() => StartServer()).Wait();
        }

        static async Task StartServer()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = webSocketContext.WebSocket;
                    connectedSockets.Add(webSocket);

                    _ = HandleWebSocketAsync(webSocket);
                }
            }
        }

        static async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            while (webSocket.State == WebSocketState.Open)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");

                if (message.StartsWith("register:"))
                {
                    var parts = message.Split(':');
                    var username = parts[1];
                    var role = parts[2];

                    if (role == "testtaker")
                    {
                        testTakers[username] = webSocket;
                        Console.WriteLine($"{username} registered as test taker.");
                        await BroadcastAsync($"register:{username}");
                    }
                    else if (role == "evaluator")
                    {
                        evaluators[username] = webSocket;
                        Console.WriteLine($"{username} registered as evaluator.");
                    }
                }
                else if (message.StartsWith("quiz:"))
                {
                    var parts = message.Split(':');
                    var username = parts[1];
                    var quizData = parts[2];

                    if (testTakers.ContainsKey(username))
                    {
                        await SendMessageAsync(testTakers[username], $"quiz:{quizData}");
                        Console.WriteLine($"Sent quiz to {username}");
                    }
                }
                else if (message.StartsWith("status:"))
                {
                    var parts = message.Split(':');
                    var username = parts[1];
                    var status = parts[2];
                    var score = parts.Length > 3 ? parts[3] : "";

                    foreach (var evaluator in evaluators.Values)
                    {
                        await SendMessageAsync(evaluator, $"status:{username}:{status}:{score}");
                    }
                }
            }

            connectedSockets.Remove(webSocket);
        }

        static async Task BroadcastAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in connectedSockets)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
