using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;

namespace EnumsEnhancedGeneratorTest;

internal static class TestHelper
{
    /// <summary>
    /// Builds a compilation from the given C# source with the full set of trusted platform
    /// assemblies referenced (so <c>System.FlagsAttribute</c> and enum underlying types resolve).
    /// </summary>
    public static CSharpCompilation CreateCompilation(string source, string assemblyName = "EnumsEnhancedTests")
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Where(static p => !string.IsNullOrEmpty(p))
            .Select(static p => (MetadataReference)MetadataReference.CreateFromFile(p));

        return CSharpCompilation.Create(
            assemblyName,
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Creates a driver that tracks incremental steps (required for both snapshotting and the
    /// incrementality assertions).
    /// </summary>
    public static GeneratorDriver CreateDriver()
    {
        var generator = new EnumsEnhanced.EnumsEnhanced();

        return CSharpGeneratorDriver.Create(
            generators: new[] { generator.AsSourceGenerator() },
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));
    }

    /// <summary>
    /// Runs the generator over <paramref name="source"/> and snapshots the result with Verify.
    /// </summary>
    public static Task Verify(string source)
    {
        var compilation = CreateCompilation(source);
        var driver = CreateDriver().RunGenerators(compilation);

        return Verifier.Verify(driver).UseDirectory("Snapshots");
    }

    /// <summary>
    /// Returns the run-step reasons recorded for the generator's tracked transform on the most
    /// recent run of <paramref name="driver"/>.
    /// </summary>
    public static ImmutableArray<IncrementalGeneratorRunStep> GetTrackedSteps(GeneratorDriver driver)
    {
        var result = driver.GetRunResult().Results.Single();
        return result.TrackedSteps[EnumsEnhanced.EnumsEnhanced.c_TrackingName];
    }
}
