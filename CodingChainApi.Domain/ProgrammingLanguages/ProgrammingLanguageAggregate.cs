using System;
using Domain.Contracts;

namespace Domain.ProgrammingLanguages
{
    public enum LanguageEnum
    {
        CSharp,
        Typescript
    }
    public record ProgrammingLanguageId(Guid Value): IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ProgrammingLanguage : Aggregate<ProgrammingLanguageId>
    {
        public ProgrammingLanguage(ProgrammingLanguageId id, LanguageEnum name): base(id)
        {
            Name = name;
        }

        public LanguageEnum Name { get; set; }

    }
}