using System;
using Application.Contracts.IService;

namespace CodingChainApi.Infrastructure.Services
{
    public class TimeService : ITimeService

    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}