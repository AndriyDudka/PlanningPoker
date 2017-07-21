using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using PlanningPocker.Models;

namespace PlanningPocker.Services
{
    public class ClientService: IClientService
    {
        public Client Client { get; set; }

        public WebSocket Socket { get; set; }

        public ClientService(WebSocket socket, int id)
        {
            this.Socket = socket;

            Client = new Client
            {
                Id = id,
                Front = false,
                Mark = "X"
            };
        }

        

        public void SetFront()
        {
            Client.Front = true;
        }

        public void Vote(string mark)
        {
            Client.Mark = mark;
        }
    }
}
