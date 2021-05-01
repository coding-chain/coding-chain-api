using System;
using System.Collections.Generic;
using CodingChainApi.Infrastructure.Common.Attributes;
using Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CodingChainApi.Infrastructure.Models
{
    public class Right
    {
        public Guid Id { get; set; }
        public RightEnum Name { get; set; }
        
        public IList<User> Users { get; set; }
    }
}