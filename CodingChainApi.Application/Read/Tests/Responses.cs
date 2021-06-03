using System;

namespace Application.Read.Tests
{
    public record TestNavigation(Guid Id, Guid StepId, string Name, string OutputValidator, string InputGenerator, decimal Score);
    public record PublicTestNavigation(Guid Id, Guid StepId, string Name, string InputType, string OutputType);

}