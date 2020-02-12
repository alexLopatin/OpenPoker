using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;

namespace OpenPoker.Hubs
{
    public class RoomHub : Hub
    {
        IHubContext<RoomHub> _hubContext = null;
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
        }
        public void SendPlayerState(object sender, GameTurnArgs args)
        {
            Clients.Group("/room/" + args.roomId.ToString()).SendAsync("UpdatePlayer", args.player);
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("UpdatePlayer", new Player());
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public RoomHub(IHubContext<RoomHub> hubContext)
        {
            _hubContext = hubContext;
        }
    }
}
