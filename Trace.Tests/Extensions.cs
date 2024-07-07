using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace Trace.Tests;

static class Extensions
{
    public static AndWhichConstraint<ObjectAssertions, TResult> BeOkObjectResult<TResult>(
        this ObjectAssertions assertions)
    {
        return assertions.BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<TResult>();
    }
}
