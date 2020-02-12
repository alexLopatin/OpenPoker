using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public struct Card
    {
        public enum Suit { Spades, Hearts, Diamonds, Clubs }
        public Suit suit { get; private set; }
        public int rank { get; private set; } // 2..14
        public Card(Suit suit, int rank)
        {
            this.suit = suit;
            this.rank = rank;
        }
        public override int GetHashCode()
        {
            return rank.GetHashCode() + suit.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Card c = (Card)obj;
            return c.rank == rank && c.suit == suit;
        }
        public override string ToString()
        {
            string r = rank.ToString();
            switch (rank)
            {
                case 11:
                    r = "Joker";
                    break;
                case 12:
                    r = "Queen";
                    break;
                case 13:
                    r = "King";
                    break;
                case 14:
                    r = "Ace";
                    break;
                default:
                    break;
            }
            return suit.ToString() + ' ' + r;
        }
    }
}
