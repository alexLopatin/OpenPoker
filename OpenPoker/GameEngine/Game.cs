using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class TurnArgs
    {
        public Player player { get; set; }
        public int playerId { get; set; }
        public string action { get; set; }
        public TurnArgs(Player player, int playerId, int diff)
        {
            this.player = player;
            this.playerId = playerId;
            action = CalcAction(diff);
        }
        public TurnArgs(Player player, int playerId)
        {
            this.player = player;
            this.playerId = playerId;
            action = "None";
        }
        private string CalcAction(int diff)
        {
            if (diff == -1)
                return "Fold";
            if (diff == 0)
                return "Check";
            if (diff == -2)
                return "Call";
            if (diff > 1)
                return "Raise";
            return "None";
        }
    }
    public class EndArgs
    {
        public List<int> winners { get; set; }
        public EndArgs(List<int> winners)
        {
            this.winners = winners;
        }
    } 
    public class Game
    {
        public List<Player> players;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellation;
        public List<Card> cards = new List<Card>();
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
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine("Player {0}:", i + 1);
                Console.WriteLine("  cards: {0}, {1}",
                    players[i].cards[0].ToString(),
                    players[i].cards[1].ToString());
                Console.WriteLine("  bet: {0}", players[i].bet);
            }
        }
        public EventHandler<TurnArgs> OnTurnMade;
        public EventHandler<EventArgs> OnGameCycle;
        private async void GameCycle()
        {
            int curBlind = -1;
            int curBetting = 0;
            int minBet = 0;
            int countInGame = players.Count;
            Deck deck = new Deck();
            int cash = 0;
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    Console.WriteLine("Shutting down...");
                    break;
                }
                //Debug.WriteLine("Current cycle is " + curCycle.ToString());
                if (curCycle == 0)
                {
                    deck = new Deck();
                    deck.Shuffle();
                    foreach (Player p in players)
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
                    for(int m = 0; m < players.Count; m++)
                        if (OnTurnMade != null)
                            OnTurnMade.Invoke(this, new TurnArgs(players[m], m));
                }

                int i = 0;
                int l = players.Count;
                while (i < l)
                {
                    if (players[(i + curBetting) % players.Count].bet >= 0)
                    {
                        Console.WriteLine("Player {0} betting: ", (i + curBetting) % players.Count + 1);
                        int prevBet = players[(i + curBetting) % players.Count].bet;
                        int diff = minBet;
                        int bet = await players[(i + curBetting) % players.Count].DoBet(minBet);
                        if (minBet == bet)
                            diff = -2;
                        else if (prevBet == players[(i + curBetting) % players.Count].bet)
                            diff = 0;
                        else
                            diff = players[(i + curBetting) % players.Count].bet - minBet;

                        cash += bet;
                        if (players[(i + curBetting) % players.Count].bet == -1)
                        {
                            countInGame--;
                            diff = -1;
                        }

                        if (OnTurnMade != null)
                            OnTurnMade.Invoke(this, new TurnArgs(players[(i + curBetting) % players.Count], (i + curBetting) % players.Count, diff));

                        if (countInGame == 1)
                            break;
                        if (players[(i + curBetting) % players.Count].bet > minBet)
                        {
                            minBet = players[(i + curBetting) % players.Count].bet;
                            curBetting = (i + curBetting + 1) % players.Count;
                            PrintState();
                            i = 0;
                            continue;
                        }
                        PrintState();
                    }
                    i++;
                }
                curBetting = (i + curBetting) % players.Count;

                            l = players.Count - 1;
                if (countInGame == 1)
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

                if (curCycle == 0)
                {
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    await Task.Delay(1000);
                    if (OnGameCycle != null)
                        OnGameCycle.Invoke(this, new EventArgs());

                    ShowCards();
                    curCycle++;
                }
                else if (curCycle == 1 || curCycle == 2)
                {
                    cards.Add(deck.Pop());
                    await Task.Delay(1000);
                    if (OnGameCycle != null)
                        OnGameCycle.Invoke(this, new EventArgs());
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
        public int curCycle = 0;
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
                List<Card> kickers = new List<Card>(allCards);
                kickers.RemoveAll(x => x.rank == rankPairs[0]);
                res = 100000000 + rankPairs[0] * 1000000 +
                    kickers[kickers.Count - 1].rank * 10000 +
                    kickers[kickers.Count - 2].rank * 100 +
                    kickers[kickers.Count - 3].rank;
            }
            else if (rankPairs.Count == 2)
            {
                List<Card> kickers = new List<Card>(allCards);
                kickers.RemoveAll(x => x.rank == rankPairs[0] || x.rank == rankPairs[1]);
                int rankKicker = kickers.Last().rank;
                res = 200000000 + rankPairs[1] * 1000000 + rankPairs[0] * 10000 + rankKicker;
            }



            // 3 - тройка
            int rankTrip = 0;
            for (int i = 0; i < 5; i++)
                if (allCards[i].rank == allCards[i + 1].rank &&
                    allCards[i + 1].rank == allCards[i + 2].rank)
                    rankTrip = allCards[i].rank;
            if (rankTrip != 0)
            {
                List<Card> kickers = new List<Card>(allCards);
                kickers.RemoveAll(x => x.rank == rankTrip);
                res = 300000000 + rankTrip * 10000 +
                    kickers[kickers.Count - 1].rank * 100 +
                    kickers[kickers.Count - 2].rank;
            }


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
            {
                List<Card> kickers = new List<Card>(allCards);
                kickers.RemoveAll(x => x.rank == rankKare);
                int rankKicker = kickers.Last().rank;
                res = 700000000 + 100 * rankKare + rankKicker;
            }


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
            switch (tier)
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
                    return "Street (higher card is " + ((comb % 100000000)).ToString() + ")";
                case 5:
                    return "Flash (higher card is " + ((comb % 100000000)).ToString() + ")";
                case 6:
                    return "Fullhouse (triple card is " + ((comb % 100000000) / 100).ToString() + " and pair card is " + (comb % 20).ToString() + ")";
                case 7:
                    return "Kare (card is " + ((comb % 100000000) / 100).ToString() + ")";
                case 8:
                    return "Street Flash (higher card is " + ((comb % 100000000)).ToString() + ")";
                case 9:
                    return "Flash Royale";
            }
            return "";
        }
        private void FindWinner(int cash)
        {
            Dictionary<Player, (int, int)> playerStats = new Dictionary<Player, (int, int)>();

            for (int i = 0; i < players.Count; i++)
                if (players[i].bet != -1)
                    playerStats[players[i]] = (CalculateCombination(players[i].cards), i + 1);

            int max = playerStats.Max(x => x.Value.Item1);
            var winners = playerStats.Where(x => x.Value.Item1 == max).ToList();
            ShowCards();
            List<int> wins = new List<int>();
            foreach (KeyValuePair<Player, (int, int)> kvp in winners)
            {
                Player p = kvp.Key;
                wins.Add(kvp.Value.Item2);
                //Console.WriteLine("Player {0} has won {1}$ with a {2}!", kvp.Value.Item2, cash / winners.Count, IntCombToStr(kvp.Value.Item1));
            }
            if (OnWinnersCalc != null)
                OnWinnersCalc.Invoke(this, new EndArgs(wins));
            //Console.ReadKey();
        }
        public EventHandler<EndArgs> OnWinnersCalc;
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
            await Task.Run(() => GameCycle());
            //GameCycle();
        }
    }
}
