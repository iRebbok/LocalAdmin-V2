using LocalAdmin.V2.Core;
using System;
using System.Diagnostics;

namespace LocalAdmin.V2.IO.ExitHandlers
{
    internal sealed class ProcessHandler : IExitHandler
    {
        public void Setup()
        {
            var process = Process.GetCurrentProcess();
            process.EnableRaisingEvents = true;
            process.Exited += Exit;
        }

        private void Exit(object? sender, EventArgs e)
        {
            SessingManager.Exit(0);
        }

        public bool IsAvailable() => true;
    }
}
