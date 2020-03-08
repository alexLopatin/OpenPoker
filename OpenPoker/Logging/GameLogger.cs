using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OpenPoker.GameEngine;
using System.Text.Json;
using System.Diagnostics;

namespace OpenPoker.Logging
{
    public struct TimeArguments
    {
        public long Time { get; set; }
        public object Argument { get; set; }
    }
    public class GameLogger
    {
        private List<KeyValuePair<GameEngine.Action, TimeArguments>> ActionTimeLogList = new List<KeyValuePair<GameEngine.Action, TimeArguments>>();
        public string Folder { get; private set; }
        private Stopwatch stopWatch;
        public GameLogger(string folder)
        {
            stopWatch = new Stopwatch();
            Folder = folder;
            
        }
        public void Log(GameUpdateArgs args)
        {
            if (!stopWatch.IsRunning)
                stopWatch.Start();
            foreach (var item in args.ActionArgumentsPairs)
                ActionTimeLogList.Add(new KeyValuePair<GameEngine.Action, TimeArguments>(
                    item.Key, new TimeArguments() { Argument = item.Value, Time = (long)stopWatch.Elapsed.TotalMilliseconds}
                    ));
        }

        public async Task SaveLog(string name)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string jsonText = JsonSerializer.Serialize(ActionTimeLogList, options);
            await File.WriteAllTextAsync(Folder + "/" + name, jsonText);
            stopWatch.Reset();
        }
        public void Clear()
        {
            ActionTimeLogList.Clear();
        }
    }
}
