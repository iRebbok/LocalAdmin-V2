using LocalAdmin.V2.Commands.Meta;
using LocalAdmin.V2.Core;
using LocalAdmin.V2.IO;
using System;

namespace LocalAdmin.V2.Commands
{
    internal class NewCommand : CommandBase
    {
        public NewCommand() : base("new") { }

        internal override void Execute(string[] arguments)
        {
            if (arguments.Length == 1)
            {
                if (ushort.TryParse(arguments[0], out var port))
                    SessingManager.SwitchPort(port);
                else
                    ConsoleUtil.WriteLine("Usage: new port", ConsoleColor.Yellow);
            }
            else
                ConsoleUtil.WriteLine("Usage: new port", ConsoleColor.Yellow);
        }
    }
}