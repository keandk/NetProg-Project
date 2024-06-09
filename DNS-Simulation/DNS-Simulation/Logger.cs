using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Simulation
{
    internal class Logger
    {
        private static readonly object _lock = new object();
        private const string LogFilePath = "log.txt";

        public static void Log(string message)
        {
            lock (_lock)
            {
                using (StreamWriter writer = File.AppendText(LogFilePath))
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    writer.WriteLine($"[{timestamp}] {message}");
                }
            }
        }
    }
}
