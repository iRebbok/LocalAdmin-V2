using LocalAdmin.V2.IO.Output.Native;
using System;

namespace LocalAdmin.V2.IO.Output
{
    public readonly struct OutputEntry
    {
        public readonly string Content;
        public readonly string Date;
        public readonly ConsoleColor Color;

        public OutputEntry(string s, ConsoleColor c)
        {
            Content = s;
            Color = c;
            Date = DateTime.Now.ToString(OutputManager.CONSOLE_DATE_FORMAT);
        }
    }

    public static class OutputManager
    {
        public const string CONSOLE_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff zzz";

        private readonly static IOutput[] _outputs = new IOutput[2]
            {
                new ConsoleOutput(),
                new LogOutput()
            };

        public static OutputEntry Entry { get; private set; }

        public static void Start()
        {
            var time = DateTime.Now;
            for (byte z = 0; z < _outputs.Length; z++)
                _outputs[z].Start(time);
        }

        public static void Dispose()
        {
            for (byte z = 0; z < _outputs.Length; z++)
                _outputs[z].Dispose();
        }

        public static void Write(string s, ConsoleColor c) =>
            RawWrite(s, c, false);

        public static void WriteLine(string s, ConsoleColor c) =>
            RawWrite(s, c, true);

        private static void RawWrite(string s, ConsoleColor c, bool writeLine)
        {
            s = PrepareContent(s);
            if (string.IsNullOrEmpty(s))
                return;

            Entry = new OutputEntry(s, c);
            for (byte z = 0; z < _outputs.Length; z++)
            {
                if (writeLine)
                    _outputs[z].WriteLine();
                else
                    _outputs[z].Write();
            }
        }

        private static string PrepareContent(string s)
        {
            // Replaces CRLF with LF
            return s.Replace("\r\n", "\n");
        }
    }
}
