using System;

namespace Application.Contracts.Processes
{
    public interface IProcessEndHandler
    {
        public event EventHandler<ProcessEndedEventArgs> ProcessEnded;
    }
}