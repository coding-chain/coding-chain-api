using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace NeosCodingApi.Helpers
{
    public static class HateoasResponseBuilder{
        public static HateoasResponse<IList<T>> FromPagedList<T>(IUrlHelper urlHelper, PagedListResume page, IList<T> values,
            string routeName, object? routeValues = null) 
        {
            var valuesDic = routeValues == null ? new Dictionary<string, object>() : routeValues.ToDictionary<object>();
            valuesDic["page"] = page.CurrentPage;
            valuesDic["size"] = page.PageSize;

            var links = new List<LinkDto>()
            {
                LinkDto.CurrentPage(urlHelper.Link(routeName, valuesDic))
            };
            if (page.HasPrevious)
            {
                var previousValues = valuesDic.Copy();
                previousValues["page"] = page.CurrentPage - 1;
                links.Add(LinkDto.PreviousPage(urlHelper.Link(routeName, previousValues)));
            }

            if (page.HasNext)
            {
                var nextValues = valuesDic.Copy();
                nextValues["page"] = page.CurrentPage + 1;
                links.Add(LinkDto.NextPage(urlHelper.Link(routeName, nextValues)));
            }

            return new HateoasResponse<IList<T>>(values, links);
        }
    }
    public class HateoasResponse<TResult>
    {
        public HateoasResponse(TResult result, IList<LinkDto> links)
        {
            Result = result;
            Links = links;
        }

        public TResult Result { get; set; }
        public IList<LinkDto> Links { get; set; }
        
    }
}