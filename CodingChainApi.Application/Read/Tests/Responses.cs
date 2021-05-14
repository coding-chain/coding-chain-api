using System;

namespace Application.Read.Tests
{
    public record TestNavigation(Guid Id, Guid StepId, string OutputValidator, string InputGenerator, decimal Score);

}