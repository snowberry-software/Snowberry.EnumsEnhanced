namespace Snowberry.EnumsEnhanced.Tests;

/// <summary>
/// Tests for complex edge cases including duplicates, composites, and unusual patterns
/// </summary>
public class ComplexEdgeCaseTests
{
    [Fact]
    public void DuplicateValues_NonFlag_GetNameReturnsFirst()
    {
        var value = DuplicateValuesEnum.Second; // Value = 2
        var alias = DuplicateValuesEnum.SecondAlias; // Also value = 2

        // GetName should return first defined name
        value.GetNameFast(false).Should().Be(Enum.GetName(typeof(DuplicateValuesEnum), value));
        value.GetNameFast(false).Should().Be(nameof(DuplicateValuesEnum.Second));
        alias.GetNameFast(false).Should().Be(nameof(DuplicateValuesEnum.Second));
    }

    [Fact]
    public void DuplicateValues_NonFlag_ToStringReturnsFirst()
    {
        var value = DuplicateValuesEnum.Third; // Value = 3

        // ToString should return last defined name
        value.ToStringFast().Should().Be(value.ToString());
        value.ToStringFast().Should().Be(nameof(DuplicateValuesEnum.Third));
    }

    [Fact]
    public void DuplicateValues_NonFlag_AllNamesParseable()
    {
        var parsed1 = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.Second));
        var parsed2 = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.SecondAlias));

        parsed2.Should().Be(parsed1);
        parsed1.Should().Be(DuplicateValuesEnum.Second);
        ((int)parsed2).Should().Be((int)DuplicateValuesEnum.Second);
    }

    [Fact]
    public void DuplicateValues_Flags_GetNameReturnsFirst()
    {
        var value = DuplicateFlagsEnum.Flag1;
        var alias = DuplicateFlagsEnum.Flag1Alias;

        value.GetNameFast(false).Should().Be(nameof(DuplicateFlagsEnum.Flag1));
        alias.GetNameFast(false).Should().Be(nameof(DuplicateFlagsEnum.Flag1));
    }

    [Fact]
    public void DuplicateValues_Flags_CompositeWithAlias()
    {
        var combined = DuplicateFlagsEnum.Combined; // Explicit name for 3
        var alias = DuplicateFlagsEnum.CombinedAlias; // Also 3

        // Both should return the composite name (last defined)
        combined.ToStringFast().Should().Be(combined.ToString());
        alias.ToStringFast().Should().Be(alias.ToString());
        combined.ToStringFast().Should().Be(nameof(DuplicateFlagsEnum.CombinedAlias));
    }

    [Fact]
    public void SingleValue_Enum_WorksCorrectly()
    {
        var value = SingleValueEnum.OnlyValue;

        value.ToStringFast().Should().Be(value.ToString());
        value.ToStringFast().Should().Be(nameof(SingleValueEnum.OnlyValue));
        value.GetNameFast(false).Should().Be(Enum.GetName(typeof(SingleValueEnum), value));
    }

    [Fact]
    public void SingleValue_GetValues_ReturnsOne()
    {
        var values = SingleValueEnumEnhanced.GetValuesFast();
        values.Should().ContainSingle();
        values[0].Should().Be(SingleValueEnum.OnlyValue);
    }

    [Fact]
    public void EmptyEnum_OnlyZero_WorksCorrectly()
    {
        var nothing = EmptyEnum.Nothing;

        nothing.ToStringFast().Should().Be(nameof(EmptyEnum.Nothing));
        nothing.ToStringFast().Should().Be(nothing.ToString());
    }

    [Fact]
    public void PseudoFlags_NotActualFlags_NoDecomposition()
    {
        // This enum has power-of-2 values but no [Flags] attribute
        var combined = (PseudoFlagsEnum)((int)PseudoFlagsEnum.One | (int)PseudoFlagsEnum.Two);

        // Should return numeric, not decompose
        combined.ToStringFast().Should().Be(combined.ToString());
        combined.ToStringFast().Should().Be("3");
    }

    [Fact]
    public void PseudoFlags_DefinedValues_ReturnNames()
    {
        PseudoFlagsEnum.Four.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Four));
        PseudoFlagsEnum.Sixteen.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Sixteen));
        PseudoFlagsEnum.Four.ToStringFast().Should().Be(PseudoFlagsEnum.Four.ToString());
    }

    [Fact]
    public void SparseFlags_OnlyDefinedBits_Decompose()
    {
        var combo = SparseFlagsEnum.Bit0 | SparseFlagsEnum.Bit10 | SparseFlagsEnum.Bit20;

        string? result = combo.ToStringFast();
        string? expected = combo.ToString();

        result.Should().Be(expected);
        result.Should().Contain(", ");
    }

    [Fact]
    public void SparseFlags_IntermediateBits_IncludeNumeric()
    {
        // Set a bit that's not defined (e.g., Bit7)
        var withExtra = (SparseFlagsEnum)((int)SparseFlagsEnum.Bit0 | (1 << 7));

        string? result = withExtra.ToStringFast();
        string? expected = withExtra.ToString();

        result.Should().Be(expected);
    }

    [Fact]
    public void OverlappingFlags_NamedComposite_PrefersCompositeName()
    {
        var abc = OverlappingFlagsEnum.ABC;

        // Should return "ABC", not "A, B, C"
        abc.ToStringFast().Should().Be(nameof(OverlappingFlagsEnum.ABC));
        abc.ToStringFast().Should().Be(abc.ToString());
    }

    [Fact]
    public void OverlappingFlags_Parse_CanUseCompositeOrComponents()
    {
        var parsed1 = OverlappingFlagsEnumEnhanced.ParseFast(nameof(OverlappingFlagsEnum.ABC));
        var parsed2 = OverlappingFlagsEnumEnhanced.ParseFast($"{nameof(OverlappingFlagsEnum.A)}, {nameof(OverlappingFlagsEnum.B)}, {nameof(OverlappingFlagsEnum.C)}");

        parsed2.Should().Be(parsed1);
        parsed1.Should().Be(OverlappingFlagsEnum.ABC);
    }

    [Fact]
    public void OverlappingFlags_MultipleComposites_UsesCorrectName()
    {
        // Both ABC and BCD are defined
        var abc = OverlappingFlagsEnum.ABC;
        var bcd = OverlappingFlagsEnum.BCD;

        bcd.ToStringFast().Should().NotBe(abc.ToStringFast());
        abc.ToStringFast().Should().Be(nameof(OverlappingFlagsEnum.ABC));
        bcd.ToStringFast().Should().Be(nameof(OverlappingFlagsEnum.BCD));
    }

    [Fact]
    public void GappedEnum_UndefinedInGap_ReturnsNumeric()
    {
        var gapped = (GappedEnum)25; // Between Small (10) and Medium (50)

        gapped.ToStringFast().Should().Be(gapped.ToString());
        gapped.ToStringFast().Should().Be("25");
    }

    [Fact]
    public void GappedEnum_Parse_WorksForGaps()
    {
        var parsed = GappedEnumEnhanced.ParseFast("25");
        parsed.Should().Be((GappedEnum)25);
    }

    [Fact]
    public void MultipleDuplicates_TripleValue_ReturnsFirst()
    {
        // Third, ThirdAlias, and ThirdAlias2 all equal 3
        var value = DuplicateValuesEnum.Third;

        value.ToStringFast().Should().Be(nameof(DuplicateValuesEnum.Third));
        value.ToStringFast().Should().Be(value.ToString());
    }

    [Fact]
    public void MultipleDuplicates_AllNamesParse()
    {
        string[] names = new[]
        {
            nameof(DuplicateValuesEnum.Third),
            nameof(DuplicateValuesEnum.ThirdAlias),
            nameof(DuplicateValuesEnum.ThirdAlias2)
        };

        foreach (string? name in names)
        {
            var parsed = DuplicateValuesEnumEnhanced.ParseFast(name);
            ((int)parsed).Should().Be(3);
        }
    }

    [Fact]
    public void GetNames_IncludesAllDuplicates()
    {
        string[] names = DuplicateValuesEnumEnhanced.GetNamesFast();

        names.Should().Contain(nameof(DuplicateValuesEnum.Second));
        names.Should().Contain(nameof(DuplicateValuesEnum.SecondAlias));
        names.Should().Contain(nameof(DuplicateValuesEnum.Third));
        names.Should().Contain(nameof(DuplicateValuesEnum.ThirdAlias));
        names.Should().Contain(nameof(DuplicateValuesEnum.ThirdAlias2));
    }

    [Fact]
    public void GetValues_IncludesAllDuplicates()
    {
        var values = DuplicateValuesEnumEnhanced.GetValuesFast();

        // Should have 6 entries even though some have duplicate values
        values.Length.Should().Be(6);
    }

    [Fact]
    public void IsDefined_Name_TrueForAllDuplicates()
    {
        DuplicateValuesEnumEnhanced.IsDefinedFast(nameof(DuplicateValuesEnum.Second)).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast(nameof(DuplicateValuesEnum.SecondAlias)).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast(nameof(DuplicateValuesEnum.Third)).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast(nameof(DuplicateValuesEnum.ThirdAlias)).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast(nameof(DuplicateValuesEnum.ThirdAlias2)).Should().BeTrue();
    }

    [Fact]
    public void IsDefined_Value_TrueForDefinedValues()
    {
        // Even though value 2 has two names, IsDefined(2) is true
        DuplicateValuesEnumEnhanced.IsDefinedFast((DuplicateValuesEnum)2).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast((DuplicateValuesEnum)3).Should().BeTrue();
        DuplicateValuesEnumEnhanced.IsDefinedFast((DuplicateValuesEnum)99).Should().BeFalse();
    }

    [Fact]
    public void CompositeFlags_HasFlag_WorksForComponents()
    {
        var composite = CompositeFlagsEnum.ReadWriteExecute;

        composite.HasFlagFast(CompositeFlagsEnum.Read).Should().BeTrue();
        composite.HasFlagFast(CompositeFlagsEnum.Write).Should().BeTrue();
        composite.HasFlagFast(CompositeFlagsEnum.Execute).Should().BeTrue();
        composite.HasFlagFast(CompositeFlagsEnum.Delete).Should().BeFalse();

        // Also works for other composites
        composite.HasFlagFast(CompositeFlagsEnum.ReadWrite).Should().BeTrue();
        composite.HasFlagFast(CompositeFlagsEnum.ReadExecute).Should().BeTrue();
    }

    [Fact]
    public void ZeroValueInFlags_NeverAppearsInDecomposition()
    {
        var combo = ByteFlagsEnum.None | ByteFlagsEnum.Bit1 | ByteFlagsEnum.Bit2;
        string? result = combo.ToStringFast();

        result.Should().NotContain(nameof(ByteFlagsEnum.None));
        result.Should().Contain(nameof(ByteFlagsEnum.Bit1));
        result.Should().Contain(nameof(ByteFlagsEnum.Bit2));
    }

    [Fact]
    public void AllEnums_CaseInsensitiveParsing_Works()
    {
        var sequential = SequentialEnumEnhanced.ParseFast("FIRST", ignoreCase: true);
        sequential.Should().Be(SequentialEnum.First);

        var gapped = GappedEnumEnhanced.ParseFast("large", ignoreCase: true);
        gapped.Should().Be(GappedEnum.Large);

        var flags = ByteFlagsEnumEnhanced.ParseFast("bit0, BIT1", ignoreCase: true);
        flags.Should().Be(ByteFlagsEnum.Bit0 | ByteFlagsEnum.Bit1);
    }

    [Fact]
    public void AllEnums_TryParse_MatchesEnumTryParse()
    {
        // Sequential
        bool success1 = SequentialEnumEnhanced.TryParseFast(nameof(SequentialEnum.Third), false, out var parsed1);
        bool expected1 = Enum.TryParse(typeof(SequentialEnum), nameof(SequentialEnum.Third), false, out object? exp1);
        success1.Should().Be(expected1);
        ((object)parsed1).Should().Be(exp1);

        // Gapped
        bool success2 = GappedEnumEnhanced.TryParseFast("50", false, out var parsed2);
        bool expected2 = Enum.TryParse(typeof(GappedEnum), "50", false, out object? exp2);
        success2.Should().Be(expected2);
        ((object)parsed2).Should().Be(exp2);

        // Flags
        bool success3 = ByteFlagsEnumEnhanced.TryParseFast("InvalidName", false, out var parsed3);
        bool expected3 = Enum.TryParse(typeof(ByteFlagsEnum), "InvalidName", false, out object? exp3);
        success3.Should().Be(expected3);
    }
}