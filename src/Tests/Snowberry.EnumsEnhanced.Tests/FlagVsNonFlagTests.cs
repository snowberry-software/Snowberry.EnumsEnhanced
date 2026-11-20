namespace Snowberry.EnumsEnhanced.Tests;

/// <summary>
/// Tests comparing flag enum behavior vs non-flag enum behavior
/// </summary>
public class FlagVsNonFlagTests
{
    [Fact]
    public void FlagEnum_CombinedValues_DecomposeToFlags()
    {
        var combined = ByteFlagsEnum.Bit0 | ByteFlagsEnum.Bit1 | ByteFlagsEnum.Bit2;

        string? resultFast = combined.ToStringFast();
        string? resultDefault = combined.ToString();

        resultFast.Should().Be(resultDefault);
        resultFast.Should().Contain(", "); // Should have commas
    }

    [Fact]
    public void NonFlagEnum_CombinedValues_ReturnsNumeric()
    {
        // Sequential enum is not [Flags], so combined values return numeric
        var combined = (SequentialEnum)((int)SequentialEnum.First | (int)SequentialEnum.Second | (int)SequentialEnum.Fifth);

        string? resultFast = combined.ToStringFast();
        string? resultDefault = combined.ToString();

        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void FlagEnum_UndefinedComposite_DecomposesKnownBits()
    {
        var composite = (ByteFlagsEnum)((byte)ByteFlagsEnum.Bit0 | (byte)ByteFlagsEnum.Bit1 | 128);

        string? resultFast = composite.ToStringFast();
        string? resultDefault = composite.ToString();

        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void NonFlagEnum_DefinedValue_ReturnsName()
    {
        var value = SequentialEnum.Third;

        value.ToStringFast().Should().Be(value.ToString());
        value.ToStringFast().Should().Be("Third");
    }

    [Fact]
    public void FlagEnum_NamedComposite_ReturnsCompositeNameNotDecomposition()
    {
        // When a composite value has an explicit name, that should be used
        var composite = CompositeFlagsEnum.ReadWrite;

        string? resultFast = composite.ToStringFast();
        string? resultDefault = composite.ToString();

        resultFast.Should().Be(resultDefault);
        // Should return "ReadWrite", not "Read, Write"
        resultFast.Should().Be("ReadWrite");
    }

    [Fact]
    public void FlagEnum_UnnamedComposite_DecomposesToComponents()
    {
        // When composite has no explicit name, decompose it
        var composite = CompositeFlagsEnum.Read | CompositeFlagsEnum.Delete;

        string? resultFast = composite.ToStringFast();
        string? resultDefault = composite.ToString();

        resultFast.Should().Be(resultDefault);
        resultFast.Should().Contain(", ");
    }

    [Fact]
    public void FlagEnum_AllFlag_ReturnsAllNameIfDefined()
    {
        var all = CompositeFlagsEnum.All;

        string? resultFast = all.ToStringFast();
        string? resultDefault = all.ToString();

        resultFast.Should().Be(resultDefault);
        resultFast.Should().Be("All");
    }

    [Fact]
    public void FlagEnum_ZeroValue_ReturnsNoneName()
    {
        var none = ByteFlagsEnum.None;

        none.ToStringFast().Should().Be(none.ToString());
        none.ToStringFast().Should().Be("None");
    }

    [Fact]
    public void NonFlagEnum_ZeroValue_ReturnsDefinedName()
    {
        var none = SequentialEnum.None;

        none.ToStringFast().Should().Be(none.ToString());
        none.ToStringFast().Should().Be("None");
    }

    [Fact]
    public void FlagEnum_Parse_SupportsMultipleNames()
    {
        var parsed = CompositeFlagsEnumEnhanced.ParseFast("Read, Write, Execute");
        var expected = CompositeFlagsEnum.Read | CompositeFlagsEnum.Write | CompositeFlagsEnum.Execute;

        parsed.Should().Be(expected);
    }

    [Fact]
    public void NonFlagEnum_Parse_DoesNotSupportMultipleNames()
    {
        var original = Enum.Parse<SequentialEnum>("First, Second");
        var actual = SequentialEnumEnhanced.ParseFast("First, Second");

        actual.Should().Be(original);
    }

    [Fact]
    public void FlagEnum_HasFlag_WorksCorrectly()
    {
        var value = ByteFlagsEnum.Bit0 | ByteFlagsEnum.Bit1;

        value.HasFlagFast(ByteFlagsEnum.Bit0).Should().BeTrue();
        value.HasFlagFast(ByteFlagsEnum.Bit1).Should().BeTrue();
        value.HasFlagFast(ByteFlagsEnum.Bit2).Should().BeFalse();

        value.HasFlagFast(ByteFlagsEnum.Bit0).Should().Be(value.HasFlag(ByteFlagsEnum.Bit0));
    }

    [Fact]
    public void NonFlagEnum_HasFlag_StillWorks()
    {
        // HasFlag works on non-flag enums but behavior is unusual
        var value = SequentialEnum.Third; // 3
        var flag = SequentialEnum.First;  // 1

        value.HasFlagFast(flag).Should().Be(value.HasFlag(flag));
        // 3 & 1 == 1, so technically "has flag"
        value.HasFlagFast(flag).Should().BeTrue();
    }

    [Fact]
    public void FlagEnum_IsDefined_FalseForCombinations()
    {
        var combined = ByteFlagsEnum.Bit0 | ByteFlagsEnum.Bit1;

        ByteFlagsEnumEnhanced.IsDefinedFast(combined).Should().Be(Enum.IsDefined(typeof(ByteFlagsEnum), combined));
        ByteFlagsEnumEnhanced.IsDefinedFast(combined).Should().BeFalse();
    }

    [Fact]
    public void NonFlagEnum_IsDefined_FalseForInvalid()
    {
        var invalid = (SequentialEnum)99;

        SequentialEnumEnhanced.IsDefinedFast(invalid).Should().Be(Enum.IsDefined(typeof(SequentialEnum), invalid));
        SequentialEnumEnhanced.IsDefinedFast(invalid).Should().BeFalse();
    }

    [Fact]
    public void FlagEnum_CompositeWithExplicitName_IsDefinedTrue()
    {
        var composite = CompositeFlagsEnum.ReadWrite;

        CompositeFlagsEnumEnhanced.IsDefinedFast(composite).Should().Be(Enum.IsDefined(typeof(CompositeFlagsEnum), composite));
        CompositeFlagsEnumEnhanced.IsDefinedFast(composite).Should().BeTrue();
    }
}
