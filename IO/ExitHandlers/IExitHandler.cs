namespace LocalAdmin.V2.IO.ExitHandlers
{
    internal interface IExitHandler
    {
        bool IsAvailable();

        void Setup();
    }
}
