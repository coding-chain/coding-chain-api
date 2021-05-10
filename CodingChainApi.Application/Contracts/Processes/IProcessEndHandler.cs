using System;
using Application.Write.Code.Contracts.Processes;

namespace Application.Contracts.Processes
{
    public interface IProcessEndHandler
    {
        public event EventHandler<ProcessEndedEventArgs> ProcessEnded;
    }
}
