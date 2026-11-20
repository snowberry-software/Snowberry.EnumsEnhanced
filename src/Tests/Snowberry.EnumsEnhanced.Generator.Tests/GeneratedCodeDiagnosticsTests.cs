using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Snowberry.EnumsEnhanced.Generator.Tests;

/// <summary>
/// Guards that the generated code compiles without any compiler warning, even at the highest warning level with nullable
/// reference types enabled and on the oldest supported language version, so consumers using
/// <c>TreatWarningsAsErrors</c> are never broken by the generator.
/// </summary>
public class GeneratedCodeDiagnosticsTests
{
    private const string c_Source = """
        using System;

        namespace Sample
        {
            [Flags]
            public enum Colors { None = 0, Red = 1, Green = 2, Blue = 4, All = Red | Green | Blue }

            [Flags]
            public enum Permissions : byte { None = 0, Read = 1, Write = 2, Execute = 128 }

            [Flags]
            public enum BigFlags : long { None = 0, Low = 1, High = 1L << 62, Sign = long.MinValue }

            public enum Huge : ulong { Zero = 0, Max = ulong.MaxValue }

            public enum Signed : sbyte { Negative = -1, Zero = 0, Positive = 1 }

            public enum Direction { North, East, South, West, Default = North }

            internal enum Mode { Off, On }
        }

        public enum GlobalEnum { A, B }
        """;

    [Theory]
    [InlineData(LanguageVersion.Latest)]
    [InlineData(LanguageVersion.CSharp7_3)]
    public void GeneratedCode_HasNoWarnings(LanguageVersion languageVersion)
    {
        var compilation = TestHelper.CreateCompilation(c_Source)
            .WithOptions(new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: languageVersion >= LanguageVersion.CSharp8 ? NullableContextOptions.Enable : NullableContextOptions.Disable,
                warningLevel: 9999));

        var parseOptions = new CSharpParseOptions(languageVersion);
        compilation = compilation.ReplaceSyntaxTree(
            compilation.SyntaxTrees.Single(),
            CSharpSyntaxTree.ParseText(c_Source, parseOptions, cancellationToken: TestContext.Current.CancellationToken));

        CSharpGeneratorDriver.Create([new EnumsEnhanced().AsSourceGenerator()], parseOptions: parseOptions)
            .RunGeneratorsAndUpdateCompilation(compilation, out var output, out var generatorDiagnostics, TestContext.Current.CancellationToken);

        generatorDiagnostics.Should().BeEmpty();
        output.SyntaxTrees.Should().HaveCountGreaterThan(1, "the generator must have produced code to check");
        output.GetDiagnostics(TestContext.Current.CancellationToken)
            .Where(static d => d.Severity >= DiagnosticSeverity.Warning)
            .Select(static d => d.ToString())
            .Should().BeEmpty();
    }
}