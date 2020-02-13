using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPoker.GameEngine;

namespace OpenPoker.Models.Update
{
    public class TableUpdateModel
    {
        public List<Card> Cards { get; set; }
        public TableUpdateModel(List<Card> cards)
        {
            Cards = cards;
        }
    }
}
