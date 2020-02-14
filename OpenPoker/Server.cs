﻿using Microsoft.AspNetCore.SignalR;
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
        public Task SendUpdateData(IClientProxy caller, int roomId);
        public Task Reject(IClientProxy caller);
        public Task SendBetQuery(string connectionId, int minBet);
    }
    public class Server : IServer
    {
        public List<GameRoom> rooms { get; private set; } = new List<GameRoom>();
        private IHubContext<RoomHub> HubContext { get; set; }
        public void SendPlayerState(object sender, GameUpdateArgs args)
        {
            var room = (GameRoom)sender;
            foreach(KeyValuePair<string, object> kvp in args.ActionArgumentsPairs)
            {
                HubContext.Clients.Group("/room/" + room.id.ToString())
                    .SendAsync(kvp.Key, kvp.Value);
            }
        }
        
        public async Task SendUpdateData(IClientProxy caller, int roomId)
        {
            var room = rooms.Find(p => p.id == roomId);
            var args = room.game.GetUpdateData();
            foreach (KeyValuePair<string, object> kvp in args.ActionArgumentsPairs)
                await caller.SendAsync(kvp.Key, kvp.Value);
        }
        public async Task Reject(IClientProxy caller)
        {
            await caller.SendAsync("Reject", "Room is full!");
        }
        public void CreateGame(GameRoom room)
        {
            rooms.Add(room);
            room.OnGameUpdate += SendPlayerState;
        }
        public async Task SendBetQuery(string connectionId, int minBet)
        {
            var client = HubContext.Clients.Client(connectionId);
            await client.SendAsync("DoBet", minBet);
        }
        public Server(IHubContext<RoomHub> hubContext)
        {
            
            HubContext = hubContext;
            //test rooms
            CreateGame(new GameRoom("First one", 1));
            CreateGame(new GameRoom("Second one", 2));
            CreateGame(new GameRoom("Third one", 3));
        }
    }
}
