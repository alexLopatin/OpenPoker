using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ConsolePoker
{
    struct Card
    {
        public enum Suit { Spades, Hearts, Diamonds, Clubs }
        public Suit suit { get; private set; }
        public int rank { get; private set; } // 2..14
        public Card(Suit suit, int rank)
        {
            this.suit = suit;
            this.rank = rank;
        }
        public override string ToString()
        {
            string r = rank.ToString();
            switch(rank)
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
    class Player
    {
        public List<Card> cards = new List<Card>();
        public int bet = 0;
        public async Task<int> DoBet(int minBet)
        {
            int nb = Int32.Parse(Console.ReadLine());
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
                return res;
            }
            return 0;
        }
    }
    class Deck
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
    class Game
    {
        List<Player> players;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellation;
        private List<Card> cards = new List<Card>();
        public Game()
        {
            tokenSource = new CancellationTokenSource();
            cancellation = tokenSource.Token;
            players = new List<Player>();
            for (int i = 0; i < 6; i++)
                players.Add(new Player());
        }
        private void PrintState()
        {
            for(int i = 0; i < players.Count; i++)
            {
                Console.WriteLine("Player {0}:", i + 1);
                Console.WriteLine("  cards: {0}, {1}",
                    players[i].cards[0].ToString(),
                    players[i].cards[1].ToString());
                Console.WriteLine("  bet: {0}", players[i].bet);
            }
        }
        private async void Tick()
        {
            int curBlind = -1;
            int curBetting = 0;
            int minBet = 0;
            int countInGame = players.Count;
            Deck deck = new Deck();
            int curCycle = 0;
            int cash = 0;
            while(true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    Console.WriteLine("task canceled");
                    break;
                }

                if (curCycle == 0)
                {
                    deck = new Deck();
                    deck.Shuffle();
                    foreach(Player p in players)
                    {
                        p.cards.Clear();
                        p.cards.Add(deck.Pop());
                        p.cards.Add(deck.Pop());
                        p.bet = 0;
                    }
                    curBlind++;
                    if (curBlind == players.Count)
                        curBlind = 0;
                    if (curBlind == 0)
                    {
                        curBlind = 0;
                        players[0].bet = 100;
                        players[players.Count - 1].bet = 50;
                    }
                    else
                    {
                        players[curBlind].bet = 100;
                        players[curBlind - 1].bet = 50;
                    }
                    PrintState();
                    curBetting = curBlind;
                    minBet = 100;
                    cash += 150;
                }

                int i = 0;
                int l = players.Count;
                while( i < l)
                {
                    if(players[(i + curBetting) % players.Count].bet >= 0)
                    {
                        Console.WriteLine("Player {0} betting: ", (i + curBetting) % players.Count + 1);
                        cash += await players[(i + curBetting) % players.Count].DoBet(minBet);
                        if (players[(i + curBetting) % players.Count].bet == -1)
                            countInGame--;
                        if (countInGame == 1)
                            break;
                        if (players[(i + curBetting) % players.Count].bet > minBet)
                        {
                            minBet = players[(i + curBetting) % players.Count].bet;
                            curBetting = (i + curBetting + 1) % players.Count;
                            PrintState();
                            i = 0;
                            l = players.Count - 1;
                            continue;
                        }
                        PrintState();
                    }
                    i++;
                }
                curBetting = (i + curBetting) % players.Count;

                if(countInGame == 1)
                {
                    Player winner;
                    for (i = 0; i < players.Count; i++)
                        if (players[i].bet >= 0)
                        {
                            winner = players[i];
                            break;
                        }        
                    Console.WriteLine("Player {0} has won {1}$!", i, cash);

                    foreach (Player p in players)
                    {
                        p.cards.Clear();
                        p.bet = 0;
                    }
                    cards.Clear();
                    curCycle = 0;
                    cash = 0;
                    continue;
                }

                if(curCycle == 0)
                {
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    ShowCards();
                    curCycle++;
                }
                else if(curCycle == 1 || curCycle == 2)
                {
                    cards.Add(deck.Pop());
                    ShowCards();
                    curCycle++;
                }
                else
                {
                    FindWinner(cash);
                    foreach (Player p in players)
                    {
                        p.cards.Clear();
                        p.bet = 0;
                    }
                    cards.Clear();
                    curCycle = 0;
                    cash = 0;
                    Console.WriteLine("EndGame");
                }
            }
        }
        private void FindWinner(int cash)
        {
            Dictionary<Player, (int, int, int)> playerStats = new Dictionary<Player, (int, int, int)>();

            for(int i = 0; i < players.Count; i++)
            {
                List<Card> endCards = new List<Card>(players[i].cards);
                endCards.AddRange(cards);

            }

            int max = playerStats.Max(x => x.Value.Item1*100000 + x.Value.Item2);
            var winners = playerStats.Where(x => x.Value.Item1 * 100000 + x.Value.Item2 == max).ToList();
            foreach(KeyValuePair<Player, (int, int, int)> kvp in winners)
            {
                Player p = kvp.Key;
                Console.WriteLine("Player {0} has won {1}$!", kvp.Value.Item3, cash/winners.Count);
            }
        }
        private void ShowCards()
        {
            Console.WriteLine("Shown cards:");
            foreach (Card c in cards)
                Console.WriteLine(c.ToString());
        }
        public void Shutdown()
        {
            tokenSource.Cancel();
        }
        public async void Start()
        {
            //await Task.Run(() => Tick());
            Tick();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}
