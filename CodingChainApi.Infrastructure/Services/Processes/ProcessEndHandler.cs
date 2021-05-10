using System;
using Application.Contracts.Processes;
using Application.Write.Code.Contracts.Processes;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public class ProcessEndHandler<TId> : IProcessEndHandler
    {
        private string? _error;
        private string? _output;

        public ProcessEndHandler(TId id)
        {
            Id = id;
        }

        public TId Id { get; private set; }
        public event EventHandler<ProcessEndedEventArgs>? ProcessEnded;

        public string AddError(string? newError)
        {
            _error += newError;
            return _error;
        }

        public string AddOutput(string? newOutput)
        {
            _output += newOutput;
            return _output;
        }

        public void End()
        {
            ProcessEnded?.Invoke(this, new ProcessEndedEventArgs {Error = _error, Output = _output});
        }
    }
}