using System.Runtime.CompilerServices;

namespace Snowberry.EnumsEnhanced.Generator.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Required for Verify to serialize GeneratorDriver results into snapshot files.
        VerifySourceGenerators.Initialize();
    }
}
