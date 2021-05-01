using System;
using System.Linq;
using AutoMapper;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class AutoMapperExtensions
    {
        public static Type? GetCorrespondingMappedType(this IMapper mapper, Type searchedType)
        {
            return mapper.ConfigurationProvider
                .GetAllTypeMaps()
                .Select(t =>
                {
                    if (t.SourceType == searchedType) return t.DestinationType;
                    return t.DestinationType == searchedType ? t.SourceType : null;
                }).FirstOrDefault(t => t != null);
        }
    }
}