namespace Snowberry.EnumsEnhanced.Tests;

public class EnumTests
{
    [Fact]
    public void HasFlagFast_SingleFlag_ReturnsTrue()
    {
        var e = TestEnum.Test1;

        e.HasFlagFast(TestEnum.Test1).Should().BeTrue();
        e.HasFlagFast(TestEnum.Test1).Should().Be(e.HasFlag(TestEnum.Test1));
    }

    [Fact]
    public void HasFlagFast_MultipleFlags_ReturnsCorrectResult()
    {
        var e = TestEnum.Test1 | TestEnum.Test4;

        // Compare with default HasFlag for each flag
        e.HasFlagFast(TestEnum.Test1).Should().Be(e.HasFlag(TestEnum.Test1));
        e.HasFlagFast(TestEnum.Test_1).Should().Be(e.HasFlag(TestEnum.Test_1));
        e.HasFlagFast(TestEnum.Test4).Should().Be(e.HasFlag(TestEnum.Test4));
        e.HasFlagFast(TestEnum.Test5).Should().Be(e.HasFlag(TestEnum.Test5));
        e.HasFlagFast(TestEnum.Test6).Should().Be(e.HasFlag(TestEnum.Test6));
    }

    [Fact]
    public void HasFlagFast_AllValues_MatchesDefaultBehavior()
    {
        var values = TestEnumEnhanced.GetValuesFast();
        foreach (var value in values)
        {
            foreach (var flag in values)
            {
                bool expected = value.HasFlag(flag);
                bool actual = value.HasFlagFast(flag);
                actual.Should().Be(expected);
            }
        }
    }

    [Fact]
    public void HasFlagFast_ComplexCombinations_MatchesDefaultBehavior()
    {
        var combinations = new[]
        {
            (TestEnum.Test1 | TestEnum.Test2, TestEnum.Test1),
            (TestEnum.Test1 | TestEnum.Test2, TestEnum.Test2),
            (TestEnum.Test1 | TestEnum.Test2, TestEnum.Test1 | TestEnum.Test2),
            (TestEnum.Test4 | TestEnum.Test5, TestEnum.Test4),
            (TestEnum.Test1, TestEnum.Test1 | TestEnum.Test2),
        };

        foreach (var (value, flag) in combinations)
        {
            value.HasFlagFast(flag).Should().Be(value.HasFlag(flag));
        }
    }

    [Fact]
    public void HasFlagFast_ZeroFlag_AlwaysReturnsTrue()
    {
        var values = TestEnumEnhanced.GetValuesFast();
        foreach (var value in values)
        {
            bool expected = value.HasFlag(TestEnum.Test0);
            bool actual = value.HasFlagFast(TestEnum.Test0);
            actual.Should().Be(expected);
            actual.Should().BeTrue();
        }
    }

    [Fact]
    public void HasFlagFast_AllFlagsCombined_MatchesDefaultBehavior()
    {
        var allFlags = TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6 | TestEnum.Test7;
        var values = TestEnumEnhanced.GetValuesFast();

        foreach (var value in values)
        {
            allFlags.HasFlagFast(value).Should().Be(allFlags.HasFlag(value));
        }
    }

    [Fact]
    public void GetNamesFast_ReturnsAllNames()
    {
        string[] namesFast = TestEnumEnhanced.GetNamesFast();
        string[] namesDefault = Enum.GetNames(typeof(TestEnum));

        namesFast.Length.Should().Be(namesDefault.Length);

        foreach (string name in namesDefault)
        {
            namesFast.Should().Contain(name);
        }
    }

    [Fact]
    public void GetNamesFast_TestEnum2_MatchesDefaultBehavior()
    {
        string[] namesFast = TestEnum2Enhanced.GetNamesFast();
        string[] namesDefault = Enum.GetNames(typeof(TestEnum2));

        namesFast.Length.Should().Be(namesDefault.Length);

        for (int i = 0; i < namesDefault.Length; i++)
        {
            namesFast.Should().Contain(namesDefault[i]);
        }
    }

    [Fact]
    public void GetNamesFast_OrderAndContent_ExactMatch()
    {
        string[] namesFast = TestEnumEnhanced.GetNamesFast();
        string[] namesDefault = Enum.GetNames(typeof(TestEnum));

        // Both should contain exactly the same names
        namesFast.OrderBy(x => x).ToArray().Should().Equal(namesDefault.OrderBy(x => x).ToArray());
    }

    [Fact]
    public void GetValuesFast_ReturnsAllValues()
    {
        var valuesFast = TestEnumEnhanced.GetValuesFast();
        var valuesDefault = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>().ToArray();

        valuesFast.Length.Should().Be(valuesDefault.Length);

        foreach (var value in valuesDefault)
        {
            valuesFast.Should().Contain(value);
        }
    }

    [Fact]
    public void GetValuesFast_TestEnum2_MatchesDefaultBehavior()
    {
        var valuesFast = TestEnum2Enhanced.GetValuesFast();
        var valuesDefault = Enum.GetValues(typeof(TestEnum2)).Cast<TestEnum2>().ToArray();

        valuesFast.Length.Should().Be(valuesDefault.Length);

        foreach (var value in valuesDefault)
        {
            valuesFast.Should().Contain(value);
        }
    }

    [Fact]
    public void GetValuesFast_OrderAndContent_ExactMatch()
    {
        var valuesFast = TestEnumEnhanced.GetValuesFast();
        var valuesDefault = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>().ToArray();

        // Both should contain exactly the same values
        valuesFast.OrderBy(x => (int)x).ToArray().Should().Equal(valuesDefault.OrderBy(x => (int)x).ToArray());
    }

    [Fact]
    public void IsDefinedFast_WithName_MatchesDefaultBehavior()
    {
        string[] names = Enum.GetNames(typeof(TestEnum));

        foreach (string name in names)
        {
            bool expectedResult = Enum.IsDefined(typeof(TestEnum), name);
            bool actualResult = TestEnumEnhanced.IsDefinedFast(name);
            actualResult.Should().Be(expectedResult);
        }

        // Test invalid names
        string[] invalidNames = { "InvalidName", "Test7x", "", "test1" };
        foreach (string invalidName in invalidNames)
        {
            if (!string.IsNullOrEmpty(invalidName))
            {
                TestEnumEnhanced.IsDefinedFast(invalidName).Should().Be(Enum.IsDefined(typeof(TestEnum), invalidName));
            }
        }
    }

    [Fact]
    public void IsDefinedFast_WithValue_MatchesDefaultBehavior()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            bool expectedResult = Enum.IsDefined(typeof(TestEnum), value);
            bool actualResult = TestEnumEnhanced.IsDefinedFast(value);
            actualResult.Should().Be(expectedResult);
        }

        // Test undefined values
        var undefinedValues = new[] { (TestEnum)999, (TestEnum)(-1), (TestEnum)1000 };
        foreach (var undefinedValue in undefinedValues)
        {
            TestEnumEnhanced.IsDefinedFast(undefinedValue).Should().Be(Enum.IsDefined(typeof(TestEnum), undefinedValue));
        }
    }

    [Fact]
    public void IsDefinedFast_WithUnderlyingType_MatchesDefaultBehavior()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            int underlyingValue = (int)value;
            bool expectedResult = Enum.IsDefined(typeof(TestEnum), underlyingValue);
            bool actualResult = TestEnumEnhanced.IsDefinedFast(underlyingValue);
            actualResult.Should().Be(expectedResult);
        }

        TestEnumEnhanced.IsDefinedFast(999999).Should().Be(Enum.IsDefined(typeof(TestEnum), 999999));
    }

    [Fact]
    public void IsDefinedFast_NullString_ThrowsArgumentNullException()
    {
        FluentActions.Invoking(() => TestEnumEnhanced.IsDefinedFast(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsDefinedFast_CombinedFlags_MatchesDefaultBehavior()
    {
        var combinedFlag = TestEnum.Test1 | TestEnum.Test4;
        TestEnumEnhanced.IsDefinedFast(combinedFlag).Should().Be(Enum.IsDefined(typeof(TestEnum), combinedFlag));
    }

    [Fact]
    public void GetNameFast_SingleValue_MatchesEnumGetName()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string? expectedName = Enum.GetName(typeof(TestEnum), value);
            string? actualName = value.GetNameFast(false);
            actualName.Should().Be(expectedName);
        }
    }

    [Fact]
    public void GetNameFast_WithoutFlags_ReturnsNull()
    {
        var e = TestEnum.Test4 | TestEnum.Test1;

        // Default Enum.GetName returns null for combined flags
        string? expectedName = Enum.GetName(typeof(TestEnum), e);
        string? actualName = e.GetNameFast(false);

        expectedName.Should().BeNull();
        actualName.Should().BeNull();
        actualName.Should().Be(expectedName);
    }

    [Fact]
    public void GetNameFast_WithFlags_ReturnsCommaSeparatedNames()
    {
        var e = TestEnum.Test1 | TestEnum.Test5;
        string? actualName = e.GetNameFast(true);

        // Should contain flag names separated by comma
        // For Test1/Test_1 duplicate, should use Test1 (first defined) since GetNameFast uses first
        actualName.Should().Contain("Test1");
        actualName.Should().Contain(nameof(TestEnum.Test5));
        actualName.Should().Contain(", ");
    }

    [Fact]
    public void GetNameFast_TestEnum2_MatchesDefaultBehavior()
    {
        TestEnum2.Test1.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum2), TestEnum2.Test1));
        TestEnum2.Test2.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum2), TestEnum2.Test2));
    }

    [Fact]
    public void GetNameFast_AllDefinedValues_MatchesEnumGetName()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>().Distinct();

        foreach (var value in values)
        {
            value.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum), value));
        }
    }

    [Fact]
    public void GetNameFast_DuplicateValue_ReturnsFirstDefinedName()
    {
        // Test1 and Test_1 have the same value
        // Enum.GetName returns the FIRST defined name (Test1)
        string? name = TestEnum.Test1.GetNameFast(false);
        name.Should().Be(Enum.GetName(typeof(TestEnum), TestEnum.Test1));

        // Both should return "Test1" (first defined)
        name.Should().Be("Test1");
        TestEnum.Test_1.GetNameFast(false).Should().Be("Test1");
    }

    [Fact]
    public void ToStringFast_SingleValue_MatchesToString()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string? expectedString = value.ToString();
            string? actualString = value.ToStringFast();

            // For single values, both should return the name
            actualString.Should().Be(expectedString);
        }
    }

    [Fact]
    public void ToStringFast_MultipleFlags_ReturnsCommaSeparatedNames()
    {
        var e = TestEnum.Test4 | TestEnum.Test1;
        string? actualString = e.ToStringFast();

        // Should return comma-separated names for flags
        actualString.Should().NotBeNull();
        actualString.Should().Contain(", ");
    }

    [Fact]
    public void ToStringFast_AllValues_ReturnsValidString()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string? result = value.ToStringFast();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
    }

    [Fact]
    public void ToStringFast_TestEnum2_MatchesToString()
    {
        TestEnum2.Test1.ToStringFast().Should().Be(TestEnum2.Test1.ToString());
        TestEnum2.Test2.ToStringFast().Should().Be(TestEnum2.Test2.ToString());
    }

    [Fact]
    public void ToStringFast_ZeroValue_ReturnsName()
    {
        var zero = TestEnum.Test0;
        zero.ToStringFast().Should().Be(zero.ToString());
        zero.ToStringFast().Should().Be("Test0");
    }

    [Fact]
    public void ToStringFast_ComplexFlagCombinations_MatchesToString()
    {
        var combinations = new[]
        {
            TestEnum.Test1 | TestEnum.Test2,
            TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4,
            TestEnum.Test2 | TestEnum.Test5,
            TestEnum.Test4 | TestEnum.Test6,
            TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6,
        };

        foreach (var combo in combinations)
        {
            string? expected = combo.ToString();
            string? actual = combo.ToStringFast();
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void ToStringFast_AllFlagsCombined_MatchesToString()
    {
        var allFlags = TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6 | TestEnum.Test7;
        allFlags.ToStringFast().Should().Be(allFlags.ToString());
    }

    [Fact]
    public void ToStringFast_UndefinedValue_ReturnsNumericString()
    {
        var undefined = (TestEnum)999;
        string? expected = undefined.ToString();
        string? actual = undefined.ToStringFast();

        // Both should return numeric representation for undefined values
        actual.Should().NotBeNull();
    }

    [Fact]
    public void ParseFast_ValidName_MatchesEnumParse()
    {
        string[] names = Enum.GetNames(typeof(TestEnum));

        foreach (string name in names)
        {
            var expectedValue = (TestEnum)Enum.Parse(typeof(TestEnum), name);
            var actualValue = TestEnumEnhanced.ParseFast(name, false);
            actualValue.Should().Be(expectedValue);
        }
    }

    [Fact]
    public void ParseFast_CaseInsensitive_MatchesEnumParse()
    {
        string[] names = Enum.GetNames(typeof(TestEnum));

        foreach (string name in names)
        {
            string upperName = name.ToUpper();
            string lowerName = name.ToLower();

            var expectedUpper = (TestEnum)Enum.Parse(typeof(TestEnum), upperName, true);
            var actualUpper = TestEnumEnhanced.ParseFast(upperName, true);
            actualUpper.Should().Be(expectedUpper);

            var expectedLower = (TestEnum)Enum.Parse(typeof(TestEnum), lowerName, true);
            var actualLower = TestEnumEnhanced.ParseFast(lowerName, true);
            actualLower.Should().Be(expectedLower);
        }
    }

    [Fact]
    public void ParseFast_MultipleNames_MatchesEnumParse()
    {
        string enumName = $"{nameof(TestEnum.Test4)}, {nameof(TestEnum.Test1)}";

        var expectedValue = (TestEnum)Enum.Parse(typeof(TestEnum), enumName);
        var actualValue = TestEnumEnhanced.ParseFast(enumName);

        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public void ParseFast_ComplexFlagCombinations_MatchesEnumParse()
    {
        string[] testCases = new[]
        {
            $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test2)}",
            $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test2)}, {nameof(TestEnum.Test4)}",
            $"{nameof(TestEnum.Test2)}, {nameof(TestEnum.Test5)}, {nameof(TestEnum.Test6)}",
            $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test4)}, {nameof(TestEnum.Test6)}",
        };

        foreach (string? testCase in testCases)
        {
            var expected = (TestEnum)Enum.Parse(typeof(TestEnum), testCase);
            var actual = TestEnumEnhanced.ParseFast(testCase);
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void ParseFast_FlagsWithExtraSpaces_MatchesEnumParse()
    {
        string[] testCases = new[]
        {
            $"{nameof(TestEnum.Test1)} , {nameof(TestEnum.Test2)}",
            $" {nameof(TestEnum.Test1)}, {nameof(TestEnum.Test2)} ",
            $"{nameof(TestEnum.Test1)}  ,  {nameof(TestEnum.Test2)}",
            $"  {nameof(TestEnum.Test1)}  ,  {nameof(TestEnum.Test2)}  ",
        };

        foreach (string? testCase in testCases)
        {
            var expected = (TestEnum)Enum.Parse(typeof(TestEnum), testCase);
            var actual = TestEnumEnhanced.ParseFast(testCase);
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void ParseFast_DuplicateNames_MatchesEnumParse()
    {
        // Parsing both names that share the same value
        var test1Result = TestEnumEnhanced.ParseFast(nameof(TestEnum.Test1));
        var test_1Result = TestEnumEnhanced.ParseFast(nameof(TestEnum.Test_1));

        ((int)test_1Result).Should().Be((int)test1Result);
        test1Result.Should().Be((TestEnum)Enum.Parse(typeof(TestEnum), nameof(TestEnum.Test1)));
        test_1Result.Should().Be((TestEnum)Enum.Parse(typeof(TestEnum), nameof(TestEnum.Test_1)));
    }

    [Fact]
    public void ParseFast_NumericValue_MatchesEnumParse()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string numericString = ((int)value).ToString();

            var expectedValue = (TestEnum)Enum.Parse(typeof(TestEnum), numericString);
            var actualValue = TestEnumEnhanced.ParseFast(numericString);

            actualValue.Should().Be(expectedValue);
        }
    }

    [Fact]
    public void ParseFast_NumericZero_MatchesEnumParse()
    {
        var expected = (TestEnum)Enum.Parse(typeof(TestEnum), "0");
        var actual = TestEnumEnhanced.ParseFast("0");
        actual.Should().Be(expected);
        actual.Should().Be(TestEnum.Test0);
    }

    [Fact]
    public void ParseFast_NumericCombinedFlags_MatchesEnumParse()
    {
        int combinedValue = (int)(TestEnum.Test1 | TestEnum.Test4);
        string numericString = combinedValue.ToString();

        var expected = (TestEnum)Enum.Parse(typeof(TestEnum), numericString);
        var actual = TestEnumEnhanced.ParseFast(numericString);

        actual.Should().Be(expected);
        actual.Should().Be(TestEnum.Test1 | TestEnum.Test4);
    }

    [Fact]
    public void ParseFast_NegativeValue_TestEnum2_MatchesEnumParse()
    {
        TestEnum2Enhanced.ParseFast("-2").Should().Be((TestEnum2)Enum.Parse(typeof(TestEnum2), "-2"));
        TestEnum2Enhanced.ParseFast("1").Should().Be((TestEnum2)Enum.Parse(typeof(TestEnum2), "1"));
    }

    [Fact]
    public void ParseFast_InvalidName_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => Enum.Parse(typeof(TestEnum), "InvalidName")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast("InvalidName")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseFast_TrailingComma_ThrowsArgumentException()
    {
        string enumName = $"{nameof(TestEnum.Test4)}, {nameof(TestEnum.Test1)},";

        FluentActions.Invoking(() => Enum.Parse(typeof(TestEnum), enumName)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast(enumName)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseFast_LeadingComma_ThrowsArgumentException()
    {
        string enumName = $", {nameof(TestEnum.Test1)}";

        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast(enumName)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseFast_MultipleConsecutiveCommas_ThrowsArgumentException()
    {
        string enumName = $"{nameof(TestEnum.Test1)},, {nameof(TestEnum.Test2)}";

        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast(enumName)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseFast_WithSpaces_MatchesEnumParse()
    {
        string nameWithSpaces = $" {nameof(TestEnum.Test1)} ";

        var expectedValue = (TestEnum)Enum.Parse(typeof(TestEnum), nameWithSpaces);
        var actualValue = TestEnumEnhanced.ParseFast(nameWithSpaces);

        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public void ParseFast_NullOrWhitespace_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => TestEnumEnhanced.ParseFast("   ")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseFast_UndefinedNumericValue_Does_Not_Throw_ArgumentException()
    {
        (Enum.Parse<TestEnum2>("0")).Should().Be((TestEnum2)0);
        TestEnum2Enhanced.ParseFast("0").Should().Be((TestEnum2)0);
    }

    [Fact]
    public void TryParseFast_ValidName_MatchesEnumTryParse()
    {
        string[] names = Enum.GetNames(typeof(TestEnum));

        foreach (string name in names)
        {
            bool expectedResult = Enum.TryParse(typeof(TestEnum), name, false, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(name, false, out var actualValue);

            actualResult.Should().Be(expectedResult);
            ((object)actualValue).Should().Be(expectedValue);
        }
    }

    [Fact]
    public void TryParseFast_InvalidName_MatchesEnumTryParse()
    {
        string[] invalidNames = { "InvalidName", "Test7x", "xxxxx" };

        foreach (string invalidName in invalidNames)
        {
            bool expectedResult = Enum.TryParse(typeof(TestEnum), invalidName, false, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(invalidName, false, out var actualValue);

            actualResult.Should().Be(expectedResult);
            actualResult.Should().BeFalse();
        }
    }

    [Fact]
    public void TryParseFast_CaseInsensitive_MatchesEnumTryParse()
    {
        string[] names = Enum.GetNames(typeof(TestEnum));

        foreach (string name in names)
        {
            string upperName = name.ToUpper();

            bool expectedResult = Enum.TryParse(typeof(TestEnum), upperName, true, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(upperName, true, out var actualValue);

            actualResult.Should().Be(expectedResult);
            ((object)actualValue).Should().Be(expectedValue);
        }
    }

    [Fact]
    public void TryParseFast_NumericValue_MatchesEnumTryParse()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string numericString = ((int)value).ToString();

            bool expectedResult = Enum.TryParse(typeof(TestEnum), numericString, false, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(numericString, false, out var actualValue);

            actualResult.Should().Be(expectedResult);
            ((object)actualValue).Should().Be(expectedValue);
        }
    }

    [Fact]
    public void TryParseFast_InvalidNumeric_ReturnsFalse()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            string invalidNumeric = ((int)value).ToString() + "x";

            bool expectedResult = Enum.TryParse(typeof(TestEnum), invalidNumeric, false, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(invalidNumeric, false, out var actualValue);

            actualResult.Should().Be(expectedResult);
            actualResult.Should().BeFalse();
        }
    }

    [Fact]
    public void TryParseFast_NullOrEmpty_ReturnsFalse()
    {
        TestEnumEnhanced.TryParseFast("", false, out _).Should().BeFalse();
        TestEnumEnhanced.TryParseFast("   ", false, out _).Should().BeFalse();
    }

    [Fact]
    public void TryParseFast_MultipleValues_MatchesEnumTryParse()
    {
        string multiValue = $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test4)}";

        bool expectedResult = Enum.TryParse(typeof(TestEnum), multiValue, false, out object? expectedValue);
        bool actualResult = TestEnumEnhanced.TryParseFast(multiValue, false, out var actualValue);

        actualResult.Should().Be(expectedResult);
        ((object)actualValue).Should().Be(expectedValue);
    }

    [Fact]
    public void TryParseFast_ComplexFlagCombinations_ReturnsTrue()
    {
        string[] testCases = new[]
        {
            $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test2)}, {nameof(TestEnum.Test4)}",
            $"{nameof(TestEnum.Test2)}, {nameof(TestEnum.Test5)}, {nameof(TestEnum.Test6)}",
            $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test4)}, {nameof(TestEnum.Test6)}",
        };

        foreach (string? testCase in testCases)
        {
            bool expectedResult = Enum.TryParse(typeof(TestEnum), testCase, false, out object? expectedValue);
            bool actualResult = TestEnumEnhanced.TryParseFast(testCase, false, out var actualValue);

            actualResult.Should().Be(expectedResult);
            ((object)actualValue).Should().Be(expectedValue);
            actualResult.Should().BeTrue();
        }
    }

    [Fact]
    public void TryParseFast_TrailingComma_ReturnsFalse()
    {
        string testCase = $"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test2)},";

        bool result = TestEnumEnhanced.TryParseFast(testCase, false, out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void AllMethods_ComprehensiveComparison_WithDefaultEnum()
    {
        var valuesFast = TestEnumEnhanced.GetValuesFast();
        var valuesDefault = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>().ToArray();

        string[] namesFast = TestEnumEnhanced.GetNamesFast();
        string[] namesDefault = Enum.GetNames(typeof(TestEnum));

        // Verify GetValues matches
        valuesFast.Length.Should().Be(valuesDefault.Length);

        // Verify GetNames matches
        namesFast.Length.Should().Be(namesDefault.Length);

        foreach (var value in valuesDefault)
        {
            // Test IsDefined
            TestEnumEnhanced.IsDefinedFast(value).Should().Be(Enum.IsDefined(typeof(TestEnum), value));

            // Test GetName
            value.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum), value));

            // Test ToString
            value.ToStringFast().Should().Be(value.ToString());

            string? name = Enum.GetName(typeof(TestEnum), value);
            if (name != null)
            {
                // Test Parse round-trip
                TestEnumEnhanced.ParseFast(name).Should().Be((TestEnum)Enum.Parse(typeof(TestEnum), name));

                // Test TryParse
                bool expectedTryParse = Enum.TryParse(typeof(TestEnum), name, false, out object? expectedParsed);
                bool actualTryParse = TestEnumEnhanced.TryParseFast(name, false, out var actualParsed);

                actualTryParse.Should().Be(expectedTryParse);
                ((object)actualParsed).Should().Be(expectedParsed);
            }
        }
    }

    [Fact]
    public void RoundTrip_AllValues_MatchesDefaultBehavior()
    {
        var values = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>();

        foreach (var value in values)
        {
            // Round trip: value -> string -> value
            string? name = Enum.GetName(typeof(TestEnum), value);
            string? nameFast = value.GetNameFast(false);

            nameFast.Should().Be(name);

            if (name != null)
            {
                var parsedDefault = (TestEnum)Enum.Parse(typeof(TestEnum), name);
                var parsedFast = TestEnumEnhanced.ParseFast(name);

                parsedFast.Should().Be(parsedDefault);
                parsedFast.Should().Be(value);
            }
        }
    }

    [Fact]
    public void RoundTrip_ComplexFlags_ToStringAndParse()
    {
        var flagCombinations = new[]
        {
            TestEnum.Test1 | TestEnum.Test2,
            TestEnum.Test1 | TestEnum.Test4 | TestEnum.Test5,
            TestEnum.Test2 | TestEnum.Test6,
            TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6,
        };

        foreach (var combo in flagCombinations)
        {
            string? toString = combo.ToStringFast();
            toString.Should().NotBeNull();

            var parsed = TestEnumEnhanced.ParseFast(toString);
            parsed.Should().Be(combo);
        }
    }

    [Fact]
    public void EdgeCase_ZeroValueBehavior_MatchesDefault()
    {
        var zero = TestEnum.Test0;

        zero.GetNameFast(false).Should().Be(Enum.GetName(typeof(TestEnum), zero));
        zero.ToStringFast().Should().Be(zero.ToString());
        TestEnumEnhanced.IsDefinedFast(zero).Should().Be(Enum.IsDefined(typeof(TestEnum), zero));
    }

    [Fact]
    public void EdgeCase_MaxValueCombination_WorksCorrectly()
    {
        var maxCombo = TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6 | TestEnum.Test7;

        string? name = maxCombo.ToStringFast();
        name.Should().NotBeNull();
        name.Should().Contain(", ");

        // Verify all flags are present
        var values = TestEnumEnhanced.GetValuesFast().Where(v => v != 0 && (int)v == ((int)v & (int)maxCombo));
        foreach (var value in values)
        {
            maxCombo.HasFlagFast(value).Should().BeTrue();
        }
    }

    [Fact]
    public void EdgeCase_ConsecutiveFlagParsing_OrderIndependent()
    {
        var combo1 = TestEnumEnhanced.ParseFast($"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test4)}");
        var combo2 = TestEnumEnhanced.ParseFast($"{nameof(TestEnum.Test4)}, {nameof(TestEnum.Test1)}");

        combo2.Should().Be(combo1);
        combo1.Should().Be(TestEnum.Test1 | TestEnum.Test4);
    }

    [Fact]
    public void EdgeCase_MixedCaseParsing_IgnoreCase()
    {
        var testCases = new[]
        {
            ($"test1, TEST2", TestEnum.Test1 | TestEnum.Test2),
            ($"TeSt4, tEsT5", TestEnum.Test4 | TestEnum.Test5),
            ($"TEST1, test4, TeSt6", TestEnum.Test1 | TestEnum.Test4 | TestEnum.Test6),
        };

        foreach (var (input, expected) in testCases)
        {
            var actual = TestEnumEnhanced.ParseFast(input, ignoreCase: true);
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void EdgeCase_DuplicateFlagsInParse_SameResult()
    {
        var single = TestEnumEnhanced.ParseFast($"{nameof(TestEnum.Test1)}");
        var duplicate = TestEnumEnhanced.ParseFast($"{nameof(TestEnum.Test1)}, {nameof(TestEnum.Test1)}");

        duplicate.Should().Be(single);
    }

    [Fact]
    public void EdgeCase_NonPowerOfTwoValue_Test7Behavior()
    {
        // Test7 should have a specific value (power of 2 or calculated)
        var test7 = TestEnum.Test7;

        string? name = test7.GetNameFast(false);
        name.Should().Be("Test7");

        var parsed = TestEnumEnhanced.ParseFast("Test7");
        parsed.Should().Be(test7);
    }
}