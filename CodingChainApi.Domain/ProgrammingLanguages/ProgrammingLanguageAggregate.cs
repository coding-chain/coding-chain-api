using System;
using Domain.Contracts;

namespace Domain.ProgrammingLanguages
{
    public record ProgrammingLanguageId(Guid Value): IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ProgrammingLanguage : Aggregate<ProgrammingLanguageId>
    {
        public ProgrammingLanguage(ProgrammingLanguageId id, string name): base(id)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
}