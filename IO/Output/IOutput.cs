using System;
using System.Threading.Tasks;

namespace LocalAdmin.V2.IO.Output
{
    public interface IOutput : IDisposable
    {
        void Start(DateTime time);

        void Write();

        void WriteLine();
    }
}
