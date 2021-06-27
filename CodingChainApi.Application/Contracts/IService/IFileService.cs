using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Contracts.IService
{
    public interface IFileService
    {
        public Task<FileInfo?> GetTournamentImageById(Guid tournamentId);
        public Task SetTournamentImageById(Guid tournamentId, Stream stream, string extension);
        public Task DeleteTournamentImageById(Guid tournamentId);
    }
}