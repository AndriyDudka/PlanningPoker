using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanningPoker.Models;

namespace PlanningPoker.ViewModels
{
    public class CardViewModel
    {
        public List<Card> Cards { get; set; }
        public List<Scale> Marks { get; set; }
    }
}