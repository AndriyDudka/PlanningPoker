using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PlanningPocker.Services;
using PlanningPocker.Models;
using static PlanningPocker.Models.ObjectResponse;

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
            if (current == 1) sockets[0].Client.ResetShow = true;
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {              
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                var str = Encoding.ASCII.GetString(seg.Array, seg.Offset, incoming.Count);
                ClientRequest clientRequest = JsonConvert.DeserializeObject<ClientRequest>(str);

                if (clientRequest == null) await CloseConnection(current);
                switch (clientRequest.Status)
                {
                    case "New Client":
                        {
                            var socket = sockets[sockets.Count - 1];
                            socket.Client.Name = clientRequest.Name;
                            socket.Client.VoteEnabled = sockets.Count < 2;
                            await UpdateCards();
                        }
                        break;
                    case "Vote": await Vote(clientRequest.Mark, current);
                        break;
                    case "Close": await CloseConnection(current);
                        break;
                    case "Reset": await Reset();
                        break;
                }
            }
        }

        async Task UpdateCards()
        {
            List<ClientResponse> clientsResponse = new List<ClientResponse>();
            ObjectResponse objResponse = new ObjectResponse();
            var bufer = new byte[BufferSize];

            foreach (var socket in sockets)
            {
                ClientResponse client = new ClientResponse
                {
                    Name = socket.Client.Name,
                    Mark = socket.Client.Front ? socket.Client.Mark : "X"
                };
                clientsResponse.Add(client);
            }
            objResponse.ClientsResponse = clientsResponse;
            
            foreach (var socket in sockets)
            {              
                objResponse.VoteEnabled = socket.Client.VoteEnabled;
                objResponse.ResetShow = socket.Client.ResetShow;
                
                var str = JsonConvert.SerializeObject(objResponse);               
                bufer = Encoding.ASCII.GetBytes(str);
                var response = new ArraySegment<byte>(bufer, 0, str.Length);
                await socket.Socket.SendAsync(response, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        async Task Vote(string mark, int id)
        {
            foreach (var socket in sockets)
                if (socket.Client.Front) return;

            sockets.Find(x => x.Client.Id == id).Client.Mark = mark;
            
            await ShowCards();
        }

        async Task ShowCards()
        {
            foreach (var socket in sockets)
                if (socket.Client.Mark == "X") return;

            sockets.ForEach(x =>
                {
                    x.Client.Front = true;
                    x.Client.VoteEnabled = false;
                });
                
            await UpdateCards();
        }

        async Task CloseConnection(int id)
        {
            sockets.Remove(sockets.Find(x => x.Client.Id == id));         
            await UpdateCards();
            await ShowCards();
        }

        async Task Reset()
        {
            sockets.ForEach(x => 
            {
                x.Client.Mark = "X";
                x.Client.Front = false;
                x.Client.VoteEnabled = x.Client.ResetShow = true;
            });
            
            await UpdateCards();
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

        public class ClientRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("mark")]
            public string Mark { get; set; }
        }

        
    }
}
