using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;
using OpenPoker.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker
{
    public static class Server
    {
        public static List<GameRoom> rooms = new List<GameRoom>();
        public static IHubContext<RoomHub> hubContext = null;
        public static void SendPlayerState(object sender, GameTurnArgs args)
        {
            if (hubContext != null)
                hubContext.Clients.Group("/room/" + args.roomId.ToString()).SendAsync("UpdatePlayer", args.players, args.deck);
        }
        public static void CreateGame(GameRoom room)
        {
            rooms.Add(room);
            room.OnGameTurn += SendPlayerState;
        }
    }
}
