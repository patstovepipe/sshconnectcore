using System;
using System.IO;

namespace SSHConnectCore.Utilities
{
    public class Logger
    {
        public static string logFile = AppContext.BaseDirectory + "\\log.txt";

        public static void Log(string tag, string message)
        {
            string logMessage = $"{DateTime.Now}: { tag }: {message}";
            File.AppendAllText(logFile, logMessage + Environment.NewLine);
        }

        public static string[] Logs()
        {
            if (File.Exists(logFile))
                return File.ReadAllLines(logFile);
            else
                return new string[0];
        }
    }
}
