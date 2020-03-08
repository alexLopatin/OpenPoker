using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPoker.Logging;
using OpenPoker.Infrastructure;
using OpenPoker.Models;
using Microsoft.EntityFrameworkCore;

namespace OpenPoker.GameEngine
{
    public class GameRoom
    {
        public Game game;
        public string name;
        public int id;
        public EventHandler<GameUpdateArgs> OnGameUpdate;
        public EventHandler<GameUpdateArgs> OnGameClose;
        public EventHandler<GameUpdateArgs> OnGameEnd;
        public GameLogger logger;
        private void TurnHandler(object sender, GameUpdateArgs args)
        {
            if (OnGameUpdate != null)
                OnGameUpdate.Invoke(this, args);
        }
        private void LogHandler(object sender, GameUpdateArgs args)
        {
            logger.Log(args);
            if (args.isEndGameUpdate)
            {
                if (OnGameEnd != null)
                    OnGameEnd.Invoke(this, args);
                logger.Clear();
            }
        }
        public int GetNewIdPlayer()
        {
            int i = 0;
            for (i = 0; i < 6; i++)
                if (game.players.All(p => p.Id != i))
                    break;
            return i;
        }
        public int CountOfPlayers()
        {
            return game.players.Count(p => !p.IsDisconnected);
        }
        private void CloseHandler(object sender, GameUpdateArgs args)
        {
            if (OnGameClose != null)
                OnGameClose.Invoke(this, args);
        }

        public GameRoom(string name, int id, int count)
        {
            Game game = null;
            logger = new GameLogger("GameLogs");
            if (game == null)
                game = new Game();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
                game.players.Add(new BotPlayer(i));
            this.game = game;
            this.name = name;
            this.id = id;
            game.OnGameUpdate += TurnHandler;
            game.OnGameClose += CloseHandler;
            game.OnGameLog += LogHandler;
            game.Start();
        }
    }
}
