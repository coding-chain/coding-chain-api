using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record SetTournamentImageByIdCommand
        (Guid TournamentId, Stream Stream, string Extension) : IRequest<string>;
    public class SetTournamentImageByIdHandler : IRequestHandler<SetTournamentImageByIdCommand, string>
    {
        private readonly IFileService _fileService;

        public SetTournamentImageByIdHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<string> Handle(SetTournamentImageByIdCommand request, CancellationToken cancellationToken)
        {
            await _fileService.SetTournamentImageById(request.TournamentId, request.Stream, request.Extension);
            return request.TournamentId.ToString();
        }
    }
}