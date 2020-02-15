using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.Hubs;

namespace OpenPoker.GameEngine
{
    public class GameRoom
    {
        public Game game;
        public string name;
        public int id;
        public EventHandler<GameUpdateArgs> OnGameUpdate;
        public EventHandler<GameUpdateArgs> OnGameClose;
        private void TurnHandler(object sender, GameUpdateArgs args)
        {
            if (OnGameUpdate != null)
                OnGameUpdate.Invoke(this, args);
        }
        public int GetNewIdPlayer()
        {
            int i = 0;
            for (i = 0; i < 6; i++)
                if (game.players.All(p => p.Id != i))
                    break;
            return i;
        }
        private void CloseHandler(object sender, GameUpdateArgs args)
        {
            if (OnGameClose != null)
                OnGameClose.Invoke(this, args);
        }
        public GameRoom(string name, int id, Game game = null)
        {
            if (game == null)
                game = new Game();
            this.game = game;
            this.name = name;
            this.id = id;
            game.OnGameUpdate += TurnHandler;
            game.OnGameClose += CloseHandler;
            game.Start();
        }
    }
}
