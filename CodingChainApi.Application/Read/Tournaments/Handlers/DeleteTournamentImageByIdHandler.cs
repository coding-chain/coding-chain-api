using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record DeleteTournamentImageByIdCommand
        (Guid TournamentId) : IRequest<string>;
    public class DeleteTournamentImageByIdHandler : IRequestHandler<DeleteTournamentImageByIdCommand, string>
    {
        private readonly IFileService _fileService;

        public DeleteTournamentImageByIdHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<string> Handle(DeleteTournamentImageByIdCommand request, CancellationToken cancellationToken)
        {
            await _fileService.DeleteTournamentImageById(request.TournamentId);
            return request.TournamentId.ToString();
        }
    }
}