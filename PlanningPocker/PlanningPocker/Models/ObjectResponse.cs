﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPocker.Models
{
    public class ObjectResponse
    {
        public List<ClientResponse> ClientsResponse { get; set; }
   
        public bool VoteEnabled { get; set; }

        public bool ResetShow { get; set; }

        public class ClientResponse
        {
            public string Name { get; set; }
            public string Mark { get; set; }
        }

    }
    
}
