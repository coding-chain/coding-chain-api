using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class ProgrammingLanguage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
        public IList<Step> Steps { get; set; } = new List<Step>();
    }
}