using System;

namespace ree7.WakeMyPC.LighthouseCore.Utils
{
    public class Log
    {
        public delegate void LoggingMethod(string message);
        private static LoggingMethod _method = Console.WriteLine;

        public static void DefineMethod(LoggingMethod m)
        {
            _method = m;
        }

        public static void d(string message)
        {
            _method(message);
        }
    }
}
