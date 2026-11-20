namespace Snowberry.EnumsEnhanced.Tests;

/// <summary>
/// Comprehensive tests for non-flag enums focusing on ToString and Parse behavior
/// </summary>
public class NonFlagEnumTests
{
    [Fact]
    public void SequentialEnum_ToString_AllValues_MatchDefault()
    {
        var values = SequentialEnumEnhanced.GetValuesFast();
        foreach (var value in values)
        {
            value.ToStringFast().Should().Be(value.ToString());
        }
    }

    [Fact]
    public void SequentialEnum_Parse_AllNames_WorkCorrectly()
    {
        string[] names = SequentialEnumEnhanced.GetNamesFast();
        foreach (string name in names)
        {
            var expectedValue = (SequentialEnum)Enum.Parse(typeof(SequentialEnum), name);
            var actualValue = SequentialEnumEnhanced.ParseFast(name);
            actualValue.Should().Be(expectedValue);
        }
    }

    [Fact]
    public void SequentialEnum_ParseNumeric_WorksForAllValues()
    {
        for (int i = 0; i <= 5; i++)
        {
            var expected = (SequentialEnum)Enum.Parse(typeof(SequentialEnum), i.ToString());
            var actual = SequentialEnumEnhanced.ParseFast(i.ToString());
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void SequentialEnum_UndefinedValue_ReturnsNumeric()
    {
        var undefined = (SequentialEnum)99;
        undefined.ToStringFast().Should().Be("99");
        undefined.ToStringFast().Should().Be(undefined.ToString());
    }

    [Fact]
    public void SequentialEnum_Parse_UndefinedNumeric_CreatesValue()
    {
        var parsed = SequentialEnumEnhanced.ParseFast("99");
        parsed.Should().Be((SequentialEnum)99);
        ((int)parsed).Should().Be(99);
    }

    [Fact]
    public void SequentialEnum_Zero_ReturnsName()
    {
        var zero = SequentialEnum.None;
        zero.ToStringFast().Should().Be(nameof(SequentialEnum.None));
        zero.ToStringFast().Should().Be(zero.ToString());
    }

    [Fact]
    public void GappedEnum_AllDefinedValues_ReturnNames()
    {
        GappedEnum.Min.ToStringFast().Should().Be(nameof(GappedEnum.Min));
        GappedEnum.Small.ToStringFast().Should().Be(nameof(GappedEnum.Small));
        GappedEnum.Medium.ToStringFast().Should().Be(nameof(GappedEnum.Medium));
        GappedEnum.Large.ToStringFast().Should().Be(nameof(GappedEnum.Large));
        GappedEnum.Max.ToStringFast().Should().Be(nameof(GappedEnum.Max));
    }

    [Fact]
    public void GappedEnum_GapValues_ReturnNumeric()
    {
        int[] testCases = new[] { 1, 5, 15, 25, 75, 150, 200, 254 };

        foreach (int testValue in testCases)
        {
            var enumValue = (GappedEnum)testValue;
            enumValue.ToStringFast().Should().Be(testValue.ToString());
            enumValue.ToStringFast().Should().Be(enumValue.ToString());
        }
    }

    [Fact]
    public void GappedEnum_Parse_DefinedNames_WorkCorrectly()
    {
        GappedEnumEnhanced.ParseFast(nameof(GappedEnum.Min)).Should().Be(GappedEnum.Min);
        GappedEnumEnhanced.ParseFast(nameof(GappedEnum.Small)).Should().Be(GappedEnum.Small);
        GappedEnumEnhanced.ParseFast(nameof(GappedEnum.Medium)).Should().Be(GappedEnum.Medium);
        GappedEnumEnhanced.ParseFast(nameof(GappedEnum.Large)).Should().Be(GappedEnum.Large);
        GappedEnumEnhanced.ParseFast(nameof(GappedEnum.Max)).Should().Be(GappedEnum.Max);
    }

    [Fact]
    public void GappedEnum_Parse_NumericInGaps_WorksCorrectly()
    {
        byte[] testCases = new byte[] { 1, 5, 15, 25, 75, 150, 200, 254 };

        foreach (byte testValue in testCases)
        {
            var parsed = GappedEnumEnhanced.ParseFast(testValue.ToString());
            parsed.Should().Be((GappedEnum)testValue);
            ((byte)parsed).Should().Be(testValue);
        }
    }

    [Fact]
    public void GappedEnum_RoundTrip_GapValues_WorksCorrectly()
    {
        var gapValue = (GappedEnum)42;
        string? toString = gapValue.ToStringFast();
        var parsed = GappedEnumEnhanced.ParseFast(toString!);

        parsed.Should().Be(gapValue);
        toString.Should().Be("42");
    }

    [Fact]
    public void NegativeValuesEnum_AllValues_ToStringCorrectly()
    {
        NegativeValuesEnum.VeryNegative.ToStringFast().Should().Be(nameof(NegativeValuesEnum.VeryNegative));
        NegativeValuesEnum.Negative.ToStringFast().Should().Be(nameof(NegativeValuesEnum.Negative));
        NegativeValuesEnum.Zero.ToStringFast().Should().Be(nameof(NegativeValuesEnum.Zero));
        NegativeValuesEnum.Positive.ToStringFast().Should().Be(nameof(NegativeValuesEnum.Positive));
        NegativeValuesEnum.VeryPositive.ToStringFast().Should().Be(nameof(NegativeValuesEnum.VeryPositive));
    }

    [Fact]
    public void NegativeValuesEnum_Parse_NegativeNumbers_WorksCorrectly()
    {
        var parsed1 = NegativeValuesEnumEnhanced.ParseFast(int.MinValue.ToString());
        parsed1.Should().Be(NegativeValuesEnum.VeryNegative);

        var parsed2 = NegativeValuesEnumEnhanced.ParseFast("-100");
        parsed2.Should().Be(NegativeValuesEnum.Negative);

        var parsed3 = NegativeValuesEnumEnhanced.ParseFast("-50");
        parsed3.Should().Be((NegativeValuesEnum)(-50));
    }

    [Fact]
    public void NegativeValuesEnum_UndefinedNegative_ReturnsNumeric()
    {
        var undefined = (NegativeValuesEnum)(-999);
        undefined.ToStringFast().Should().Be("-999");
        undefined.ToStringFast().Should().Be(undefined.ToString());
    }

    [Fact]
    public void DuplicateValuesEnum_DuplicatesSameValue_ToStringReturnsFirst()
    {
        // Value 2 has two names: Second (first), SecondAlias (last)
        var value2 = (DuplicateValuesEnum)2;
        value2.ToStringFast().Should().Be(value2.ToString());
        value2.ToStringFast().Should().Be(nameof(DuplicateValuesEnum.Second));

        // Value 3 has three names: Third, ThirdAlias, ThirdAlias2 (last)
        var value3 = (DuplicateValuesEnum)3;
        value3.ToStringFast().Should().Be(value3.ToString());
        value3.ToStringFast().Should().Be(nameof(DuplicateValuesEnum.Third));
    }

    [Fact]
    public void DuplicateValuesEnum_ParseAllAliases_ReturnsSameValue()
    {
        // Parse all aliases for value 2
        var second = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.Second));
        var secondAlias = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.SecondAlias));
        secondAlias.Should().Be(second);
        ((int)second).Should().Be(2);

        // Parse all aliases for value 3
        var third = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.Third));
        var thirdAlias = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.ThirdAlias));
        var thirdAlias2 = DuplicateValuesEnumEnhanced.ParseFast(nameof(DuplicateValuesEnum.ThirdAlias2));
        thirdAlias.Should().Be(third);
        thirdAlias2.Should().Be(third);
        ((int)third).Should().Be(3);
    }

    [Fact]
    public void DuplicateValuesEnum_RoundTrip_UsesLastName()
    {
        var value2 = DuplicateValuesEnum.Second;
        string? toString = value2.ToStringFast();
        var parsed = DuplicateValuesEnumEnhanced.ParseFast(toString!);

        // ToString returns first name (Second), parsing it should give same value
        toString.Should().Be(value2.ToString());
        toString.Should().Be(nameof(DuplicateValuesEnum.Second));
        parsed.Should().Be(value2);
        ((int)parsed).Should().Be(2);
    }

    [Fact]
    public void PseudoFlagsEnum_CombinedValues_NotDecomposed()
    {
        // Even though it has power-of-2 values, it's not [Flags]
        var combined = (PseudoFlagsEnum)(1 | 2 | 4); // 7
        combined.ToStringFast().Should().Be("7");
        combined.ToStringFast().Should().Be(combined.ToString());

        var combined2 = (PseudoFlagsEnum)(8 | 16); // 24
        combined2.ToStringFast().Should().Be("24");
    }

    [Fact]
    public void PseudoFlagsEnum_DefinedPowerOfTwo_ReturnsName()
    {
        PseudoFlagsEnum.One.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.One));
        PseudoFlagsEnum.Two.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Two));
        PseudoFlagsEnum.Four.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Four));
        PseudoFlagsEnum.Eight.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Eight));
        PseudoFlagsEnum.Sixteen.ToStringFast().Should().Be(nameof(PseudoFlagsEnum.Sixteen));
    }

    [Fact]
    public void PseudoFlagsEnum_Parse_NumericCombinations_WorksCorrectly()
    {
        var parsed1 = PseudoFlagsEnumEnhanced.ParseFast("3");
        parsed1.Should().Be((PseudoFlagsEnum)3);

        var parsed2 = PseudoFlagsEnumEnhanced.ParseFast("7");
        parsed2.Should().Be((PseudoFlagsEnum)7);

        var parsed3 = PseudoFlagsEnumEnhanced.ParseFast("24");
        parsed3.Should().Be((PseudoFlagsEnum)24);
    }

    [Fact]
    public void SingleValueEnum_OnlyValue_ToString()
    {
        var value = SingleValueEnum.OnlyValue;
        value.ToStringFast().Should().Be(nameof(SingleValueEnum.OnlyValue));
        value.ToStringFast().Should().Be(value.ToString());
    }

    [Fact]
    public void SingleValueEnum_Parse_Name_Works()
    {
        var parsed = SingleValueEnumEnhanced.ParseFast(nameof(SingleValueEnum.OnlyValue));
        parsed.Should().Be(SingleValueEnum.OnlyValue);
        ((int)parsed).Should().Be(42);
    }

    [Fact]
    public void SingleValueEnum_Parse_Numeric_Works()
    {
        var parsed = SingleValueEnumEnhanced.ParseFast("42");
        parsed.Should().Be(SingleValueEnum.OnlyValue);
    }

    [Fact]
    public void SingleValueEnum_UndefinedValue_ReturnsNumeric()
    {
        var undefined = (SingleValueEnum)100;
        undefined.ToStringFast().Should().Be("100");
        undefined.ToStringFast().Should().Be(undefined.ToString());
    }

    [Fact]
    public void EmptyEnum_Zero_ToString()
    {
        var nothing = EmptyEnum.Nothing;
        nothing.ToStringFast().Should().Be(nameof(EmptyEnum.Nothing));
        nothing.ToStringFast().Should().Be(nothing.ToString());
    }

    [Fact]
    public void EmptyEnum_Parse_Name_Works()
    {
        var parsed = EmptyEnumEnhanced.ParseFast(nameof(EmptyEnum.Nothing));
        parsed.Should().Be(EmptyEnum.Nothing);
        ((int)parsed).Should().Be(0);
    }

    [Fact]
    public void EmptyEnum_Parse_Zero_Works()
    {
        var parsed = EmptyEnumEnhanced.ParseFast("0");
        parsed.Should().Be(EmptyEnum.Nothing);
    }

    [Fact]
    public void AllNonFlagEnums_ParseCaseInsensitive_WorksCorrectly()
    {
        // Sequential
        var seq = SequentialEnumEnhanced.ParseFast("third", ignoreCase: true);
        seq.Should().Be(SequentialEnum.Third);

        // Gapped
        var gap = GappedEnumEnhanced.ParseFast("MEDIUM", ignoreCase: true);
        gap.Should().Be(GappedEnum.Medium);

        // Negative
        var neg = NegativeValuesEnumEnhanced.ParseFast("positive", ignoreCase: true);
        neg.Should().Be(NegativeValuesEnum.Positive);

        // Pseudo
        var pseudo = PseudoFlagsEnumEnhanced.ParseFast("EIGHT", ignoreCase: true);
        pseudo.Should().Be(PseudoFlagsEnum.Eight);
    }

    [Fact]
    public void AllNonFlagEnums_TryParse_ValidNames_ReturnsTrue()
    {
        bool success1 = SequentialEnumEnhanced.TryParseFast(nameof(SequentialEnum.Fourth), false, out var result1);
        success1.Should().BeTrue();
        result1.Should().Be(SequentialEnum.Fourth);

        bool success2 = GappedEnumEnhanced.TryParseFast(nameof(GappedEnum.Large), false, out var result2);
        success2.Should().BeTrue();
        result2.Should().Be(GappedEnum.Large);

        bool success3 = DuplicateValuesEnumEnhanced.TryParseFast(nameof(DuplicateValuesEnum.ThirdAlias), false, out var result3);
        success3.Should().BeTrue();
        result3.Should().Be(DuplicateValuesEnum.Third);
    }

    [Fact]
    public void AllNonFlagEnums_TryParse_InvalidNames_ReturnsFalse()
    {
        bool success1 = SequentialEnumEnhanced.TryParseFast("InvalidName", false, out _);
        success1.Should().BeFalse();

        bool success2 = GappedEnumEnhanced.TryParseFast("NotAValue", false, out _);
        success2.Should().BeFalse();

        bool success3 = NegativeValuesEnumEnhanced.TryParseFast("FakeValue", false, out _);
        success3.Should().BeFalse();
    }

    [Fact]
    public void AllNonFlagEnums_TryParse_NumericValid_ReturnsTrue()
    {
        bool success1 = SequentialEnumEnhanced.TryParseFast("3", false, out var result1);
        success1.Should().BeTrue();
        result1.Should().Be(SequentialEnum.Third);

        bool success2 = GappedEnumEnhanced.TryParseFast("100", false, out var result2);
        success2.Should().BeTrue();
        result2.Should().Be(GappedEnum.Large);

        bool success3 = NegativeValuesEnumEnhanced.TryParseFast("-100", false, out var result3);
        success3.Should().BeTrue();
        result3.Should().Be(NegativeValuesEnum.Negative);
    }

    [Fact]
    public void AllNonFlagEnums_TryParse_NumericUndefined_ReturnsTrue()
    {
        bool success1 = SequentialEnumEnhanced.TryParseFast("999", false, out var result1);
        success1.Should().BeTrue();
        result1.Should().Be((SequentialEnum)999);

        bool success2 = GappedEnumEnhanced.TryParseFast("42", false, out var result2);
        success2.Should().BeTrue();
        result2.Should().Be((GappedEnum)42);
    }

    [Fact]
    public void AllNonFlagEnums_RoundTrip_AllDefinedValues()
    {
        // Sequential
        foreach (var value in SequentialEnumEnhanced.GetValuesFast())
        {
            string? toString = value.ToStringFast();
            toString.Should().NotBeNull();

            if (Enum.IsDefined(typeof(SequentialEnum), value))
            {
                var parsed = SequentialEnumEnhanced.ParseFast(toString);
                parsed.Should().Be(value);
            }
        }

        // Gapped
        foreach (var value in GappedEnumEnhanced.GetValuesFast())
        {
            string? toString = value.ToStringFast();
            toString.Should().NotBeNull();

            if (Enum.IsDefined(typeof(GappedEnum), value))
            {
                var parsed = GappedEnumEnhanced.ParseFast(toString);
                parsed.Should().Be(value);
            }
        }
    }

    [Fact]
    public void NonFlagEnum_ParseWithWhitespace_WorksCorrectly()
    {
        var parsed1 = SequentialEnumEnhanced.ParseFast(" First ");
        parsed1.Should().Be(SequentialEnum.First);

        var parsed2 = GappedEnumEnhanced.ParseFast("  Medium  ");
        parsed2.Should().Be(GappedEnum.Medium);

        var parsed3 = NegativeValuesEnumEnhanced.ParseFast("\tPositive\t");
        parsed3.Should().Be(NegativeValuesEnum.Positive);
    }

    [Fact]
    public void NonFlagEnum_ParseNumericWithWhitespace_WorksCorrectly()
    {
        var parsed1 = SequentialEnumEnhanced.ParseFast(" 2 ");
        parsed1.Should().Be(SequentialEnum.Second);

        var parsed2 = GappedEnumEnhanced.ParseFast("  50  ");
        parsed2.Should().Be(GappedEnum.Medium);
    }

    [Fact]
    public void NonFlagEnum_ParseWithPlusSign_WorksForPositive()
    {
        var parsed1 = SequentialEnumEnhanced.ParseFast("+3");
        parsed1.Should().Be(SequentialEnum.Third);

        var parsed2 = NegativeValuesEnumEnhanced.ParseFast("+100");
        parsed2.Should().Be(NegativeValuesEnum.Positive);
    }

    [Fact]
    public void NonFlagEnum_ParseInvalidFormat_ThrowsException()
    {
        FluentActions.Invoking(() => SequentialEnumEnhanced.ParseFast("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => SequentialEnumEnhanced.ParseFast("   ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => SequentialEnumEnhanced.ParseFast("Not, A, Valid, Enum")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => GappedEnumEnhanced.ParseFast("InvalidValue")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NonFlagEnum_TryParseInvalidFormat_ReturnsFalse()
    {
        SequentialEnumEnhanced.TryParseFast("", false, out _).Should().BeFalse();
        SequentialEnumEnhanced.TryParseFast("   ", false, out _).Should().BeFalse();
        GappedEnumEnhanced.TryParseFast("1.5", false, out _).Should().BeFalse();
        NegativeValuesEnumEnhanced.TryParseFast("not_a_number", false, out _).Should().BeFalse();
    }
}