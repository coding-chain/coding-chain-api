using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingChainApi.Infrastructure.Models
{
    public class Step
    {
        public IList<TournamentStep> TournamentSteps = new List<TournamentStep>();
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string? HeaderCode { get; set; } = "";
        public int? MinFunctionsCount { get; set; }
        public int? MaxFunctionsCount { get; set; }
        public decimal Score { get; set; }
        public bool IsOptional { get; set; }
        public int Difficulty { get; set; }
        public bool IsDeleted { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; }
        public IList<Test> Tests { get; set; } = new List<Test>();
        public IList<Participation> Participations { get; set; } = new List<Participation>();

        public bool IsPublished => TournamentSteps.Any(tS => !tS.Tournament.IsDeleted && tS.Tournament.IsPublished);
        public IList<Guid> TestsIds => Tests.Where(t => !t.IsDeleted).Select(t => t.Id).ToList();

        public IList<Guid> TournamentsIds =>
            TournamentSteps.Where(tS => !tS.Tournament.IsDeleted).Select(t => t.TournamentId).ToList();

        public IList<Guid> ActiveParticipationsIds =>
            Participations.Where(p => !p.Team.IsDeleted).Select(p => p.Id).ToList();
    }
}