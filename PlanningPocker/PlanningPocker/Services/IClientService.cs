using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using PlanningPocker.Models;

namespace PlanningPocker.Services
{
    public interface IClientService
    {
        Client Client { get; set; }

        WebSocket Socket { get; set; }

        void SetFront();

        void Vote(string mark);
    }
}
