using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tests.Handlers
{
    public record GetTestNavigationByIdQuery(Guid TestId) : IRequest<TestNavigation>;

    public class GetTestNavigationByIdHandler : IRequestHandler<GetTestNavigationByIdQuery, TestNavigation>
    {
        private readonly IReadTestRepository _readTestRepository;

        public GetTestNavigationByIdHandler(IReadTestRepository readTestRepository)
        {
            _readTestRepository = readTestRepository;
        }

        public async Task<TestNavigation> Handle(GetTestNavigationByIdQuery request, CancellationToken cancellationToken)
        {
            return await _readTestRepository.GetOneTestNavigationByID( request.TestId) ??
                   throw new NotFoundException(request.TestId.ToString(),
                       nameof(TestNavigation));
        }
    }
}