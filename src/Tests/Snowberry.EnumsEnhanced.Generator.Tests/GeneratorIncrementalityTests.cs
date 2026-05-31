using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Snowberry.EnumsEnhanced.Generator.Tests;

/// <summary>
/// Regression guard for the incremental pipeline: proves the generator caches its per-enum model
/// across compilations. This only holds because the pipeline carries a value-equatable model
/// (<c>EnumToGenerate</c> / <c>EquatableArray</c>) instead of raw Roslyn symbols.
/// </summary>
public class GeneratorIncrementalityTests
{
    private const string c_Source = """
        using System;

        namespace Sample;

        [Flags]
        public enum Colors
        {
            None = 0,
            Red = 1,
            Green = 2,
            Blue = 4,
        }
        """;

    [Fact]
    public void Pipeline_Caches_When_UnrelatedSyntaxChanges()
    {
        var compilation1 = TestHelper.CreateCompilation(c_Source);

        var driver = TestHelper.CreateDriver().RunGenerators(compilation1);

        // Add an unrelated, non-enum syntax tree. The enum declaration is untouched, so the
        // tracked transform output must be reused (Cached/Unchanged) on the second run.
        var compilation2 = compilation1.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText("namespace Sample { public class Unrelated { } }"));

        driver = driver.RunGenerators(compilation2);

        var steps = TestHelper.GetTrackedSteps(driver);

        Assert.NotEmpty(steps);
        Assert.All(steps, step =>
            Assert.All(step.Outputs, output =>
                Assert.True(
                    output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
                    $"Expected Cached/Unchanged but got {output.Reason}. Incremental caching is broken.")));
    }

    [Fact]
    public void Pipeline_ProducesStableModel_AcrossIdenticalRuns()
    {
        var compilation = TestHelper.CreateCompilation(c_Source);

        var first = TestHelper.CreateDriver().RunGenerators(compilation);
        var firstResult = first.GetRunResult().Results.Single();

        // Re-run the same driver against the same compilation: outputs must be cached.
        var second = first.RunGenerators(compilation);
        var steps = TestHelper.GetTrackedSteps(second);

        Assert.Single(firstResult.GeneratedSources);
        Assert.All(steps, step =>
            Assert.All(step.Outputs, output =>
                Assert.True(
                    output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
                    $"Expected Cached/Unchanged but got {output.Reason}.")));
    }
}
