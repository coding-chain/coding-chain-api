using System;
using Domain.Participations;
using Domain.Tournaments;
using Domain.Users;

namespace CodingChainApi.Domain.Tests
{
    public static class TestsHelper
    {
        public static TournamentId GetTournamentId()
        {
            return new(Guid.NewGuid());
        }
        
        public static FunctionId GetNewFunctionId()
        {
            return new(Guid.NewGuid());
        }
        public static UserId GetNewUserId()
        {
            return new(Guid.NewGuid());
        }

    }
}