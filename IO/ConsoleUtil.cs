using System;

namespace LocalAdmin.V2.IO
{
    public static class ConsoleUtil
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff zzz";
        private static readonly char[] _toTrim = { '\n', '\r' };
        private static readonly object _lck = new object();

        public static void Clear()
        {
            lock (_lck)
            {
                Console.Clear();
            }
        }

        public static void Write(string content, ConsoleColor color = ConsoleColor.White, int height = 0)
        {
            RawWrite(content, color, height, false);
        }

        public static void WriteLine(string content, ConsoleColor color = ConsoleColor.White, int height = 0)
        {
            RawWrite(content, color, height, true);
        }

        private static void RawWrite(string content, ConsoleColor color, int height, bool includeNewLine)
        {
            content = content.Trim().Trim(_toTrim);
            if (string.IsNullOrEmpty(content))
                return;

            lock (_lck)
            {
                var formatedDate = $"[{DateTime.Now.ToString(DATE_FORMAT)}]";
                Console.ResetColor();
                Console.ForegroundColor = color;
                if (height > 0)
                    Console.CursorTop += height;

                Console.Write(formatedDate);
                Console.Write(" "); // space
                Console.Write(content);
                if (includeNewLine)
                    Console.WriteLine();
                Console.ResetColor();
            }
        }
    }
}