using System;

namespace LocalAdmin.V2.Commands.Meta
{
    internal static class CommandService
    {
        private static readonly CommandBase[] _commands;

        static CommandService()
        {
            _commands = new CommandBase[]
            {
                new HelpCommand(),
                new LicenseCommand(),
                new NewCommand(),
                new RestartCommand()
            };
        }

        public static CommandBase? GetCommandByName(string name)
        {
            for (int z = 0; z < _commands.Length; z++)
            {
                var cmd = _commands[z];
                if (cmd.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return cmd;
            }

            return null;
        }
    }
}