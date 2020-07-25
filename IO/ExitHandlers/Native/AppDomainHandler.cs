using LocalAdmin.V2.Core;
using LocalAdmin.V2.IO.Output;
using System;

namespace LocalAdmin.V2.IO.ExitHandlers
{
    internal sealed class AppDomainHandler : IExitHandler
    {
        public void Setup()
        {
            AppDomain.CurrentDomain.ProcessExit += Exit;
            AppDomain.CurrentDomain.DomainUnload += DomandUnload;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        private void Exit(object? sender, EventArgs e)
        {
            SessingManager.Exit(0);
        }

        private void DomandUnload(object? sender, EventArgs e)
        {
            SessingManager.Exit(0);
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                if (e.ExceptionObject is Exception ex)
                {
                    OutputManager.WriteLine($"Unhandled Exception: {ex}", ConsoleColor.Red);
                    SessingManager.Exit(1);
                }
                else
                {
                    OutputManager.WriteLine("Unhandled Exception!", ConsoleColor.Red);
                    SessingManager.Exit(1);
                }
            }
        }

        public bool IsAvailable() => true;
    }
}
