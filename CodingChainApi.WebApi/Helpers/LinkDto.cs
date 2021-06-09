using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace NeosCodingApi.Helpers
{
    public class LinkDto
    {
        public LinkDto(string? href, string rel, HttpMethod method)
        {
            Href = href ?? throw new ArgumentException($"Link href cannot be null for new {nameof(LinkDto)} instance");
            Rel = rel;
            Method = method;
        }

        public string Href { get; }
        public string Rel { get; }
        public HttpMethod Method { get; }

        public static LinkDto SelfLink(string? href)
        {
            return new(href, "self", HttpMethod.Get);
        }

        public static LinkDto AuthLink(string? href)
        {
            return new(href, "auth", HttpMethod.Get);
        }

        public static LinkDto AllLink(string? href)
        {
            return new(href, "all", HttpMethod.Get);
        }

        public static LinkDto CreateLink(string? href)
        {
            return new(href, "create", HttpMethod.Post);
        }

        public static LinkDto DeleteLink(string? href)
        {
            return new(href, "delete", HttpMethod.Delete);
        }

        public static LinkDto CurrentPage(string? href)
        {
            return new(href, "currentPage", HttpMethod.Get);
        }

        public static LinkDto NextPage(string? href)
        {
            return new(href, "nextPage", HttpMethod.Get);
        }

        public static LinkDto PreviousPage(string? href)
        {
            return new(href, "previousPage", HttpMethod.Get);
        }

        public static LinkDto UpdateLink(string? href)
        {
            return new(href, "update", HttpMethod.Put);
        }
    }
}