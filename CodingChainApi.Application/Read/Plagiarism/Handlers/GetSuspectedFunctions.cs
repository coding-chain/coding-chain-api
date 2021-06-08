using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Contracts.Dtos;
using Application.Read.Contracts;
using Application.Read.Functions;
using Application.Read.Participations.Handlers;
using Application.Write.Contracts;
using Domain.CodeAnalysis;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetSuspectedFunctionRequest(DateTime DateFilter) : IRequest<IList<Function>>;

    public class GetSuspectedFunctions : IRequestHandler<GetSuspectedFunctionRequest, IList<Function>>
    {
        private readonly IReadFunctionRepository _readFunctionRepository;


        public GetSuspectedFunctions(IReadFunctionRepository readFunctionRepository)
        {
            _readFunctionRepository = readFunctionRepository;
        }

        public async Task<IList<Function>> Handle(GetSuspectedFunctionRequest request,
            CancellationToken cancellationToken)
        {
            return await _readFunctionRepository.GetAllFunctionFilterOnModifiedDate(request.DateFilter);
        }
    }
}