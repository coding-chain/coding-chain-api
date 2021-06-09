using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.Cron;
using MediatR;

namespace Application.Write.Cron.Handlers.UpdateCron
{
    public record UpdateCronHandlerRequest(Guid CronId, CronStatus NewStatus) : IRequest<CronId>;

    public class UpdateCronHandler : IRequestHandler<UpdateCronHandlerRequest, CronId>
    {
        private readonly ICronRepository _cronRepository;

        public UpdateCronHandler(ICronRepository cronRepository)
        {
            _cronRepository = cronRepository;
        }

        public async Task<CronId> Handle(UpdateCronHandlerRequest request, CancellationToken cancellationToken)
        {
            CronAggregate cron = await _cronRepository.FindByIdAsync(new CronId(request.CronId)) ??
                                 throw new InvalidOperationException($"Could not update cron with Id {request.CronId}");
            cron.Status = request.NewStatus;
            cron.FinishedAt = DateTime.Now;
            return await _cronRepository.SetAsync(cron);
        }
    }
}