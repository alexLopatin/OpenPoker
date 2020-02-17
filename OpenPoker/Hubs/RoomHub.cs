using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;

namespace OpenPoker.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IServer _server;
        private readonly PlayerManager playerManager;
        public async Task JoinRoom(string roomId)
        {
            if(!Context.User.Identity.IsAuthenticated)
            {
                await _server.RequireLogin(Clients.Caller);
                return;
            }
            int newId = playerManager.AddNewPlayer(Context.ConnectionId, roomId, Context.User.Identity.Name);
            if (newId > 0)
            {
                Context.Items["roomId"] = roomId;
                await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
                await _server.SendSetupData(Clients.Caller, newId);
                await _server.SendUpdateData(Clients.Caller, Int32.Parse(roomId));
            }
            else
                await _server.Reject(Clients.Caller);

        }
        public async Task MakeBet(string bet)
        {
            string roomId = Context.Items["roomId"] as string;
            string connectionId = Context.ConnectionId;
            await playerManager.SetPlayerBetAsync(connectionId, roomId, Int32.Parse(bet));
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string roomId =  Context.Items["roomId"] as string;

            if (roomId == null)
                await base.OnDisconnectedAsync(exception);

            playerManager.SetPlayerDisconnected(Context.ConnectionId, roomId);

            await _server.SendUpdateData(Clients.Others, Int32.Parse(roomId));
            await base.OnDisconnectedAsync(exception);
        }
        public RoomHub(IServer server)
        {
            _server = server;
            playerManager = new PlayerManager(_server);
        }
    }
}
