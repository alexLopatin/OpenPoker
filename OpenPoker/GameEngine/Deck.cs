using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
        public Deck()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 2; j < 15; j++)
                    cards.Add(new Card((Card.Suit)i, j));
        }
        public void Shuffle()
        {
            Random rand = new Random();
            cards = cards.OrderBy(x => rand.Next()).ToList();
        }
        public Card Pop()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}
