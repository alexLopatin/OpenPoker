using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace OpenPoker.GameEngine
{
    public class Game
    {
        public const int MAX_PLAYER_COUNT = 6;
        public List<IPlayer> players;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellation;
        public List<Card> cards = new List<Card>();
        private UpdateComposer updateComposer;
        public Game()
        {
            tokenSource = new CancellationTokenSource();
            cancellation = tokenSource.Token;
            updateComposer = new UpdateComposer(this);
            players = new List<IPlayer>();
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
                players.Add(new BotPlayer(i));
        }
        private void PrintState()
        {
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine("Player {0}:", i + 1);
                if(players[i].cards.Count > 0)
                Console.WriteLine("  cards: {0}, {1}",
                    players[i].cards[0].ToString(),
                    players[i].cards[1].ToString());
                Console.WriteLine("  bet: {0}", players[i].bet);
            }
        }
        public EventHandler<GameUpdateArgs> OnGameUpdate;
        public EventHandler<GameUpdateArgs> OnGameClose;
        public enum GameState
        {
            Lobby, Started, Ended
        }
        public GameState state = GameState.Lobby;
        /// <summary>
        /// Only call on IServer instance
        /// </summary>
        /// <returns></returns>
        public GameUpdateArgs GetUpdateData(bool showCards = false)
        {
            return updateComposer.UpdateAll(showCards);
        }
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
                    if (OnGameClose != null)
                        OnGameClose.Invoke(this, updateComposer.GameClose("External close"));
                    break;
                }
                if(players.Count == 0)
                {
                    if (OnGameClose != null)
                        OnGameClose.Invoke(this, updateComposer.GameClose("No players"));
                    break;
                }
                if(players.Count == 1 && curCycle == 0)
                {
                    await Task.Delay(1000);
                    countInGame = players.Count;
                    continue;
                }
                state = GameState.Started;
                //Debug.WriteLine("Current cycle is " + curCycle.ToString());
                if (curCycle == 0)
                {
                    deck = new Deck();
                    deck.Shuffle();
                    foreach (IPlayer p in players)
                    {
                        p.cards.Clear();
                        p.cards.Add(deck.Pop());
                        p.cards.Add(deck.Pop());
                        p.bet = 0;
                    }
                    curBlind++;
                    if (curBlind >= players.Count)
                        curBlind = curBlind % players.Count;
                    if (curBlind == 0)
                    {
                        players[0].bet = 100;
                        players[players.Count - 1].bet = 50;
                    }
                    else
                    {
                        players[curBlind].bet = 100;
                        players[(curBlind - 1)].bet = 50;
                        //players[curBlind % players.Count].bet = 100;
                        //players[(curBlind - 1) % players.Count].bet = 50;
                    }
                    PrintState();
                    curBetting = curBlind;
                    minBet = 100;
                    cash += 150;
                    if (OnGameUpdate != null)
                    {
                        OnGameUpdate.Invoke(this, updateComposer.UpdateAll());
                    }
                        

                }

                int i = 0;
                int l = players.Count;
                while (i < l && curCycle < 3)
                {
                    if (players[(i + curBetting) % players.Count].bet >= 0)
                    {
                        Console.WriteLine("Player {0} betting: ", (i + curBetting) % players.Count + 1);
                        int prevBet = players[(i + curBetting) % players.Count].bet;
                        int bet = await players[(i + curBetting) % players.Count].DoBet(minBet);
                        int diff = 0;
                        if (players[(i + curBetting) % players.Count].bet > minBet)
                            diff = 1;
                        if (players[(i + curBetting) % players.Count].bet == minBet)
                            diff = 0;
                        if (prevBet == players[(i + curBetting) % players.Count].bet)
                            diff = -2;
                        cash += bet;
                        if (players[(i + curBetting) % players.Count].bet == -1)
                        {
                            countInGame--;
                            diff = -1;
                        }
                        if (OnGameUpdate != null)
                            OnGameUpdate.Invoke(this, updateComposer.UpdateOnlyOnePlayer( players[ (i + curBetting) % players.Count].Id, diff));


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
                    IPlayer winner;
                    for (i = 0; i < players.Count; i++)
                        if (players[i].bet >= 0)
                        {
                            winner = players[i];
                            break;
                        }
                    string res = String.Format("Player {0} has won {1}$!", players[i].Name, cash);
                    if (OnGameUpdate != null)
                        OnGameUpdate.Invoke(this, updateComposer.EndGameUpdate(res));
                    Console.WriteLine("Player {0} has won {1}$!", players[i].Id, cash);
                    await Task.Delay(5000);
                    foreach (IPlayer p in players)
                    {
                        p.cards.Clear();
                        p.bet = 0;
                    }
                    cards.Clear();
                    curCycle = 0;
                    cash = 0;
                    countInGame = players.Count;
                    continue;
                }

                if (curCycle == 0)
                {
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    cards.Add(deck.Pop());
                    await Task.Delay(1000);
                    if (OnGameUpdate != null)
                        OnGameUpdate.Invoke(this, updateComposer.UpdateTable());
                    ShowCards();
                    curCycle++;
                }
                else if (curCycle == 1 || curCycle == 2)
                {
                    cards.Add(deck.Pop());
                    await Task.Delay(1000);
                    if (OnGameUpdate != null)
                        OnGameUpdate.Invoke(this, updateComposer.UpdateTable());
                    ShowCards();
                    curCycle++;
                }
                else
                {
                    FindWinner(cash);
                    state = GameState.Ended;
                    await Task.Delay(5000);
                    players.RemoveAll(p => p.IsDisconnected);    
                    foreach (IPlayer p in players)
                    {

                        p.cards.Clear();
                        p.bet = 0;
                    }
                    cards.Clear();
                    curCycle = 0;
                    cash = 0;
                    Console.WriteLine("EndGame");
                    countInGame = players.Count;
                    state = GameState.Lobby;
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
            Dictionary<IPlayer, int> playerStats = new Dictionary<IPlayer, int>();

            for (int i = 0; i < players.Count; i++)
                if (players[i].bet != -1)
                    playerStats[players[i]] = CalculateCombination(players[i].cards);

            int max = playerStats.Max(x => x.Value);
            var winners = playerStats.Where(x => x.Value == max).ToList();
            ShowCards();
            string res = "";
            foreach (KeyValuePair<IPlayer, int> kvp in winners)
            {
                IPlayer p = kvp.Key;
                res += String.Format("Player {0} has won {1}$ with a {2}!", p.Name, cash / winners.Count, IntCombToStr(kvp.Value));
            }
            if (OnGameUpdate != null)
                OnGameUpdate.Invoke(this, updateComposer.EndGameUpdate(res));
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
            await Task.Run(() => GameCycle());
            //GameCycle();
        }
    }
}
