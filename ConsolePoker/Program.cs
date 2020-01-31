using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ConsolePoker
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
                return res;
            }
            return 0;
        }
    }
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
    public class Game
    {
        List<Player> players;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellation;
        public List<Card> cards = new List<Card>();
        public Game()
        {
            tokenSource = new CancellationTokenSource();
            cancellation = tokenSource.Token;
            players = new List<Player>();
            for (int i = 0; i < 2; i++)
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
        
        public int CalculateCombination(List<Card> playerCards)
        {
            List<Card> allCards = new List<Card>(playerCards);
            allCards.AddRange(cards);
            int res = 0;


            // 0 - старшая карта
            playerCards = playerCards.OrderBy(x => x.rank).ToList();
            res = 100 * playerCards[1].rank + playerCards[0].rank;

            // 1,2 - пара или две пары
            allCards = allCards.OrderBy(x => x.rank).ToList();
            List<int> rankPairs = new List<int>();
            for (int i = 0; i < 6; i++)
                if (allCards[i].rank == allCards[i + 1].rank)
                    rankPairs.Add(allCards[i].rank);
            rankPairs.Sort();
            if (rankPairs.Count == 1)
            {
                res = 100000000 + rankPairs[0] * 1000000; //доделать
            }
            else if (rankPairs.Count == 2)
                res = 200000000 + rankPairs[1] * 1000000 + rankPairs[0] * 10000;


            // 3 - тройка
            int rankTrip = 0;
            for (int i = 0; i < 5; i++)
                if (allCards[i].rank == allCards[i + 1].rank &&
                    allCards[i + 1].rank == allCards[i + 2].rank)
                    rankTrip = allCards[i].rank;
            if (rankTrip != 0)
                res = 300000000 + rankTrip * 10000;

            // 4 - стрит
            int rankStreet = 0;
            for (int i = 0; i < 3; i++)
                for (int j = i; j < i + 4; j++)
                    if (allCards[j].rank + 1 != allCards[j + 1].rank)
                        break;
                    else if (j == i + 3)
                        rankStreet = allCards[j + 1].rank;
            if (rankStreet != 0)
                res = 400000000 + rankStreet;

            // 5 - флеш
            allCards = allCards.OrderBy(x => (int)x.suit * 100 + x.rank).ToList();
            int rankFlash = 0;
            for (int i = 0; i < 3; i++)
                for (int j = i; j < i + 4; j++)
                    if (allCards[j].suit != allCards[j + 1].suit)
                        break;
                    else if (j == i + 3)
                        rankFlash = allCards[j + 1].rank;
            if (rankFlash != 0)
                res = 500000000 + rankFlash;

            // 6 - фуллхаус
            int rankFullHouse = 0;
            for (int i = 0; i < rankPairs.Count; i++)
                if (rankFullHouse < rankPairs[i] && rankPairs[i] != rankTrip)
                    rankFullHouse = rankPairs[i];
            if (rankFullHouse != 0 && rankTrip != 0)
                res = 600000000 + 100 * rankTrip + rankFullHouse;

            // 7 - каре
            int rankKare = 0;
            allCards = allCards.OrderBy(x => x.rank).ToList();
            for (int i = 0; i < 4; i++)
                for (int j = i; j < i + 3; j++)
                    if (allCards[j].rank != allCards[j + 1].rank)
                        break;
                    else if (j == i + 2)
                        rankKare = allCards[j + 1].rank;
            if (rankKare != 0)
                res = 700000000 + 100* rankKare;

            // 8 - стрит флеш
            int streetFlash = 0;
            allCards = allCards.OrderBy(x => (int)x.suit * 100 + x.rank).ToList();
            for (int i = 0; i < 3; i++)
                for (int j = i; j < i + 4; j++)
                    if (allCards[j].suit != allCards[j + 1].suit || allCards[j].rank + 1 != allCards[j + 1].rank)
                        break;
                    else if (j == i + 3)
                        streetFlash = allCards[j + 1].rank;
            if (streetFlash != 0)
                res = 800000000 + streetFlash;

            // 9 - флеш рояль
            for (int i = 0; i < 3; i++)
                for (int j = i; j < i + 4; j++)
                    if (allCards[j].suit != allCards[j + 1].suit || allCards[j].rank + 1 != allCards[j + 1].rank)
                        break;
                    else if (j == i + 3 && allCards[j + 1].rank == 14)
                        res = 900000000;

            return res;
        }
        private string IntCombToStr(int comb)
        {
            int tier = comb / 100000000;
            switch(tier)
            {
                case 0:
                    return "Higher card of " + ((comb % 100000000) / 100).ToString();
                case 1:
                    return "Pair of " + ((comb % 100000000) / 1000000).ToString();
                case 2:
                    return "Two pair (cards are " + ((comb % 100000000) / 1000000).ToString() + " and " + ((comb % 1000000) / 10000).ToString() + ")";
                case 3:
                    return "Triple (higher card is " + ((comb % 1000000) / 10000).ToString() + ")";
                case 4:
                    return "Street (higher card is " + ((comb % 100000000) ).ToString() + ")";
                case 5:
                    return "Flash (higher card is " + ((comb % 100000000) ).ToString() + ")";
                case 6:
                    return "Fullhouse (triple card is " + ((comb % 100000000)/100).ToString() + " and pair card is " + (comb % 20).ToString() + ")";
                case 7:
                    return "Kare (card is " + ((comb % 100000000)/100).ToString() + ")";
                case 8:
                    return "Street Flash (higher card is " + ((comb % 100000000) ).ToString() + ")";
                case 9:
                    return "Flash Royale";
            }
            return "";
        }
        private void FindWinner(int cash)
        {
            Dictionary<Player, (int, int)> playerStats = new Dictionary<Player, (int, int)>();

            for (int i = 0; i < players.Count; i++)
                playerStats[players[i]] = (CalculateCombination(players[i].cards), i + 1);

            int max = playerStats.Max(x => x.Value.Item1);
            var winners = playerStats.Where(x => x.Value.Item1 == max).ToList();
            ShowCards();
            foreach(KeyValuePair<Player, (int, int)> kvp in winners)
            {
                Player p = kvp.Key;
                Console.WriteLine("Player {0} has won {1}$ with a {2}!", kvp.Value.Item2, cash/winners.Count, IntCombToStr(kvp.Value.Item1));
            }
            Console.ReadKey();
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
