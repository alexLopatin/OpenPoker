using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace OpenPoker.GameEngine
{
    public class GameTurnArgs
    {
        public Player player { get; set; }
        public int roomId { get; set; }
        public GameTurnArgs(Player player, int roomId)
        {
            this.player = player;
            this.roomId = roomId;
        }
    }
    public class GameRoom
    {
        public Game game;
        public string name;
        public int id;
        public EventHandler<GameTurnArgs> OnGameTurn;
        private void TurnHandler(object sender, TurnArgs args)
        {
            if (OnGameTurn != null)
                OnGameTurn.Invoke(this, new GameTurnArgs(args.player, id));
        }
        public GameRoom(string name, int id, Game game = null)
        {
            if (game == null)
                game = new Game();
            this.game = game;
            this.name = name;
            this.id = id;
            game.OnTurnMade += TurnHandler;
            OnGameTurn += 
            game.Start();
        }
    }
}
