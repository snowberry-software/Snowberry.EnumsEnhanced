namespace Snowberry.EnumsEnhanced.Tests;

/// <summary>
/// Tests for enums with different underlying types (byte, sbyte, short, int, long, ulong)
/// </summary>
public class UnderlyingTypeTests
{
    [Fact]
    public void ByteEnum_MaxValue_HandledCorrectly()
    {
        var max = GappedEnum.Max; // 255

        ((byte)max).Should().Be(byte.MaxValue);
        max.ToStringFast().Should().Be(max.ToString());
        max.ToStringFast().Should().Be("Max");
    }

    [Fact]
    public void ByteEnum_NumericParsing_WorksWithinRange()
    {
        var parsed = GappedEnumEnhanced.ParseFast("255");
        parsed.Should().Be(GappedEnum.Max);

        var parsed2 = GappedEnumEnhanced.ParseFast("0");
        parsed2.Should().Be(GappedEnum.Min);
    }

    [Fact]
    public void SByteEnum_NegativeValues_WorkCorrectly()
    {
        var neg = SignedByteFlagsEnum.NegativeBit; // -128

        ((sbyte)neg).Should().Be(sbyte.MinValue);
        neg.ToStringFast().Should().Be(neg.ToString());
    }

    [Fact]
    public void SByteEnum_NumericParsing_HandlesNegative()
    {
        var parsed = SignedByteFlagsEnumEnhanced.ParseFast("-128");
        parsed.Should().Be(SignedByteFlagsEnum.NegativeBit);
    }

    [Fact]
    public void ShortEnum_NegativeValues_WorkCorrectly()
    {
        var neg = TestEnum2.Test1; // -2

        ((short)neg).Should().Be((short)-2);
        neg.ToStringFast().Should().Be(neg.ToString());
        neg.ToStringFast().Should().Be("Test1");
    }

    [Fact]
    public void ShortEnum_Parse_NegativeWithPlus()
    {
        var pos = TestEnum2Enhanced.ParseFast("+1");
        pos.Should().Be(TestEnum2.Test2);

        var neg = TestEnum2Enhanced.ParseFast("-2");
        neg.Should().Be(TestEnum2.Test1);
    }

    [Fact]
    public void IntEnum_ExtremeValues_WorkCorrectly()
    {
        var min = NegativeValuesEnum.VeryNegative;
        var max = NegativeValuesEnum.VeryPositive;

        ((int)min).Should().Be(int.MinValue);
        ((int)max).Should().Be(int.MaxValue);

        min.ToStringFast().Should().Be(min.ToString());
        max.ToStringFast().Should().Be(max.ToString());
    }

    [Fact]
    public void IntEnum_Parse_ExtremeValues()
    {
        var min = NegativeValuesEnumEnhanced.ParseFast(int.MinValue.ToString());
        min.Should().Be(NegativeValuesEnum.VeryNegative);

        var max = NegativeValuesEnumEnhanced.ParseFast(int.MaxValue.ToString());
        max.Should().Be(NegativeValuesEnum.VeryPositive);
    }

    [Fact]
    public void LongEnum_HighBits_WorkCorrectly()
    {
        var highBit = LongFlagsEnum.Bit62;

        ((long)highBit).Should().Be(1L << 62);
        highBit.ToStringFast().Should().Be(highBit.ToString());
    }

    [Fact]
    public void LongEnum_NegativeBit_WorksCorrectly()
    {
        var negBit = LongFlagsEnum.HighBit; // long.MinValue

        ((long)negBit).Should().Be(long.MinValue);
        negBit.ToStringFast().Should().Be(negBit.ToString());
    }

    [Fact]
    public void LongEnum_Parse_LargeValues()
    {
        var large = LongFlagsEnumEnhanced.ParseFast((1L << 50).ToString());
        large.Should().Be(LongFlagsEnum.Bit50);
    }

    [Fact]
    public void LongEnum_FlagCombination_WorksCorrectly()
    {
        var combo = LongFlagsEnum.Bit0 | LongFlagsEnum.Bit10 | LongFlagsEnum.Bit20;

        string? result = combo.ToStringFast();
        string? expected = combo.ToString();

        result.Should().Be(expected);
    }

    [Fact]
    public void ULongEnum_MaxValue_WorksCorrectly()
    {
        var max = UlongEnum.Max;

        ((ulong)max).Should().Be(ulong.MaxValue);
        max.ToStringFast().Should().Be(max.ToString());
    }

    [Fact]
    public void ULongEnum_Parse_VeryLargeValue()
    {
        var parsed = UlongEnumEnhanced.ParseFast(ulong.MaxValue.ToString());
        parsed.Should().Be(UlongEnum.Max);
    }

    [Fact]
    public void ByteFlags_AllBitsSet_DecomposesCorrectly()
    {
        var all = ByteFlagsEnum.All; // 0xFF

        string? result = all.ToStringFast();
        string? expected = all.ToString();

        result.Should().Be(expected);
        result.Should().Be("All");
    }

    [Fact]
    public void ByteFlags_NibbleComposites_WorkCorrectly()
    {
        var lower = ByteFlagsEnum.LowerNibble;
        var upper = ByteFlagsEnum.UpperNibble;

        lower.ToStringFast().Should().Be("LowerNibble");
        upper.ToStringFast().Should().Be("UpperNibble");
        lower.ToStringFast().Should().Be(lower.ToString());
        upper.ToStringFast().Should().Be(upper.ToString());
    }

    [Fact]
    public void AllUnderlyingTypes_IsDefined_WorksCorrectly()
    {
        GappedEnumEnhanced.IsDefinedFast(GappedEnum.Large).Should().BeTrue();
        SignedByteFlagsEnumEnhanced.IsDefinedFast(SignedByteFlagsEnum.Bit0).Should().BeTrue();
        LongFlagsEnumEnhanced.IsDefinedFast(LongFlagsEnum.Bit40).Should().BeTrue();
        UlongEnumEnhanced.IsDefinedFast(UlongEnum.Medium).Should().BeTrue();
        TestEnum2Enhanced.IsDefinedFast(TestEnum2.Test1).Should().BeTrue();

        GappedEnumEnhanced.IsDefinedFast((GappedEnum)99).Should().BeFalse();
        TestEnum2Enhanced.IsDefinedFast((TestEnum2)50).Should().BeFalse();
    }

    [Fact]
    public void AllUnderlyingTypes_GetValues_ReturnsCorrectCount()
    {
        var byteValues = GappedEnumEnhanced.GetValuesFast();
        var sbyteValues = SignedByteFlagsEnumEnhanced.GetValuesFast();
        var shortValues = TestEnum2Enhanced.GetValuesFast();
        var longValues = LongFlagsEnumEnhanced.GetValuesFast();
        var ulongValues = UlongEnumEnhanced.GetValuesFast();

        byteValues.Length.Should().Be(Enum.GetValues(typeof(GappedEnum)).Length);
        sbyteValues.Length.Should().Be(Enum.GetValues(typeof(SignedByteFlagsEnum)).Length);
        shortValues.Length.Should().Be(Enum.GetValues(typeof(TestEnum2)).Length);
        longValues.Length.Should().Be(Enum.GetValues(typeof(LongFlagsEnum)).Length);
        ulongValues.Length.Should().Be(Enum.GetValues(typeof(UlongEnum)).Length);
    }

    [Fact]
    public void AllUnderlyingTypes_RoundTrip_WorksCorrectly()
    {
        // Byte
        var byteVal = GappedEnum.Medium;
        var byteParsed = GappedEnumEnhanced.ParseFast(byteVal.ToStringFast()!);
        byteParsed.Should().Be(byteVal);

        // SByte
        var sbyteVal = SignedByteFlagsEnum.Bit3;
        var sbyteParsed = SignedByteFlagsEnumEnhanced.ParseFast(sbyteVal.ToStringFast()!);
        sbyteParsed.Should().Be(sbyteVal);

        // Short
        var shortVal = TestEnum2.Test1;
        var shortParsed = TestEnum2Enhanced.ParseFast(shortVal.ToStringFast()!);
        shortParsed.Should().Be(shortVal);

        // Long
        var longVal = LongFlagsEnum.Bit30;
        var longParsed = LongFlagsEnumEnhanced.ParseFast(longVal.ToStringFast()!);
        longParsed.Should().Be(longVal);

        // ULong
        var ulongVal = UlongEnum.Large;
        var ulongParsed = UlongEnumEnhanced.ParseFast(ulongVal.ToStringFast()!);
        ulongParsed.Should().Be(ulongVal);
    }
}