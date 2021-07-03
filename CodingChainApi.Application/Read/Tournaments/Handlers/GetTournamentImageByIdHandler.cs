using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetTournamentImageByIdQuery(Guid TournamentId) : IRequest<FileInfo>;

    public class GetTournamentImageByIdHandler : IRequestHandler<GetTournamentImageByIdQuery, FileInfo>
    {
        private readonly IFileService _fileService;

        public GetTournamentImageByIdHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<FileInfo> Handle(GetTournamentImageByIdQuery request, CancellationToken cancellationToken)
        {
            var tournamentImage = await _fileService.GetTournamentImageById(request.TournamentId);
            if (tournamentImage is null)
                throw new NotFoundException(request.TournamentId.ToString(), "TournamentImage");
            return tournamentImage;
        }
    }
}