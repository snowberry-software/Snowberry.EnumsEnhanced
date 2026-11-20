using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Snowberry.EnumsEnhanced.Generator.Tests;

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
        var generator = new Snowberry.EnumsEnhanced.EnumsEnhanced();

        return CSharpGeneratorDriver.Create(
            generators: new[] { generator.AsSourceGenerator() },
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));
    }

    /// <summary>
    /// Runs the generator over <paramref name="source"/> and compares every generated file with its approved
    /// snapshot in <c>Snapshots/{TestClass}.{TestMethod}#{HintName}.verified.cs</c>.
    /// </summary>
    /// <remarks>
    /// On a mismatch the actual output is written next to the snapshot as <c>.received.cs</c> (git-ignored) so it can
    /// be diffed. Set the <c>UPDATE_SNAPSHOTS</c> environment variable to <c>1</c> to accept the current output instead.
    /// </remarks>
    /// <param name="source">The C# source to run the generator over.</param>
    /// <param name="callerFilePath">Supplied by the compiler; locates the <c>Snapshots</c> folder and names the test class.</param>
    /// <param name="testName">Supplied by the compiler; the calling test method's name.</param>
    public static void AssertMatchesSnapshot(
        string source,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string testName = "")
    {
        var compilation = CreateCompilation(source);
        var runResult = CreateDriver()
            .RunGenerators(compilation, TestContext.Current.CancellationToken)
            .GetRunResult();

        runResult.Diagnostics.Should().BeEmpty("the generator must not report diagnostics for the snapshot inputs");

        var snapshotDirectory = Path.Combine(Path.GetDirectoryName(callerFilePath)!, "Snapshots");
        var prefix = $"{Path.GetFileNameWithoutExtension(callerFilePath)}.{testName}#";
        var updateSnapshots = Environment.GetEnvironmentVariable("UPDATE_SNAPSHOTS") == "1";

        var expectedFiles = new HashSet<string>(
            Directory.EnumerateFiles(snapshotDirectory, prefix + "*.verified.cs"),
            StringComparer.OrdinalIgnoreCase);

        foreach (var generated in runResult.Results.Single().GeneratedSources)
        {
            var verifiedPath = Path.Combine(snapshotDirectory, $"{prefix}{Path.GetFileNameWithoutExtension(generated.HintName)}.verified.cs");
            var receivedPath = Path.ChangeExtension(Path.ChangeExtension(verifiedPath, null), ".received.cs");
            var actual = NormalizeSnapshot($"//HintName: {generated.HintName}\n{generated.SourceText}");

            expectedFiles.Remove(verifiedPath);

            if (updateSnapshots)
            {
                File.WriteAllText(verifiedPath, actual, s_SnapshotEncoding);
                File.Delete(receivedPath);
                continue;
            }

            var expected = File.Exists(verifiedPath) ? NormalizeSnapshot(File.ReadAllText(verifiedPath)) : null;

            if (actual == expected)
            {
                File.Delete(receivedPath);
                continue;
            }

            File.WriteAllText(receivedPath, actual, s_SnapshotEncoding);
            actual.Should().Be(
                expected,
                "the generated {0} must match the approved snapshot {1} (actual output written to {2}; set UPDATE_SNAPSHOTS=1 to accept it)",
                generated.HintName,
                Path.GetFileName(verifiedPath),
                Path.GetFileName(receivedPath));
        }

        if (updateSnapshots)
        {
            foreach (var stale in expectedFiles)
                File.Delete(stale);

            return;
        }

        expectedFiles.Should().BeEmpty("every approved snapshot of {0} must still be generated", testName);
    }

    /// <summary>UTF-8 with BOM, matching the existing snapshot files.</summary>
    private static readonly Encoding s_SnapshotEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    /// <summary>
    /// Normalizes line endings to LF and trims trailing whitespace so snapshots compare equal across platforms and
    /// git checkouts.
    /// </summary>
    /// <param name="text">The snapshot text.</param>
    /// <returns>The normalized text.</returns>
    private static string NormalizeSnapshot(string text)
        => text.Replace("\r\n", "\n").TrimEnd();

    /// <summary>
    /// Returns the run-step reasons recorded for the generator's tracked transform on the most
    /// recent run of <paramref name="driver"/>.
    /// </summary>
    public static ImmutableArray<IncrementalGeneratorRunStep> GetTrackedSteps(GeneratorDriver driver)
    {
        var result = driver.GetRunResult().Results.Single();
        return result.TrackedSteps[Snowberry.EnumsEnhanced.EnumsEnhanced.c_TrackingName];
    }
}
