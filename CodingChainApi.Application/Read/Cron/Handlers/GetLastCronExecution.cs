using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Read.Cron.Handlers
{
    public record GetLastCronExecutionRequest(string CronCode, CronStatusEnum StatusFilter) : IRequest<DateTime?>;

    public class GetLastCronExecution : IRequestHandler<GetLastCronExecutionRequest, DateTime?>
    {
        private readonly IReadCronRepository _readCronRepository;

        public GetLastCronExecution(IReadCronRepository readCronRepository)
        {
            _readCronRepository = readCronRepository;
        }

        public async Task<DateTime?> Handle(GetLastCronExecutionRequest request,
            CancellationToken cancellationToken)
        {
            return await _readCronRepository.GetLastExecution(request.CronCode, new CronStatus(request.StatusFilter));
        }
    }
}