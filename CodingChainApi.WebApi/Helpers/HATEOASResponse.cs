using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CodingChainApi.Helpers
{
    public static class HateoasResponseBuilder
    {
        public static HateoasPageResponse<T> FromPagedList<T>(IUrlHelper urlHelper, PagedListResume page,
            IList<T> values,
            string routeName, object? routeValues = null)
        {
            var valuesDic = routeValues == null ? new Dictionary<string, object>() : routeValues.ToDictionary<object>();
            valuesDic["page"] = page.CurrentPage;
            valuesDic["size"] = page.PageSize;

            var links = new List<LinkDto>
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

            return new HateoasPageResponse<T>(values, links, page.TotalCount);
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

    public class HateoasPageResponse<T> : HateoasResponse<IList<T>>
    {
        public HateoasPageResponse(IList<T> result, IList<LinkDto> links, long total) : base(result, links)
        {
            Total = total;
        }

        public long Total { get; set; }
    }
}