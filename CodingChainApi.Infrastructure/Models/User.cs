using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CodingChainApi.Infrastructure.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public IList<Right> Rights { get; set; } = new List<Right>();

    }
}