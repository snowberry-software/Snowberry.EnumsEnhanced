namespace EnumsEnhancedGeneratorTest;

/// <summary>
/// Golden-file snapshot tests for the generated source. A failing test here means the emitted
/// code changed — review the <c>*.received.cs</c> diff and, if intended, accept it as the new
/// <c>*.verified.cs</c> baseline.
/// </summary>
public class GeneratorSnapshotTests
{
    [Fact]
    public Task FlagsEnum_WithDuplicateValue()
    {
        const string source = """
            using System;

            namespace Sample;

            [Flags]
            public enum FileAccess
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadAlias = 1,
                ReadWrite = Read | Write,
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task NonFlagsSequentialEnum()
    {
        const string source = """
            namespace Sample;

            public enum Direction
            {
                North,
                East,
                South,
                West,
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task ByteFlagsEnum()
    {
        const string source = """
            using System;

            namespace Sample;

            [Flags]
            public enum Permissions : byte
            {
                None = 0,
                A = 1,
                B = 2,
                C = 4,
                All = A | B | C,
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task LongFlagsEnum_WithSignBit()
    {
        const string source = """
            using System;

            namespace Sample;

            [Flags]
            public enum BigFlags : long
            {
                None = 0,
                Low = 1,
                High = 1L << 32,
                Sign = long.MinValue,
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task UlongEnum_WithMaxValue()
    {
        const string source = """
            namespace Sample;

            public enum Huge : ulong
            {
                Zero = 0,
                Mid = 9223372036854775808,
                Max = ulong.MaxValue,
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task NestedEnum_IsSkipped()
    {
        // Nested enums are intentionally not generated; the snapshot should contain no sources.
        const string source = """
            namespace Sample;

            public class Container
            {
                public enum Inner
                {
                    A,
                    B,
                }
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task InternalEnum_GeneratesInternalClass()
    {
        const string source = """
            namespace Sample;

            internal enum Mode
            {
                Off,
                On,
            }
            """;

        return TestHelper.Verify(source);
    }
}
