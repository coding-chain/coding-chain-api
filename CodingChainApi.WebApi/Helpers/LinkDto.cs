using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace NeosCodingApi.Helpers
{
    public class LinkDto
    {

        public string Href { get; private set; }
        public string Rel { get; private set; }
        public HttpMethod Method { get; private set; }

        public LinkDto(string? href, string rel, HttpMethod method)
        {
            Href = href ?? throw new ArgumentException($"Link href cannot be null for new {nameof(LinkDto)} instance");
            Rel = rel;
            Method = method;
        }

        public static LinkDto SelfLink(string? href)
        {
            return new LinkDto(href, "self", HttpMethod.Get);
        }
        public static LinkDto CreateLink(string? href)
        {
            return new LinkDto(href, "create", HttpMethod.Post);
        }
        public static LinkDto CurrentPage(string? href)
        {
            return new LinkDto(href, "currentPage", HttpMethod.Get);
        }
        public static LinkDto NextPage(string? href)
        {
            return new LinkDto(href, "nextPage", HttpMethod.Get);
        }
        public static LinkDto PreviousPage(string? href)
        {
            return new LinkDto(href, "previousPage", HttpMethod.Get);
        }
    }
}