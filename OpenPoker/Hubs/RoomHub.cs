using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;
using OpenPoker.Models;

namespace OpenPoker.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IServer _server;
        private readonly PlayerManager playerManager;
        private readonly UserManager<User> _userManager;
        public async Task JoinRoom(string roomId)
        {
            if(!Context.User.Identity.IsAuthenticated)
            {
                await _server.RequireLogin(Clients.Caller);
                return;
            }
            var user = await _userManager.GetUserAsync(Context.User);
            int newId = playerManager.AddNewPlayer(Context.ConnectionId, roomId, user);
            if (newId >= 0)
            {
                Context.Items["roomId"] = roomId;
                await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
                await _server.SendSetupData(Clients.Caller, newId);
                await _server.SendUpdateData(Clients.Caller, Int32.Parse(roomId), false);
            }
            else
                await _server.Reject(Clients.Caller, "Room is full!");

        }
        public async Task MakeBet(string bet)
        {
            string roomId = Context.Items["roomId"] as string;
            string connectionId = Context.ConnectionId;
            await playerManager.SetPlayerBetAsync(connectionId, roomId, Int32.Parse(bet));
        }
        [Authorize(Roles ="admin")]
        public async Task ShowCards()
        {
            string roomId = Context.Items["roomId"] as string;
            await _server.SendUpdateData(Clients.Caller, Int32.Parse(roomId), true);
        }

        [Authorize(Roles = "admin")]
        public async Task Kick(int id)
        {
            string roomId = Context.Items["roomId"] as string;
            string connId = playerManager.Kick(id, roomId);
            if (connId != null)
            {
                await Groups.RemoveFromGroupAsync(connId, "/room/" + roomId);
                await _server.Reject(Clients.Client(connId), "You've been kicked");
            }
            await _server.SendUpdateData(Clients.Group("/room/" + roomId), Int32.Parse(roomId), false);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string roomId =  Context.Items["roomId"] as string;

            if (roomId == null)
                await base.OnDisconnectedAsync(exception);

            playerManager.SetPlayerDisconnected(Context.ConnectionId, roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "/room/" + roomId);
            await _server.SendUpdateData(Clients.Group("/room/" + roomId), Int32.Parse(roomId), false);
            await base.OnDisconnectedAsync(exception);
        }

        public RoomHub(IServer server, UserManager<User> userManager)
        {
            _server = server;
            _userManager = userManager;
            playerManager = new PlayerManager(_server);
        }
    }
}
