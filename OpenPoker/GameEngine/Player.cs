using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class Player
    {
        public List<Card> cards = new List<Card>();
        public int bet = 0;
        public async Task<int> DoBet(int minBet)
        {
            //int nb = Int32.Parse(Console.ReadLine());
            int nb = 100;
            if (nb == -1)
                bet = -1;
            else if (nb < minBet)
            {
                Console.WriteLine("Write correct bet.");
                return await DoBet(minBet);
            }
            else
            {
                int res = nb - bet;
                bet = nb;
                Random rand = new Random();
                await Task.Delay(rand.Next(1, 1000));
                return res;
            }
            return 0;
        }
    }
}
