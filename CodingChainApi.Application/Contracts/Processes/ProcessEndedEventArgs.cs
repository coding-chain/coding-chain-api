using System;

namespace Application.Write.Code.Contracts.Processes
{
    public sealed class ProcessEndedEventArgs : EventArgs
    {
        public string? Output { get; init; }
        public string? Error { get; init; }
    }
}