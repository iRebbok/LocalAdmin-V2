using LocalAdmin.V2.Commands.Meta;
using LocalAdmin.V2.IO;
using LocalAdmin.V2.IO.ExitHandlers;
using LocalAdmin.V2.IO.Output;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LocalAdmin.V2.Core
{
    public static class SessingManager
    {
        private static readonly object _lck = new object();
        private static volatile bool _processClosing = false;
        private static readonly IExitHandler[] _exitHandlers = new IExitHandler[]
        {
            new AppDomainHandler(),
            new ProcessHandler(),
#if LINUX_SIGNALS
            new UnixHandler(),
#endif
            new WindowsHandler()
        };

        public static TcpServer? Server { get; private set; }
        public static Process? GameProcess { get; private set; }
        public static ushort ConsolePort => Server?.ConsolePort ?? 0;
        public static ushort GamePort { get; private set; }
        public static string ScpSlExecutable
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return "SCPSL.exe";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return "SCPSL.x86_64";

                Console.WriteLine("Failed - Unsupported platform!", ConsoleColor.Red);
                Exit(1);
                return string.Empty;
            }
        }

        static SessingManager()
        {
            for (byte z = 0; z < _exitHandlers.Length; z++)
                if (_exitHandlers[z].IsAvailable())
                    _exitHandlers[z].Setup();
        }

        public static void RestartSession()
        {
            TerminateGame();
            OutputManager.Dispose();

            ConsoleUtil.WriteLine("Started new session.", ConsoleColor.DarkGreen);
            ConsoleUtil.WriteLine("Trying to start server...", ConsoleColor.Gray);

            OutputManager.Start();
            SetupServer();
            RunGame();
            UpdateTitle();
        }

        public static void SwitchPort(ushort port)
        {
            GamePort = port;
            RestartSession();
        }

        private static void RunGame()
        {
            var executable = ScpSlExecutable;
            if (File.Exists(ScpSlExecutable))
            {
                OutputManager.WriteLine("Executing: " + executable, ConsoleColor.DarkGreen);

                var startInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = $"-batchmode -nographics -nodedicateddelete -port{GamePort} -console{ConsolePort}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                GameProcess = Process.Start(startInfo);
                GameProcess.OutputDataReceived += OnUnityOutputRevieved;
                GameProcess.BeginOutputReadLine();
                GameProcess.ErrorDataReceived += OnUnityErrorRevieved;
                GameProcess.BeginErrorReadLine();
            }
            else
            {
                ConsoleUtil.WriteLine("Failed - Executable file not found!", ConsoleColor.Red);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Exit((int)WindowsErrorCode.ERROR_FILE_NOT_FOUND, true);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Exit((int)UnixErrorCode.ERROR_FILE_NOT_FOUND, true);
                else
                    Exit(1);
            }
        }

        private static void SetupServer()
        {
            Server = new TcpServer();
            Server.Received += OnGameMessage;
            Server.Start();
        }

        internal static async Task SetupReader()
        {
            while (GameProcess is null)
                await Task.Delay(10).ConfigureAwait(false);

            while (true)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                //var currentLineCursor = Console.CursorTop;
                //Console.SetCursorPosition(0, Console.CursorTop - 1);
                ConsoleUtil.WriteLine($">>> {input}", ConsoleColor.DarkMagenta);
                //Console.SetCursorPosition(0, currentLineCursor);

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var split = input.Split(' ');
                var cmd = split[0];
                var command = CommandService.GetCommandByName(cmd);

                if (!(command is null))
                {
                    command.Execute(split.Skip(1).ToArray());
                }
                else if (Server!.Connected)
                {
                    if (GameProcess?.HasExited == true)
                    {
                        ConsoleUtil.WriteLine("Failed to send command - the game process was terminated...", ConsoleColor.Red);
                        break;
                    }

                    Server.Write(input);
                }
                else
                {
                    ConsoleUtil.WriteLine("Failed to send command - connection to server process hasn't been established yet.", ConsoleColor.Yellow);
                }
            }

            // If the game was terminated intentionally, then wait, otherwise no
            Exit(0, GameProcess?.HasExited == true); // After the readerTask is completed this will happen
        }

        /// <summary>
        ///     Terminates the game.
        /// </summary>
        private static void TerminateGame()
        {
            if (GameProcess?.HasExited == false)
            {
                GameProcess.CancelOutputRead();
                GameProcess.OutputDataReceived -= OnUnityOutputRevieved;
                GameProcess.CancelErrorRead();
                GameProcess.ErrorDataReceived -= OnUnityErrorRevieved;
                GameProcess.Kill();
            }
            Server?.Stop();
        }

        /// <summary>
        ///     Terminates the game and console.
        /// </summary>
        public static void Exit(int code = -1, bool waitForKey = false)
        {
            lock (_lck)
            {
                if (_processClosing)
                    return;

                _processClosing = true;
                TerminateGame(); // Forcefully terminating the process
                if (waitForKey)
                {
                    ConsoleUtil.WriteLine("Press any key to close...", ConsoleColor.DarkGray);
                    Console.Read();
                }

                Environment.Exit(code);
            }
        }

        internal static void UpdateTitle()
        {
            string DEFAULT_TITLE = "LocalAdmin v." + Program.VERSION;
            const string SPORT_TEMPLATE = " | SPort {0}";
            const string CPORT_TEMPLATE = " | CPort {0}";

            string result = DEFAULT_TITLE;
            if (GamePort != 0)
                result += string.Format(SPORT_TEMPLATE, GamePort);
            if (ConsolePort != 0)
                result += string.Format(CPORT_TEMPLATE, ConsolePort);

            Console.Title = result;
        }

        private static void OnUnityOutputRevieved(object _, DataReceivedEventArgs data)
        {
            // Although it always says that it'll not be null,
            // but when it's destroyed, we get a null message
            if (string.IsNullOrEmpty(data.Data))
                return;

            OutputManager.WriteLine($"[SCPSL] {data.Data}", ConsoleColor.DarkCyan);
        }

        private static void OnUnityErrorRevieved(object _, DataReceivedEventArgs data)
        {
            if (string.IsNullOrEmpty(data.Data))
                return;

            OutputManager.WriteLine($"[SCPSL] {data.Data}", ConsoleColor.DarkRed);
        }

        private static void OnGameMessage(string line)
        {
            if (!byte.TryParse(line.AsSpan(0, 1), NumberStyles.HexNumber, null, out var colorValue))
                colorValue = (byte)ConsoleColor.Gray;

            OutputManager.WriteLine(line[1..], (ConsoleColor)colorValue);
        }
    }
}
