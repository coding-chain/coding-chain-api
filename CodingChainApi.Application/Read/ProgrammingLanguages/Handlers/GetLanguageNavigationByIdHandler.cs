using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Domain.ProgrammingLanguages;
using MediatR;

namespace Application.Read.Languages.Handlers
{
    public record GetLanguageNavigationByIdQuery(Guid LanguageId) : IRequest<ProgrammingLanguageNavigation>;
    public class GetLanguageNavigationByIdHandler: IRequestHandler<GetLanguageNavigationByIdQuery, ProgrammingLanguageNavigation>
    {
        private readonly IReadProgrammingLanguageRepository _readProgrammingLanguageRepository;

        public GetLanguageNavigationByIdHandler(IReadProgrammingLanguageRepository readProgrammingLanguageRepository)
        {
            _readProgrammingLanguageRepository = readProgrammingLanguageRepository;
        }

        public async Task<ProgrammingLanguageNavigation> Handle(GetLanguageNavigationByIdQuery request, CancellationToken cancellationToken)
        {
            var language = await _readProgrammingLanguageRepository.GetOneLanguageNavigationByIdAsync(request.LanguageId);
            if (language is null) throw new ApplicationException($"Language with id {request.LanguageId} doesn't exist");
            return language;
        }
    }
}