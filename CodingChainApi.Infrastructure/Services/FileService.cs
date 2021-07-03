using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.IService;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Settings;

namespace CodingChainApi.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IAppDataSettings _appDataSettings;

        public FileService(IAppDataSettings appDataSettings)
        {
            _appDataSettings = appDataSettings;
        }

        public async Task<FileInfo?> GetTournamentImageById(Guid tournamentId)
        {
            var imagePath = Directory.GetFiles(Path.Join(_appDataSettings.BasePath, _appDataSettings.TournamentsPath),
                $"{tournamentId}.*").FirstOrDefault();
            if (imagePath is null) return null;
            return await new FileInfo(imagePath).ToTask();
        }

        public Task SetTournamentImageById(Guid tournamentId, Stream stream, string extension)
        {
            var imagePath = Path.Combine(_appDataSettings.BasePath, _appDataSettings.TournamentsPath,
                $"{tournamentId}{extension}");
            using var outputFileStream = new FileStream(imagePath, FileMode.Create);
            stream.CopyTo(outputFileStream);
            return Task.CompletedTask;
        }

        public async Task DeleteTournamentImageById(Guid tournamentId)
        {
            var image = await GetTournamentImageById(tournamentId);
            if(image is not null)
                File.Delete(image.FullName);
        }
    }
}