using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetOneSuspectFunctionQuery(Guid FunctionId) : IRequest<SuspectFunctionNavigation>;

    public class GetOneSuspectFunctionHandler : IRequestHandler<GetOneSuspectFunctionQuery, SuspectFunctionNavigation>
    {
        private readonly IReadFunctionRepository _readFunctionRepository;

        public GetOneSuspectFunctionHandler(IReadFunctionRepository readFunctionRepository)
        {
            _readFunctionRepository = readFunctionRepository;
        }

        public async Task<SuspectFunctionNavigation> Handle(GetOneSuspectFunctionQuery request,
            CancellationToken cancellationToken)
        {
            var func = await _readFunctionRepository.GetLastByFunctionId(request.FunctionId);
            if (func is null)
                throw new NotFoundException(request.FunctionId.ToString(), "SuspectFunction");
            return func;
        }
    }
}