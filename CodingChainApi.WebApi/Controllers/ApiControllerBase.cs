using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Services;

namespace NeosCodingApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/" + TemplateControllerName)]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected const string TemplateActionName = "[action]";
        protected const string TemplateControllerName = "[controller]";
        protected readonly IMapper Mapper;
        protected readonly ISender Mediator;
        protected readonly IPropertyCheckerService PropertyCheckerService;

        protected ApiControllerBase(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService)
        {
            Mediator = mediator;
            Mapper = mapper;
            PropertyCheckerService = propertyCheckerService;
        }

    }
}