namespace LocalAdmin.V2.Commands.Meta
{
    internal abstract class CommandBase
    {
        public readonly string Name;

        protected CommandBase(string name) => Name = name.ToLowerInvariant();

        internal abstract void Execute(string[] arguments);
    }
}