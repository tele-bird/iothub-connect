using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public static class Utils
    {
        public static string GenerateText(bool success)
        {
            return success ? "succeeded" : "failed";
        }

        public static void WriteToConsole(bool success, string message)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (!success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(message);
            if (!success)
            {
                Console.ForegroundColor = oldColor;
            }
        }

        public static string GenerateText(ConsoleTelemetry telemetryState)
        {
            return telemetryState != null ? $"{telemetryState}" : "(not started)";
        }
    }
}
