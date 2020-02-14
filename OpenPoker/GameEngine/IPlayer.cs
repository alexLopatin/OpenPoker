using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public interface IPlayer
    {
        public int Id { get; }
        public bool IsDisconnected { get; set; }
        public List<Card> cards { get; set; }
        public int bet { get; set; }
        public Task<int> DoBet(int minBet);
    }
}
