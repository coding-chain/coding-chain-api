using System;
using Domain.Contracts;

namespace Domain.ProgrammingLanguages
{
    public record LanguageId(Guid Value): IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ProgrammingLanguage : Aggregate<LanguageId>
    {
        public ProgrammingLanguage(LanguageId id, string name): base(id)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
}