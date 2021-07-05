using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models.Cache
{
    public class Step
    {
        public Guid Id { get; set; }
        public IList<Test> Tests { get; set; }
        public IList<Guid> TournamentIds { get; set; }
    }
}