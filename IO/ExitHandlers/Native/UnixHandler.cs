#if LINUX_SIGNALS
using LocalAdmin.V2.Core;
using Mono.Unix;
using Mono.Unix.Native;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace LocalAdmin.V2.IO.ExitHandlers
{
    /// <summary>
    ///     Native signal processing on Unix systems.
    /// </summary>
    internal sealed class UnixHandler : IExitHandler
    {
        private static readonly UnixSignal[] Signals = {
            new UnixSignal(Signum.SIGINT),  // CTRL + C pressed
            new UnixSignal(Signum.SIGTERM), // Sending KILL
            new UnixSignal(Signum.SIGUSR1),
            new UnixSignal(Signum.SIGUSR2),
            new UnixSignal(Signum.SIGHUP)   // Terminal is closed
        };

        public void Setup()
        {
            try
            {
                new Thread(() =>
                {
                // Blocking operation with infinite expectation of any signal
                UnixSignal.WaitAny(Signals, -1);
                    SessingManager.Exit(0);
                }).Start();
            }
            catch (DllNotFoundException ex)
            {
                if (!CheckMonoException(ex)) throw;
            }
            catch (EntryPointNotFoundException ex)
            {
                if (!CheckMonoException(ex)) throw;
            }
            catch (TypeInitializationException ex)
            {
                switch (ex.InnerException)
                {
                    case DllNotFoundException dll:
                        if (!CheckMonoException(dll)) throw;
                        break;
                    case EntryPointNotFoundException dll:
                        if (!CheckMonoException(dll)) throw;
                        break;
                    default:
                        throw;
                }
            }
        }

        public bool IsAvailable() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private static bool CheckMonoException(Exception ex)
        {
            if (!ex.Message.Contains("MonoPosixHelper")) return false;
            Console.WriteLine("Native exit handling for Linux requires Mono to be installed!", ConsoleColor.Yellow);
            return true;
        }
    }
}
#endif
