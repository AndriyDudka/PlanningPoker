﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPocker.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Front { get; set; }
        public string Mark { get; set; }
        public bool VoteEnabled { get; set; }
        public bool ResetShow { get; set; }
    }
}
