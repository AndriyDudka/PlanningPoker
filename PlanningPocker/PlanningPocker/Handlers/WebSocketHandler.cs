using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PlanningPocker.Handlers
{
    public class WebSocketHandler
    {
        public const int BufferSize = 4096;

        WebSocket socket;

        WebSocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }

        async Task SayHello()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                var outgoing = new ArraySegment<byte>(buffer, 0, incoming.Count);
                Console.WriteLine(outgoing);
                await this.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task Acceptor(HttpContext hc, Func<Task> fake)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebSocketHandler(socket);
            await h.SayHello();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(WebSocketHandler.Acceptor);
        }
    }
}
