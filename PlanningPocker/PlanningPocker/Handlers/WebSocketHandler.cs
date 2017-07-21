using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PlanningPocker.Services;

namespace PlanningPocker.Handlers
{
    public class WebSocketHandler
    {
        public const int BufferSize = 4096;

        public static List<IClientService> sockets = new List<IClientService>();

        WebSocket socket;
        
        public WebSocketHandler(WebSocket socket)
        {       
            this.socket = socket;
            sockets.Add(new ClientService(socket));
            AddCard();
        }

        async Task AddCard()
        {
            for (int i = 0; i < sockets.Count - 1; i++)
            {
                var socket = sockets[i].Socket;

                if (socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[BufferSize];
                    var str = "X";
                    buffer = Encoding.ASCII.GetBytes(str);
                    var outgoing = new ArraySegment<byte>(buffer, 0, str.Length);
                    await socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    sockets.Remove(sockets[i]);
                }
            }
        }

        async Task SayHello()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {              
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                var str = "X";
                buffer = Encoding.ASCII.GetBytes(str);             
                var outgoing = new ArraySegment<byte>(buffer, 0, str.Length);            
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
