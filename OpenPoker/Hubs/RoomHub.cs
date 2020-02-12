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
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
        }
        public RoomHub(IHubContext<RoomHub> hubContext)
        {
            Server.hubContext = hubContext;
        }
    }
}
