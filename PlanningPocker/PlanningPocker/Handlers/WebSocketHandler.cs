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

                if (clientRequest == null) CloseConnection(current);
                switch (clientRequest.Status)
                {
                    case "New Client":
                        {
                            var socket = sockets[sockets.Count - 1];
                            socket.Client.Name = clientRequest.Name;
                            socket.Client.VoteEnabled = sockets.Count < 2;
                            UpdateCards();
                        }
                        break;
                    case "Vote": Vote(clientRequest.Mark, current);
                        break;
                    case "Close": CloseConnection(current);
                        break;
                    case "Reset": Reset();
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
            for (int i = 0; i < sockets.Count; i++)
                if (sockets[i].Client.Front) return;

            for (int i = 0; i < sockets.Count; i++)
            if (sockets[i].Client.Id == id)
            {
                sockets[i].Client.Mark = mark;
                break;
            }
            
            await ShowCards();
        }

        async Task ShowCards()
        {
            for (int i = 0; i < sockets.Count; i++)
                if (sockets[i].Client.Mark == "X") return;

            for (int i = 0; i < sockets.Count; i++)
            {
                sockets[i].Client.Front = true;
                sockets[i].Client.VoteEnabled = false;
            }
                
            await UpdateCards();
        }

        async Task CloseConnection(int id)
        {
            for (int i = 0; i < sockets.Count; i++)
                if (sockets[i].Client.Id == id)
                {
                    sockets.Remove(sockets[i]);
                    break;
                }
            await UpdateCards();
            await ShowCards();
        }

        async Task Reset()
        {
            for (int i = 0; i < sockets.Count; i++)
            {
                sockets[i].Client.Mark = "X";
                sockets[i].Client.Front = false;
                sockets[i].Client.VoteEnabled = true;
                sockets[i].Client.ResetShow = true;
            }
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
