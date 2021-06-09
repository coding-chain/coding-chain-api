using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Write.Cron.Handlers.RegisterCron
{
    public record CronRegisteredRequest(string CronName) : IRequest<Guid>;

    public class RegisterCronHandler : IRequestHandler<CronRegisteredRequest, Guid>
    {
        private readonly ICronRepository _cronRepository;
        private readonly ITimeService _timeService;

        public RegisterCronHandler(ICronRepository cronRepository, ITimeService timeService)
        {
            _cronRepository = cronRepository;
            _timeService = timeService;
        }

        public async Task<Guid> Handle(CronRegisteredRequest request, CancellationToken cancellationToken)
        {
            var cronId = await _cronRepository.NextIdAsync();
            var cron = CronAggregate.CreateNew(cronId, request.CronName, _timeService.Now(),
                new CronStatus(CronStatusEnum.Executing), null);
            await _cronRepository.SetAsync(cron);
            return cronId.Value;
        }
    }
}