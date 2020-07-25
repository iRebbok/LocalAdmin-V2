using LocalAdmin.V2.Commands.Meta;
using LocalAdmin.V2.Core;

namespace LocalAdmin.V2.Commands
{
    internal sealed class RestartCommand : CommandBase
    {
        public RestartCommand() : base("restart") { }

        internal override void Execute(string[] arguments)
        {
            SessingManager.RestartSession();
        }
    }
}
