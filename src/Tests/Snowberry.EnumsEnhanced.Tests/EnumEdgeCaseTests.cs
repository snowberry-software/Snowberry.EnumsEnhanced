namespace Snowberry.EnumsEnhanced.Tests;

/// <summary>
/// Comprehensive edge case tests for EnumsEnhanced functionality
/// </summary>
public class EnumEdgeCaseTests
{
    [Fact]
    public void ZeroValue_ExcludedFromFlagDecomposition()
    {
        // When combining flags, zero should never appear in the output
        var combo = TestEnum.Test0 | TestEnum.Test1 | TestEnum.Test2;
        string? resultFast = combo.ToStringFast();
        string? resultDefault = combo.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
        resultFast.Should().NotContain("Test0");
    }

    [Fact]
    public void ZeroValueAlone_ReturnsZeroName()
    {
        var zero = TestEnum.Test0;
        zero.ToStringFast().Should().Be(zero.ToString());
        zero.ToStringFast().Should().Be("Test0");
        zero.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum), zero));
    }

    [Fact]
    public void DuplicateValues_FirstInGetName_LastInToString()
    {
        // Test1 = 1, Test_1 = 1 (duplicate)
        var value = TestEnum.Test1;

        // GetNameFast should return first (matches Enum.GetName)
        value.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum), value));
        value.GetNameFast(false).Should().Be("Test1");
        TestEnum.Test_1.GetNameFast(false).Should().Be("Test1");

        // ToStringFast should return last (matches ToString)
        value.ToStringFast().Should().Be(value.ToString());
        value.ToStringFast().Should().Be("Test_1");
        TestEnum.Test_1.ToStringFast().Should().Be("Test_1");
    }

    [Fact]
    public void DuplicateValues_InFlagCombination_UsesFirst()
    {
        // When Test1/Test_1 is part of a flag combination, GetNameFast uses first
        var combo = TestEnum.Test1 | TestEnum.Test2;
        string? result = combo.GetNameFast(true);

        result.Should().NotBeNull();
        result.Should().Contain("Test1");
        result.Should().NotContain("Test_1");
    }

    [Fact]
    public void DuplicateValues_InFlagCombinationToString_MatchesDefault()
    {
        // When Test1/Test_1 is part of a flag combination, compare with default
        var combo = TestEnum.Test1 | TestEnum.Test2;
        string? resultFast = combo.ToStringFast();
        string? resultDefault = combo.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void AllBitsSet_DecomposesCorrectly()
    {
        // Set all possible bits
        var allBits = (TestEnum)int.MaxValue;
        string? resultFast = allBits.ToStringFast();
        string? resultDefault = allBits.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
        // Should contain multiple flags or numeric value
        resultFast.Should().NotBeEmpty();
    }

    [Fact]
    public void NegativeValue_TestEnum2_Behavior()
    {
        var negValue = TestEnum2.Test1; // -2

        ((short)negValue).ToString().Should().Be("-2");
        negValue.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum2), negValue));
        negValue.GetNameFast(false).Should().Be("Test1");
        negValue.ToStringFast().Should().Be(negValue.ToString());
        negValue.ToStringFast().Should().Be("Test1");
    }

    [Fact]
    public void UndefinedValue_ReturnsNumeric()
    {
        var undefined = (TestEnum)12345;

        string? getName = undefined.GetNameFast(false);
        string? getNameFlags = undefined.GetNameFast(true);
        string? toString = undefined.ToStringFast();
        string? toStringDefault = undefined.ToString();

        getName.Should().Be(Enum.GetName(typeof(TestEnum), undefined));
        getName.Should().BeNull();
        getNameFlags.Should().NotBeNull();
        toString.Should().NotBeNull();
        // Should match default behavior
        toString.Should().Be(toStringDefault);
    }

    [Fact]
    public void UndefinedValueWithSomeKnownBits_DecomposesKnownPart()
    {
        // Value that has Test1 bit (1) + unknown bit (256)
        var partiallyDefined = (TestEnum)(1 | 256);
        string? resultFast = partiallyDefined.ToStringFast();
        string? resultDefault = partiallyDefined.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void MaxValue_Behavior()
    {
        var maxInt = (TestEnum)int.MaxValue;
        string? resultFast = maxInt.ToStringFast();
        string? resultDefault = maxInt.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void MinValue_Behavior()
    {
        var minInt = (TestEnum)int.MinValue;
        string? resultFast = minInt.ToStringFast();
        string? resultDefault = minInt.ToString();

        resultFast.Should().NotBeNull();
        resultFast.Should().Be(resultDefault);
    }

    [Fact]
    public void ParseFast_BothDuplicateNames_ProduceSameValue()
    {
        var parsed1 = TestEnumEnhanced.ParseFast("Test1");
        var parsed2 = TestEnumEnhanced.ParseFast("Test_1");

        parsed2.Should().Be(parsed1);
        ((int)parsed2).Should().Be((int)parsed1);
        ((int)parsed1).Should().Be(1);
    }

    [Fact]
    public void ParseFast_MixingDuplicateNamesInFlags_Works()
    {
        var result1 = TestEnumEnhanced.ParseFast("Test1, Test2");
        var result2 = TestEnumEnhanced.ParseFast("Test_1, Test2");

        result2.Should().Be(result1);
        result1.Should().Be(TestEnum.Test1 | TestEnum.Test2);
    }

    [Fact]
    public void ParseFast_SingleFlagMultipleTimes_Idempotent()
    {
        var result = TestEnumEnhanced.ParseFast("Test1, Test1, Test1");
        result.Should().Be(TestEnum.Test1);
    }

    [Fact]
    public void ParseFast_FlagsInDifferentOrder_SameResult()
    {
        var result1 = TestEnumEnhanced.ParseFast("Test1, Test2, Test4");
        var result2 = TestEnumEnhanced.ParseFast("Test4, Test2, Test1");
        var result3 = TestEnumEnhanced.ParseFast("Test2, Test1, Test4");

        result2.Should().Be(result1);
        result3.Should().Be(result2);
    }

    [Fact]
    public void ParseFast_WhitespaceVariations_AllWork()
    {
        string[] testCases = new[]
        {
            "Test1,Test2",           // No spaces
            "Test1, Test2",          // Normal spacing
            "Test1 ,Test2",          // Space before comma
            "Test1 , Test2",         // Spaces around comma
            " Test1,Test2 ",         // Leading/trailing spaces
            "\tTest1,\tTest2",       // Tabs
            "Test1  ,  Test2",       // Multiple spaces
        };

        var expected = TestEnum.Test1 | TestEnum.Test2;
        foreach (string? testCase in testCases)
        {
            var result = TestEnumEnhanced.ParseFast(testCase);
            result.Should().Be(expected);
        }
    }

    [Fact]
    public void ParseFast_NumericValueWithLeadingPlus_Works()
    {
        var result = TestEnumEnhanced.ParseFast("+1");
        result.Should().Be(TestEnum.Test1);
    }

    [Fact]
    public void ParseFast_NumericValueWithWhitespace_Works()
    {
        var originalResult = Enum.Parse<TestEnum>(" 1 ");
        var result = TestEnumEnhanced.ParseFast(" 1 ");

        result.Should().Be(originalResult);
        result.Should().Be(TestEnum.Test1);
    }

    [Fact]
    public void ParseFast_TestEnum2_NegativeWithSpaces_Works()
    {
        var originalResult = Enum.Parse<TestEnum2>(" -2 ");
        var result = TestEnum2Enhanced.ParseFast(" -2 ");

        result.Should().Be(originalResult);
        result.Should().Be(TestEnum2.Test1);
    }

    [Fact]
    public void TryParseFast_EmptyString_ReturnsFalse()
    {
        bool result = TestEnumEnhanced.TryParseFast("", false, out var value);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryParseFast_WhitespaceOnly_ReturnsFalse()
    {
        string[] testCases = new[] { " ", "  ", "\t", "\n", "\r\n", "   \t  " };

        foreach (string? testCase in testCases)
        {
            bool result = TestEnumEnhanced.TryParseFast(testCase, false, out _);
            result.Should().BeFalse();
        }
    }

    [Fact]
    public void TryParseFast_InvalidNumericFormat_ReturnsFalse()
    {
        string[] invalidFormats = new[]
        {
            "1.5",           // Decimal
            "1,000",         // Thousands separator
            "0x01",          // Hex format
            "1e2",           // Scientific notation
            "++1",           // Double plus
            "--1",           // Double minus
            "+-1",           // Plus-minus
            "1-",            // Trailing sign
        };

        foreach (string? format in invalidFormats)
        {
            bool result = TestEnumEnhanced.TryParseFast(format, false, out _);
            result.Should().BeFalse();
        }
    }

    [Fact]
    public void HasFlagFast_ZeroValue_AlwaysTrue()
    {
        // Zero flag check should always return true
        var values = new[]
        {
            TestEnum.Test0,
            TestEnum.Test1,
            TestEnum.Test2,
            TestEnum.Test1 | TestEnum.Test2,
            (TestEnum)999
        };

        foreach (var value in values)
        {
            value.HasFlagFast(TestEnum.Test0).Should().BeTrue();
            value.HasFlagFast(TestEnum.Test0).Should().Be(value.HasFlag(TestEnum.Test0));
        }
    }

    [Fact]
    public void HasFlagFast_ValueCheckingItself_AlwaysTrue()
    {
        var values = TestEnumEnhanced.GetValuesFast();
        foreach (var value in values)
        {
            value.HasFlagFast(value).Should().BeTrue();
            value.HasFlagFast(value).Should().Be(value.HasFlag(value));
        }
    }

    [Fact]
    public void HasFlagFast_LargerFlagInSmallerValue_ReturnsFalse()
    {
        var small = TestEnum.Test1;
        var large = TestEnum.Test1 | TestEnum.Test2;

        small.HasFlagFast(large).Should().BeFalse();
        small.HasFlagFast(large).Should().Be(small.HasFlag(large));
    }

    [Fact]
    public void HasFlagFast_PartialOverlap_ReturnsCorrectResult()
    {
        var value1 = TestEnum.Test1 | TestEnum.Test2;
        var value2 = TestEnum.Test2 | TestEnum.Test4;

        // value1 does not have all bits of value2
        value1.HasFlagFast(value2).Should().BeFalse();
        value1.HasFlagFast(value2).Should().Be(value1.HasFlag(value2));

        // But it does have Test2
        value1.HasFlagFast(TestEnum.Test2).Should().BeTrue();
    }

    [Fact]
    public void IsDefinedFast_UndefinedValue_ReturnsFalse()
    {
        var undefined = (TestEnum)999;
        TestEnumEnhanced.IsDefinedFast(undefined).Should().BeFalse();
        TestEnumEnhanced.IsDefinedFast(undefined).Should().Be(Enum.IsDefined(typeof(TestEnum), undefined));
    }

    [Fact]
    public void IsDefinedFast_CombinedFlags_ReturnsFalse()
    {
        var combined = TestEnum.Test1 | TestEnum.Test2;
        TestEnumEnhanced.IsDefinedFast(combined).Should().BeFalse();
        TestEnumEnhanced.IsDefinedFast(combined).Should().Be(Enum.IsDefined(typeof(TestEnum), combined));
    }

    [Fact]
    public void IsDefinedFast_UnderlyingTypeOverflow_ReturnsFalse()
    {
        long overflowValue = (long)int.MaxValue + 1;
        int truncated = (int)overflowValue;

        TestEnumEnhanced.IsDefinedFast(truncated).Should().BeFalse();
    }

    [Fact]
    public void GetValuesFast_ContainsDuplicateValues()
    {
        var values = TestEnumEnhanced.GetValuesFast();

        // Should contain both Test1 and Test_1
        values.Should().Contain(TestEnum.Test1);
        values.Should().Contain(TestEnum.Test_1);

        // They have the same underlying value
        ((int)TestEnum.Test_1).Should().Be((int)TestEnum.Test1);
    }

    [Fact]
    public void GetNamesFast_ContainsBothDuplicateNames()
    {
        string[] names = TestEnumEnhanced.GetNamesFast();

        names.Should().Contain("Test1");
        names.Should().Contain("Test_1");
    }

    [Fact]
    public void ToStringFast_EmptyFlags_ReturnsZero()
    {
        var empty = (TestEnum)0;
        string? resultFast = empty.ToStringFast();
        string? resultDefault = empty.ToString();

        resultFast.Should().Be(resultDefault);
        resultFast.Should().Be("Test0");
    }

    [Fact]
    public void ToStringFast_NonPowerOfTwo_Test7_HandledCorrectly()
    {
        // Test7 doesn't have explicit value, so it's auto-assigned
        var test7 = TestEnum.Test7;
        string? resultFast = test7.ToStringFast();
        string? resultDefault = test7.ToString();

        resultFast.Should().Be(resultDefault);
        resultFast.Should().Be("Test7");
    }

    [Fact]
    public void RoundTrip_UndefinedValue_PreservesValue()
    {
        var undefined = (TestEnum)12345;
        string? stringValueFast = undefined.ToStringFast();
        string? stringValueDefault = undefined.ToString();

        stringValueFast.Should().NotBeNull();
        stringValueFast.Should().Be(stringValueDefault);

        // Parsing back might work if it's numeric
        if (int.TryParse(stringValueFast, out int numValue))
        {
            numValue.Should().Be(12345);
        }
    }

    [Fact]
    public void RoundTrip_AllPossibleDuplicateCombinations()
    {
        // Test all combinations of Test1/Test_1 with other flags
        var flagsToTest = new[]
        {
            TestEnum.Test2,
            TestEnum.Test4,
            TestEnum.Test5
        };

        foreach (var flag in flagsToTest)
        {
            var combo1 = TestEnum.Test1 | flag;
            var combo2 = TestEnum.Test_1 | flag;

            // Should produce same numeric value
            ((int)combo2).Should().Be((int)combo1);

            // ToString should be identical
            combo2.ToStringFast().Should().Be(combo1.ToStringFast());
            combo1.ToStringFast().Should().Be(combo1.ToString());

            // Round trip should work
            var parsedFast = TestEnumEnhanced.ParseFast(combo1.ToStringFast()!);
            var parsedDefault = (TestEnum)Enum.Parse(typeof(TestEnum), combo1.ToString());
            parsedFast.Should().Be(parsedDefault);
            parsedFast.Should().Be(combo1);
        }
    }

    [Fact]
    public void EdgeCase_ConsecutiveValues_AllParseable()
    {
        bool canParseFast2 = TestEnumEnhanced.TryParseFast("2332", false, out var parsedFast2);
        bool canParseDefault2 = Enum.TryParse(typeof(TestEnum), "2332", false, out object? parsedDefault2);

        // Test parsing of consecutive values
        for (int i = 0; i <= 128; i++)
        {
            var enumValue = (TestEnum)i;
            string? toStringFast = enumValue.ToStringFast();
            string? toStringDefault = enumValue.ToString();

            toStringFast.Should().NotBeNull();
            toStringFast.Should().Be(toStringDefault);

            // Should be parseable
            bool canParseFast = TestEnumEnhanced.TryParseFast(toStringFast, false, out var parsedFast);
            bool canParseDefault = Enum.TryParse(typeof(TestEnum), toStringDefault, false, out object? parsedDefault);

            if (canParseFast || canParseDefault)
            {
                canParseFast.Should().Be(canParseDefault);
                if (canParseFast)
                {
                    ((object)parsedFast).Should().Be(parsedDefault);
                    parsedFast.Should().Be(enumValue);
                }
            }
        }
    }

    [Fact]
    public void EdgeCase_PowerOfTwoValues_CorrectDecomposition()
    {
        // All power of 2 values should decompose to themselves
        var powersOfTwo = new[]
        {
            TestEnum.Test1,  // 1
            TestEnum.Test2,  // 2
            TestEnum.Test4,  // 4
            TestEnum.Test5,  // 16
            TestEnum.Test6   // 64
        };

        foreach (var value in powersOfTwo)
        {
            string? resultFast = value.ToStringFast();
            string? resultDefault = value.ToString();

            resultFast.Should().NotBeNull();
            resultFast.Should().Be(resultDefault);
            resultFast.Should().NotContain(", "); // Should be single value
        }
    }

    [Fact]
    public void EdgeCase_AllCombinationsOfThreeFlags()
    {
        var flags = new[] { TestEnum.Test1, TestEnum.Test2, TestEnum.Test4 };

        // Test all 2^3 = 8 combinations
        for (int i = 0; i < 8; i++)
        {
            TestEnum combination = 0;

            for (int j = 0; j < 3; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    combination |= flags[j];
                }
            }

            if (combination != 0)
            {
                string? resultFast = combination.ToStringFast();
                string? resultDefault = combination.ToString();

                resultFast.Should().NotBeNull();
                resultFast.Should().Be(resultDefault);
            }
        }
    }

    [Fact]
    public void StressTest_AllDefinedValues_RoundTrip()
    {
        var values = TestEnumEnhanced.GetValuesFast();

        foreach (var value in values)
        {
            // GetNameFast round trip - compare with Enum.GetName
            string? name = value.GetNameFast(false);
            string? nameDefault = Enum.GetName(typeof(TestEnum), value);
            name.Should().Be(nameDefault);

            if (name != null)
            {
                var parsedFast = TestEnumEnhanced.ParseFast(name);
                var parsedDefault = (TestEnum)Enum.Parse(typeof(TestEnum), name);
                parsedFast.Should().Be(parsedDefault);
                parsedFast.Should().Be(value);
            }

            // ToStringFast round trip - compare with ToString
            string? toStringFast = value.ToStringFast();
            string? toStringDefault = value.ToString();
            toStringFast.Should().NotBeNull();
            toStringFast.Should().Be(toStringDefault);

            bool canParseFast = TestEnumEnhanced.TryParseFast(toStringFast, false, out var parsedToStringFast);
            bool canParseDefault = Enum.TryParse(typeof(TestEnum), toStringDefault, false, out object? parsedToStringDefault);

            canParseFast.Should().Be(canParseDefault);
            if (canParseFast)
            {
                ((object)parsedToStringFast).Should().Be(parsedToStringDefault);
                parsedToStringFast.Should().Be(value);
            }
        }
    }

    [Fact]
    public void StressTest_RandomFlagCombinations()
    {
        var random = new Random(42); // Seeded for reproducibility
        var allFlags = TestEnumEnhanced.GetValuesFast()
            .Where(v => v != TestEnum.Test0)
            .Distinct()
            .ToArray();

        for (int i = 0; i < 100; i++)
        {
            TestEnum combination = 0;
            int flagCount = random.Next(1, allFlags.Length);

            for (int j = 0; j < flagCount; j++)
            {
                combination |= allFlags[random.Next(allFlags.Length)];
            }

            // ToString should match default
            string? resultFast = combination.ToStringFast();
            string? resultDefault = combination.ToString();
            resultFast.Should().NotBeNull();
            resultFast.Should().Be(resultDefault);

            // Parsing should work and match default
            bool canParseFast = TestEnumEnhanced.TryParseFast(resultFast, false, out var parsedFast);
            bool canParseDefault = Enum.TryParse(typeof(TestEnum), resultDefault, false, out object? parsedDefault);

            canParseFast.Should().Be(canParseDefault);
            if (canParseFast)
            {
                ((object)parsedFast).Should().Be(parsedDefault);
                parsedFast.Should().Be(combination);
            }
        }
    }
}
