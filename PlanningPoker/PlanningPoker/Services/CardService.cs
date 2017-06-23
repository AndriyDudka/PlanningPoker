using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanningPoker.Models;

namespace PlanningPoker.Services
{
    public class CardService
    {
        private List<Card> cards;

        public CardService()
        {
            cards = new List<Card>
            {
                new Card{Id = 1, Name = "name1"},
                new Card{Id = 2, Name = "name2"},
                new Card{Id = 3, Name = "name3"},
                new Card{Id = 4, Name = "name4"}
            };
        }

        public List<Card> GetCards()
        {
            return cards;
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void DeleteCard(string name)
        {
            cards.Remove(cards.FirstOrDefault(x => x.Name.Equals(name)));
        }
    }
}