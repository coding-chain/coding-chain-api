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
        private readonly IReadSuspectFunctionRepository _readSuspectFunctionRepository;

        public GetOneSuspectFunctionHandler(IReadSuspectFunctionRepository readSuspectFunctionRepository)
        {
            _readSuspectFunctionRepository = readSuspectFunctionRepository;
        }

        public async Task<SuspectFunctionNavigation> Handle(GetOneSuspectFunctionQuery request,
            CancellationToken cancellationToken)
        {
            var func = await _readSuspectFunctionRepository.GetLastByFunctionId(request.FunctionId);
            if (func is null)
                throw new NotFoundException(request.FunctionId.ToString(), "SuspectFunction");
            return func;
        }
    }
}