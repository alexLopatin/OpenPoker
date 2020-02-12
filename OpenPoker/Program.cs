using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenPoker.GameEngine;

namespace OpenPoker
{
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
