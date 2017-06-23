using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanningPoker.Models;

namespace PlanningPoker.Services
{
    public class CardService
    {
        private static List<Card> cards;

        public CardService()
        {
            if (cards == null)
            cards = new List<Card>
            {
                new Card{Name = "name1"},
                new Card{Name = "name2"},
                new Card{Name = "name3"},
                new Card{Name = "name4"}
            };
        }

        public List<Card> GetCards()
        {
            return cards;
        }

        public Card GetById(string name)
        {
            return cards.FirstOrDefault(x => x.Name.Equals(name));
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