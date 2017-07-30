using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPocker.Models
{
    public class ObjectResponse
    {
        public List<ClientResponse> clientsResponse { get; set; }

        public bool VoteEnabled { get; set; }


        public class ClientResponse
        {
            public string Name { get; set; }
            public string Mark { get; set; }
        }

    }
    
}
