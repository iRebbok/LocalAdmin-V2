using System;
using System.IO;
using System.Text;

namespace LocalAdmin.V2.IO.Output.Native
{
    public sealed class LogOutput : IOutput
    {
        private StreamWriter? _writter;
        private const string LOG_FOLDER = "la_logs";
        private const string LOG_EXTENSION = "log";
        public const string LOG_DATE_FORMAT = "yyyy-MM-ddTHH-mm-ssZ";
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false, true);

        public void Start(DateTime time)
        {
            if (!Directory.Exists(LOG_FOLDER))
                Directory.CreateDirectory(LOG_FOLDER);

            var logPath = Path.GetFullPath(Path.ChangeExtension(Path.Combine(LOG_FOLDER, time.ToString(LOG_DATE_FORMAT)), LOG_EXTENSION));
            _writter = new StreamWriter(new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read), encoding)
            {
                AutoFlush = true
            };
        }

        public void Write()
        {
            var entry = OutputManager.Entry;
            _writter!.Write("[");
            _writter!.Write(entry.Date);
            _writter!.Write("] ");
            _writter.WriteLine(entry.Content);
        }

        public void WriteLine()
        {
            Write();
        }

        public void Dispose()
        {
            _writter?.Dispose();
        }
    }
}
