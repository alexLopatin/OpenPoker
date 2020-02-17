using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class BotPlayer : IPlayer
    {
        public string Name { get { return "Bot #" + (Id + 1).ToString(); } }
        public int Id { get; private set; }
        public bool IsDisconnected { get; set; } = false;
        public List<Card> cards { get; set; } = new List<Card>();
        public int bet { get; set; } = 0;
        public BotPlayer(int id)
        {
            Id = id;
        }
        public async Task<int> DoBet(int minBet)
        {
            //int nb = Int32.Parse(Console.ReadLine());
            Random rand = new Random();
            int r = rand.Next(1, 15);
            
            int nb = 100;

            if (r < 2)
                nb = minBet + 100 * r;
            else
                nb = minBet;

            if (r == 9)
                nb = -1;

            if (nb == -1)
            {
                bet = -1;
                await Task.Delay(rand.Next(1000, 2000));
            }  
            else if (nb < minBet)
            {
                Console.WriteLine("Write correct bet.");
                return await DoBet(minBet);
            }
            else
            {
                int res = nb - bet;
                bet = nb;
                
                await Task.Delay(rand.Next(1000, 2000));
                return res;
            }
            return 0;
        }
    }
}
