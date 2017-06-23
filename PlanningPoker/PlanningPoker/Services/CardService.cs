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

        public Card GetById(int id)
        {
            return cards.FirstOrDefault(x => x.Id.Equals(id));
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void DeleteCard(int id)
        {
            cards.Remove(cards.FirstOrDefault(x => x.Id.Equals(id)));
        }
    }
}