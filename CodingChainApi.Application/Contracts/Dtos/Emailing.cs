using Application.Read.Functions;
using Application.Read.Participations;
using Application.Read.Steps;
using Application.Read.Teams;
using Application.Read.Tournaments;

namespace Application.Contracts.Dtos
{
    public record SuspectFunctionContent(FunctionNavigation Function, TournamentNavigation Tournament,
        TeamNavigation Team, StepNavigation Step);
}