using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.Hubs;

namespace OpenPoker.GameEngine
{
    public class GameTurnArgs
    {
        public List<Player> players { get; set; }
        public List<Card> deck { get; set; }
        public int roomId { get; set; }
        public GameTurnArgs(List<Player> players, int roomId, List<Card> deck)
        {
            this.players = players;
            this.roomId = roomId;
            this.deck = deck;
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
                OnGameTurn.Invoke(this, new GameTurnArgs(game.players, id, game.cards));
        }
        public GameRoom(string name, int id, Game game = null)
        {
            if (game == null)
                game = new Game();
            this.game = game;
            this.name = name;
            this.id = id;
            game.OnTurnMade += TurnHandler;
            game.Start();
        }
    }
}
