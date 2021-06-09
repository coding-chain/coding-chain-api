using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Write.Cron.Handlers.UpdateCron
{
    public record UpdateCronHandlerRequest(Guid CronId, CronStatusEnum NewStatus) : IRequest<CronId>;

    public class UpdateCronHandler : IRequestHandler<UpdateCronHandlerRequest, CronId>
    {
        private readonly ICronRepository _cronRepository;
        private readonly ITimeService _timeService;

        public UpdateCronHandler(ICronRepository cronRepository, ITimeService timeService)
        {
            _cronRepository = cronRepository;
            _timeService = timeService;
        }

        public async Task<CronId> Handle(UpdateCronHandlerRequest request, CancellationToken cancellationToken)
        {
            CronAggregate cron = await _cronRepository.FindByIdAsync(new CronId(request.CronId)) ??
                                 throw new InvalidOperationException($"Could not update cron with Id {request.CronId}");
            cron.FinishCron(new CronStatus(request.NewStatus), _timeService.Now());
            return await _cronRepository.SetAsync(cron);
        }
    }
}