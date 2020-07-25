using LocalAdmin.V2.Core;
using System;

namespace LocalAdmin.V2.IO.Output.Native
{
    public sealed class ConsoleOutput : IOutput
    {
        public void Start(DateTime time)
        {
            ConsoleUtil.Clear();
            Menu();
        }

        private void Menu()
        {
            ConsoleUtil.WriteLine($"SCP: Secret Laboratory - LocalAdmin v. {Program.VERSION}", ConsoleColor.Cyan);
            ConsoleUtil.WriteLine(string.Empty);
            ConsoleUtil.WriteLine("Licensed under The MIT License (use command \"license\" to get license text).", ConsoleColor.Cyan);
            ConsoleUtil.WriteLine("Copyright by KernelError and zabszk, 2019 - 2020", ConsoleColor.Cyan);
            ConsoleUtil.WriteLine(string.Empty);
            ConsoleUtil.WriteLine("Type 'help' to get list of available commands.", ConsoleColor.Cyan);
            ConsoleUtil.WriteLine(string.Empty);
        }

        public void Write()
        {
            ConsoleUtil.Write(OutputManager.Entry.Content, OutputManager.Entry.Color, OutputManager.Entry.Date);
        }

        public void WriteLine()
        {
            ConsoleUtil.WriteLine(OutputManager.Entry.Content, OutputManager.Entry.Color, OutputManager.Entry.Date);
        }

        public void Dispose()
        {
            ConsoleUtil.Clear();
        }
    }
}
