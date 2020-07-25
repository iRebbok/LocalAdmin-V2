using LocalAdmin.V2.Core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LocalAdmin.V2.IO.ExitHandlers
{
    /// <summary>
    ///     Native signal processing on Windows NT systems.
    /// </summary>
    internal sealed class WindowsHandler : IExitHandler
    {
        // .Net Core sometimes crashes when the delegate isn't in a field
        private readonly HandlerRoutine Routine;

        public WindowsHandler()
        {
            Routine = OnNativeSignal;
        }

        public void Setup()
        {
            if (!SetConsoleCtrlHandler(Routine, true))
            {
                throw new Win32Exception();
            }
        }

        private bool OnNativeSignal(CtrlTypes ctrl)
        {
            SessingManager.Exit(0);
            return true;
        }

        public bool IsAvailable() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        #region Native

        [DllImport("Kernel32", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        private delegate bool HandlerRoutine(CtrlTypes ctrlType);

        private enum CtrlTypes
        {
            CTRL_C_EVENT,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}
