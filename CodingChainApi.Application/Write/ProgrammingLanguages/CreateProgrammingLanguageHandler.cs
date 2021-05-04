using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;
using MediatR;

namespace Application.Write.Languages
{
    public record CreateLanguageCommand(string Name): IRequest<string>;
    
    public class CreateLanguageHandler: IRequestHandler<CreateLanguageCommand, string>
    {
        private readonly IProgrammingLanguageRepository _repository;
        private readonly IReadProgrammingLanguageRepository _readProgrammingLanguageRepository;

        public CreateLanguageHandler(IProgrammingLanguageRepository repository, IReadProgrammingLanguageRepository readProgrammingLanguageRepository)
        {
            _repository = repository;
            _readProgrammingLanguageRepository = readProgrammingLanguageRepository;
        }

        public async Task<string> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var languageExists = await _readProgrammingLanguageRepository.LanguageExistsByName(request.Name);

            if (languageExists ) throw new ApplicationException($"Language {request.Name} already exist");

            var language = new ProgrammingLanguage(await _repository.NextIdAsync(), request.Name);

            var id =  await _repository.SetAsync(language);
            if (id is null) throw new ApplicationException($"Language {request.Name} cannot be created");
            return id.ToString();
        }
    }
}