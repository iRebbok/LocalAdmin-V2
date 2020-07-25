using LocalAdmin.V2.IO;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace LocalAdmin.V2.Core
{
    public static class Program
    {
        public static readonly string VERSION = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        private static void Main(string[] args)
        {
            StartSafe(args);
        }

        private static void StartSafe(string[] args)
        {
            try
            {
                SessingManager.UpdateTitle();

                ushort port = 0;
                if (args.Length == 0)
                {
                    Console.WriteLine("You can pass port number as first startup argument.", ConsoleColor.Green);
                    Console.WriteLine(string.Empty);
                    Console.Write("Port number (default: 7777): ", ConsoleColor.Green);

                    ReadInput((input) =>
                    {
                        if (!string.IsNullOrEmpty(input))
                            return ushort.TryParse(input, out port);
                        port = 7777;
                        return true;
                    },
                    () => Console.WriteLine("Port number must be a unsigned short integer.", ConsoleColor.Red));
                }
                else
                {
                    if (!ushort.TryParse(args[0], out port))
                    {
                        Console.WriteLine("Failed - Invalid port!");

                        // No waiting here
                        // Most often with arguments launched from the console,
                        // the user will see an error
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            SessingManager.Exit((int)WindowsErrorCode.INVALID_PORT_GIVEN);
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            SessingManager.Exit((int)UnixErrorCode.INVALID_PORT_GIVEN);
                        else
                            SessingManager.Exit(1);
                    }
                }

                SessingManager.SwitchPort(port);
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                File.WriteAllText($"{DateTime.UtcNow:yyyy-MM-ddTHH-mm-ssZ}-crash.txt", ex.ToString());
            }
        }

        private static void ReadInput(Func<string, bool> checkInput, Action invalidInputAction)
        {
            var input = Console.ReadLine();

            while (!checkInput(input))
            {
                invalidInputAction();

                input = Console.ReadLine();
            }
        }
    }
}