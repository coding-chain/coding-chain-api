using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Users;
using Application.Write.ParticipationsSessions;
using CodingChainApi.Infrastructure.Models;
using Domain.Participations;
using Domain.ParticipationStates;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Hubs
{
    public record ConnectedUserAddedEvent(Guid UserId);
    public record ConnectedUserRemovedEvent(Guid UserId);
    public record UpdatedConnectedUserEvent(Guid UserId);
    public record ParticipationFunctionAddedEvent(Guid FunctionId);
    public record ParticipationFunctionUpdatedEvent(Guid FunctionId);
    public record ParticipationFunctionsReorderedEvent(IList<Guid> FunctionsIds);
    public record ParticipationFunctionRemovedEvent(Guid FunctionId);
    public record ParticipationProcessEndEvent(Guid ParticipationId);
    public record ParticipationProcessStartEvent(Guid ParticipationId);
    public record ParticipationReadyEvent(Guid ParticipationId);

    public interface IParticipationsClient
    {
        Task OnConnectedUser(ConnectedUserAddedEvent? serverEvent);
        Task OnDisconnectedUser(ConnectedUserRemovedEvent serverEvent);
        Task OnUpdatedConnectedUser(UpdatedConnectedUserEvent serverEvent);
        Task OnFunctionAdded(ParticipationFunctionAddedEvent serverEvent);
        Task OnFunctionUpdated(ParticipationFunctionUpdatedEvent serverEvent);
        Task OnFunctionRemoved(ParticipationFunctionRemovedEvent serverEvent);
        Task OnFunctionsReordered(ParticipationFunctionsReorderedEvent serverEvent);
        
        Task OnProcessStart(ParticipationProcessStartEvent serverEvent);
        Task OnProcessEnd(ParticipationProcessEndEvent serverEvent);
        Task OnReady(ParticipationReadyEvent serverEvent);
    }

    public class ParticipationSessionsHub : Hub<IParticipationsClient>
    {
        public static string Route = "/api/v1/participationsessionshub";
        private readonly IMediator _mediator;

        public ParticipationSessionsHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User is null ||Context.UserIdentifier is null) return;
            var participationId = Context.User.Claims.FirstOrDefault(c => c.Type == nameof(ParticipationId))?.Value;
            if (participationId is null) return;
            var connectionCount = await _mediator.Send(new ConnectUserToParticipation(new Guid(participationId)));
            await Groups.AddToGroupAsync(Context.ConnectionId, participationId);
            if (connectionCount <= 1 )
            {
                await Clients.Group(participationId).OnConnectedUser(new ConnectedUserAddedEvent(new Guid(Context.UserIdentifier)));
            }
            else
            {
                await Clients.Group(participationId).OnConnectedUser(null);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User is null ||Context.UserIdentifier is null) return;
            var participationId = Context.User.Claims.FirstOrDefault(c => c.Type == nameof(ParticipationId))?.Value;
            if (participationId is null) return;
            var connectionCount = await _mediator.Send(new DisconnectUserFromParticipationCommand(new Guid(participationId)));
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, participationId);
            if (connectionCount <= 0 )
            {
                await Clients.Group(participationId).OnDisconnectedUser(new ConnectedUserRemovedEvent(new Guid(Context.UserIdentifier)));
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}