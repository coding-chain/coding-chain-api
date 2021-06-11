using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Read.Cron.Handlers
{
    public record GetLastCronExecutionRequest(string CronCode, CronStatusEnum StatusFilter) : IRequest<DateTime?>;

    public class GetLastCronExecution : IRequestHandler<GetLastCronExecutionRequest, DateTime?>
    {
        private readonly ICronRepository _cronRepository;

        public GetLastCronExecution(ICronRepository cronRepository)
        {
            _cronRepository = cronRepository;
        }

        public async Task<DateTime?> Handle(GetLastCronExecutionRequest request,
            CancellationToken cancellationToken)
        {
            return await _cronRepository.GetCronLastExecution(request.CronCode, new CronStatus(request.StatusFilter));
        }
    }
}