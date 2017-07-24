using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlanningPocker.Models;

namespace PlanningPocker.Services
{
    public class CardService
    {
        public List<Card> card = new List<Card>()
        {
            new Card {CardName = "0"},
            new Card {CardName = "1/2"},
            new Card {CardName = "1"},
            new Card {CardName = "2"},
            new Card {CardName = "3"},
            new Card {CardName = "5"},
            new Card {CardName = "8"},
            new Card {CardName = "13"},
            new Card {CardName = "20"},
            new Card {CardName = "40"},
            new Card {CardName = "100"},
            new Card {CardName = "infinity"},
            new Card {CardName = "?"},
            new Card {CardName = "coffee"}
        };
    }
}
