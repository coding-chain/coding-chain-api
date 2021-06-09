using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Write.Cron.Handlers.RegisterCron
{
    public record CronRegisteredRequest(string CronName, DateTime ExecutedAt) : IRequest<Guid>;

    public class RegisterCronHandler : IRequestHandler<CronRegisteredRequest, Guid>
    {
        private readonly ICronRepository _cronRepository;

        public RegisterCronHandler(ICronRepository cronRepository)
        {
            _cronRepository = cronRepository;
        }

        public async Task<Guid> Handle(CronRegisteredRequest request, CancellationToken cancellationToken)
        {
            var cronId = await _cronRepository.NextIdAsync();
            var cron = new CronAggregate(cronId, request.CronName, request.ExecutedAt,
                new CronStatus(CronStatusEnum.Executing));
            await _cronRepository.SetAsync(cron);
            return cronId.Value;
        }
    }
}