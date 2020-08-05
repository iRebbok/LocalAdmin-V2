using LocalAdmin.V2.IO.Output;
using System;

namespace LocalAdmin.V2.IO
{
    /*
    * Console colors:
    * Gray - LocalAdmin log
    * Red - critical error
    * DarkGray - insignificant info
    * Cyan - Header or important tip
    * Yellow - warning
    * DarkGreen - success
    * Blue - normal SCPSL log
*/

    public static class ConsoleUtil
    {
        private static readonly object _lck = new object();

        public static void Clear()
        {
            lock (_lck)
                Console.Clear();
        }

        public static void Write(string s, ConsoleColor c = ConsoleColor.White, string? time = null)
        {
            time ??= DateTime.Now.ToString(OutputManager.CONSOLE_DATE_FORMAT);
            RawWrite(s, c, false, time);
        }

        public static void WriteLine(string s, ConsoleColor c = ConsoleColor.White, string? time = null)
        {
            time ??= DateTime.Now.ToString(OutputManager.CONSOLE_DATE_FORMAT);
            RawWrite(s, c, true, time);
        }

        internal static void RawWrite(string s, ConsoleColor c, bool includeNewLine, string time)
        {
            lock (_lck)
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (includeNewLine)
                        Console.WriteLine();
                    else
                        Console.Write(string.Empty);

                    return;
                }

                Console.ForegroundColor = c;

                Console.Write("[");
                Console.Write(time);
                Console.Write("]");
#if LINUX_SIGNALS
                Console.Write(" ");
#else
                Console.CursorLeft++;
#endif
                Console.Write(s);
                if (includeNewLine)
                    Console.WriteLine();

                Console.ResetColor();
            }
        }
    }
}
