using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;
using OpenPoker.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker
{
    public interface IServer
    {
        public List<GameRoom> rooms { get; }
        public void CreateGame(GameRoom room);
        public IHubContext<RoomHub> hubContext { get; set; }
    }
    public class Server : IServer
    {
        public List<GameRoom> rooms { get; private set; } = new List<GameRoom>();
        public IHubContext<RoomHub> hubContext { get; set; }
        public void SendPlayerState(object sender, GameTurnArgs args)
        {
            var room = (GameRoom)sender;
            if (hubContext != null)
                hubContext.Clients.Group("/room/" + room.id.ToString()).SendAsync("UpdateGame", args.players, args.deck);
            if(args.action!= "None" && hubContext != null)
                hubContext.Clients.Group("/room/" + room.id.ToString()).SendAsync("UpdatePlayer", args.playerId, args.action);

        }
        public void CreateGame(GameRoom room)
        {
            rooms.Add(room);
            room.OnGameTurn += SendPlayerState;

        }
        public Server(IHubContext<RoomHub> hubContext)
        {
            this.hubContext = hubContext;
            //test rooms
            CreateGame(new GameRoom("First one", 1));
            CreateGame(new GameRoom("Second one", 2));
            CreateGame(new GameRoom("Third one", 3));
        }
    }
}
