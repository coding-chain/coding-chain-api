using System;
using Domain.ProgrammingLanguages;

namespace Application.Read.ProgrammingLanguages
{
    public record ProgrammingLanguageNavigation(Guid Id, LanguageEnum Name);
}