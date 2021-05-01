using System;
using System.Collections.Generic;
using CodingChainApi.Infrastructure.Common.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CodingChainApi.Infrastructure.Models
{
    public class ProgrammingLanguage 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}