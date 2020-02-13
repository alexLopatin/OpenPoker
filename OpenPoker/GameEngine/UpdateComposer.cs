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
        public GameUpdateArgs UpdateAll()
        {
            GameUpdateArgs args = new GameUpdateArgs();
            for(int i = 0; i < game.players.Count; i++)
            {
                args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdatePlayer", 
                    new PlayerUpdateModel(i, game.players[i].cards, game.players[i].bet)
                    ));
            }
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdateTable",
                    new TableUpdateModel(game.cards)
                    ));
            return args;
        }
        public GameUpdateArgs UpdateOnlyOnePlayer(int id, int diff)
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
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<string, object>("UpdatePlayer",
                    new PlayerUpdateModel(id, game.players[id].cards, game.players[id].bet, choice)
                    ));
            return args;
        }
        public GameUpdateArgs EndGameUpdate(string final)
        {
            GameUpdateArgs args = new GameUpdateArgs();
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
