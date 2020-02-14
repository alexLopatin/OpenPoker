using OpenPoker.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models.Update
{
    public class PlayerUpdateModel
    {
        public bool IsDisconnected { get; set; }
        public int Id { get; set; }
        public List<Card> Cards { get; set; }
        public int Bet { get; set; }
        public string Choice { get; set; }
        public PlayerUpdateModel(int id, List<Card> cards, int bet, string choice = "None", bool isDisconnected = false)
        {
            Id = id;
            Cards = cards;
            Bet = bet;
            Choice = choice;
            IsDisconnected = isDisconnected;
        }
    }
}
