using Domain.Contracts;
using Domain.Tournaments;

namespace Domain.Participations
{
    public class TournamentEntity : Entity<TournamentId>
    {
        public bool IsPublished { get; set; }
        public TournamentEntity(TournamentId id, bool isPublished) : base(id)
        {
            IsPublished = isPublished;
        }
    }
}