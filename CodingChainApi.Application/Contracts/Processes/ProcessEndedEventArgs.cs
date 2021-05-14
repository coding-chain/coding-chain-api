using System;

namespace Application.Contracts.Processes
{
    public sealed class ProcessEndedEventArgs : EventArgs
    {
        public string? Output { get; init; }
        public string? Error { get; init; }
    }
}