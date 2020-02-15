using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPoker.Models.Update;

namespace OpenPoker.GameEngine
{
    public class GameUpdateArgs
    {
        public List<KeyValuePair <string, object>> ActionArgumentsPairs { get; set; } = new List<KeyValuePair<string, object>>();
    }
    public class UpdateComposer
    {
        
        private Game game;
        public UpdateComposer(Game game)
        {
            this.game = game;
        }
        public GameUpdateArgs UpdateAll(bool showCards = false)
        {
            GameUpdateArgs args = new GameUpdateArgs();
            for(int i = 0; i < game.players.Count; i++)
            {
                List<Card> cards;
                //don't show players' cards who folded
                if (showCards && game.players[i].bet != -1)
                    cards = game.players[i].cards;
                else
                    cards = new List<Card>();
                args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdatePlayer", 
                    new PlayerUpdateModel(game.players[i].Id, cards, game.players[i].bet, 
                    "None", game.players[i].IsDisconnected)
                    ));

                //Personal update for specific user (i.e. showing only his cards)
                //Happen only once when player gets his cards
                if(game.players[i] is NetworkPlayer)
                {
                    var netPlayer = game.players[i] as NetworkPlayer;
                    List<object> playerParams = new List<object>();
                    playerParams.Add(netPlayer.ConnectionId);
                    playerParams.Add(new PlayerUpdateModel(netPlayer.Id, netPlayer.cards, netPlayer.bet,
                        "None", netPlayer.IsDisconnected));
                    args.ActionArgumentsPairs.Add(
                        new KeyValuePair<string, object>("Player#" + "UpdatePlayer",
                        playerParams
                        ));
                }
                

            }
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdateTable",
                    new TableUpdateModel(game.cards)
                    ));
            return args;
        }
        public GameUpdateArgs UpdateOnlyOnePlayer(int id, int diff, bool showCards = false)
        {
            string choice = "None";
            if (diff == -2)
                choice = "Check";
            if (diff == -1)
                choice = "Fold";
            if(diff == 0)
                choice = "Call";
            if (diff > 0)
                choice = "Bet";
            GameUpdateArgs args = new GameUpdateArgs();
            IPlayer p = game.players.Find(p => p.Id == id);
            List<Card> cards;
            if (showCards)
                cards = p.cards;
            else
                cards = new List<Card>();
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdatePlayer",
                    new PlayerUpdateModel(id, cards, p.bet, choice, p.IsDisconnected)
                    ));
            return args;
        }
        public GameUpdateArgs EndGameUpdate(string final)
        {
            GameUpdateArgs args = UpdateAll(true);

            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("EndGameUpdate",
                    new EndGameUpdateModel(final)
                    ));
            return args;
        }
        public GameUpdateArgs UpdateTable()
        {
            GameUpdateArgs args = new GameUpdateArgs();
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdateTable",
                    new TableUpdateModel(game.cards)
                    ));
            return args;
        }
    }
}
