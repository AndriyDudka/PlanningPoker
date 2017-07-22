using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using PlanningPocker.Services;

namespace PlanningPocker.Handlers
{
    public class WebSocketHandler
    {
        public const int BufferSize = 4096;
        public static List<IClientService> sockets = new List<IClientService>();
        WebSocket socket;
        private static int id;
        
        public WebSocketHandler(WebSocket socket)
        {
            id++;
            this.socket = socket;
            sockets.Add(new ClientService(socket, id));          
        }

        async Task Message()
        {
            int current = id;
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {              
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                var str = Encoding.ASCII.GetString(seg.Array, seg.Offset, incoming.Count);
                ClientStatus clientStatus = JsonConvert.DeserializeObject<ClientStatus>(str);

                switch (clientStatus.Status)
                {
                    case "New Client": AddNewClient(true);
                        break;
                    case "Vote": Vote(clientStatus.Mark, current);
                        break;
                    case "Close": CloseConnection(current);
                        break;
                    case "Reset": Reset();
                        break;
                }
            }
        }

        public async Task AddNewClient(bool newClient)
        {
            for (int i = 0; i < sockets.Count; i++)
            {
                if (sockets[i].Socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[BufferSize];
                    var str = "clean";
                    buffer = Encoding.ASCII.GetBytes(str);
                    var outgoing = new ArraySegment<byte>(buffer, 0, str.Length);
                    await sockets[i].Socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                    for (int j = 0; j < sockets.Count; j++)
                    {                                           
                        str = sockets[j].Client.Mark;
                        if ((i == sockets.Count - 1 && newClient) || !sockets[j].Client.Front) str = "X";
                        buffer = Encoding.ASCII.GetBytes(str);
                        outgoing = new ArraySegment<byte>(buffer, 0, str.Length);
                        await sockets[i].Socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);                    
                    }                  
                }
                else
                {
                    sockets.Remove(sockets[i]);
                }
            }
        }

        public void Vote(string mark, int id)
        {
            for (int i = 0; i < sockets.Count; i++)
            {
                if (sockets[i].Client.Id == id)
                {
                    sockets[i].Client.Mark = mark;
                    break;
                }
            }
            ShowCards();
        }

        public void ShowCards()
        {
            for (int i = 0; i < sockets.Count; i++)
                if (sockets[i].Client.Mark == "X") return;

            for (int i = 0; i < sockets.Count; i++)
                sockets[i].Client.Front = true;
            AddNewClient(false);
        }

        public void CloseConnection(int id)
        {
            for (int i = 0; i < sockets.Count; i++)
                if (sockets[i].Client.Id == id)
                {
                    sockets.Remove(sockets[i]);
                    break;
                }         
        }

        public void Reset()
        {
            for (int i = 0; i < sockets.Count; i++)
            {
                sockets[i].Client.Mark = "X";
            }
            AddNewClient(false);
        }

        static async Task Acceptor(HttpContext hc, Func<Task> fake)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebSocketHandler(socket);
            await h.Message();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(WebSocketHandler.Acceptor);
        }

        public class ClientStatus
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("mark")]
            public string Mark { get; set; }
        }
    }
}
