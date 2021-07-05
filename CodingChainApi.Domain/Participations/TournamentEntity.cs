using Domain.Contracts;
using Domain.Tournaments;

namespace Domain.Participations
{
    public class TournamentEntity : Entity<TournamentId>
    {
        public TournamentEntity(TournamentId id, bool isPublished) : base(id)
        {
            IsPublished = isPublished;
        }

        public bool IsPublished { get; set; }
    }
}