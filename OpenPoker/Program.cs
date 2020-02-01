using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ConsolePoker;

namespace OpenPoker
{
    public class GameRoom
    {
        public Game game;
        public string name;
        public int id;
        public GameRoom(string name, int id, Game game = null)
        {
            if (game == null)
                game = new Game();
            this.game = game;
            this.name = name;
            this.id = id;
            game.Start();
        }
    }
    public static class Server
    {
        public static List<GameRoom> rooms = new List<GameRoom>();
        
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Server.rooms.Add(new GameRoom("First one", 1));
            Server.rooms.Add(new GameRoom("Second one", 2));
            Server.rooms.Add(new GameRoom("Third one", 3));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
