using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace OpenPoker.Logging
{
    public class LogDeserialize
    {
        public string FilePath { get; set; }
        public LogDeserialize(string filePath)
        {
            FilePath = filePath;
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }
        public string GetRawData()
        {
            return File.ReadAllText(FilePath);
        }
        public T Deserialize<T>()
        {
            string data = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
