[![License](https://img.shields.io/github/license/snowberry-software/Snowberry.EnumsEnhanced)](https://github.com/snowberry-software/Snowberry.EnumsEnhanced/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/Snowberry.EnumsEnhanced.svg?logo=nuget)](https://www.nuget.org/packages/Snowberry.EnumsEnhanced/)

# Snowberry.EnumsEnhanced

`Snowberry.EnumsEnhanced` is a C# [source generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators) for generating common (extension) methods for enums without having the cost of the Reflection API (slow and many allocations).

Following extension methods will be generated for each enum:

| Default   | Equivalent    |
| --------- | ------------- |
| GetName   | GetNameFast   |
| GetNames  | GetNamesFast  |
| GetValues | GetValuesFast |
| HasFlag   | HasFlagFast   |
| IsDefined | IsDefinedFast |
| Parse     | ParseFast     |
| TryParse  | TryParseFast  |
| ToString  | ToStringFast  |

# How to use it

Also it will only generate the extension methods if the enum has no containing type.

```cs
internal class EnumContainingTypeTest
{
    // Will not generate any code for this enum.
    public enum ContainingTypeEnum
    {
        Test1
    }
}
```

The generated class will always have the same naming scheme by using your enum type name with an `Enhanced` postfix (e.g. **YourEnumName**`Enhanced`)
or in this example **TestEnumEnhanced**.

```cs
namespace TestEnumEnhancedTest;

[Flags]
public enum TestEnum : short
{
    Test1 = 1 << 0,
    Test2 = 1 << 1,
    Test4 = 1 << 2,
    Test5 = 1 << 4,
    Test6 = 1 << 6,
    Test7
}
```

Generated code:

```cs
#nullable enable

using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace TestEnumEnhancedTest
{
    /// <summary>
    /// Reflection free extension methods for the <see cref="TestEnum"/> type.
    /// </summary>
    public static partial class TestEnumEnhanced
    {
        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <param name="e">The value of the enum.</param>
        /// <param name="flag">The flag to check.</param>
        /// <returns><see langword="true"/> if the bit field or bit fields that are set in <paramref name="flag"/> are also set in <paramref name="e"/>; otherwise, <see langword="false"/>.</returns>
#if NETCOREAPP3_0_OR_GREATER
#else
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool HasFlagFast(this TestEnum e, TestEnum flag)
        {
#if NETCOREAPP3_0_OR_GREATER
            Int16 flagsValue = Unsafe.As<TestEnum, Int16>(ref flag);
            return (Unsafe.As<TestEnum, Int16>(ref e) & flagsValue) == flagsValue;
#else
            return ((Int16)e & (Int16)flag) == (Int16)flag;
#endif
        }

        /// <summary>
        /// Retrieves an array of the names of the constants.
        /// </summary>
        /// <returns>A string array of the names of the constants.</returns>
        public static string[] GetNamesFast()
        {
            return new string[] { "Test1", "Test2", "Test4", "Test5", "Test6", "Test7" };
        }

        /// <summary>
        /// Retrieves an array of the values of the constants.
        /// </summary>
        /// <returns>An array that contains the values of the constants.</returns>
        public static TestEnum[] GetValuesFast()
        {
            return new TestEnum[] { TestEnum.Test1, TestEnum.Test2, TestEnum.Test4, TestEnum.Test5, TestEnum.Test6, TestEnum.Test7 };
        }

        /// <inheritdoc cref="IsDefinedFast(TestEnum)"/>
        /// <param name="value">The name of the enumeration constant.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public static bool IsDefinedFast(string value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            switch (value)
            {
                case "Test1": return true;
                case "Test2": return true;
                case "Test4": return true;
                case "Test5": return true;
                case "Test6": return true;
                case "Test7": return true;
            }

            return false;
        }

        /// <inheritdoc cref="IsDefinedFast(TestEnum)"/>
#if NETCOREAPP3_0_OR_GREATER
#else
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsDefinedFast(Int16 value)
        {
            return IsDefinedFast((TestEnum)value);
        }

        /// <summary>
        /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
        /// </summary>
        /// <param name="value">The value of the enumeration constant.</param>
        /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
        public static bool IsDefinedFast(TestEnum value)
        {
            switch (value)
            {
                case TestEnum.Test1: return true;
                case TestEnum.Test2: return true;
                case TestEnum.Test4: return true;
                case TestEnum.Test5: return true;
                case TestEnum.Test6: return true;
                case TestEnum.Test7: return true;
            }

            return false;
        }

        /// <summary>
        /// Resolves the name of the given enum value.
        /// </summary>
        /// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
        /// <param name="includeFlagNames">Determines whether the value has flags, so it will return <c>EnumValue, EnumValue2</c>.</param>
        /// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
        public static string? GetNameFast(this TestEnum e, bool includeFlagNames = false)
        {
            switch (e)
            {
                case TestEnum.Test1: return nameof(TestEnum.Test1);
                case TestEnum.Test2: return nameof(TestEnum.Test2);
                case TestEnum.Test4: return nameof(TestEnum.Test4);
                case TestEnum.Test5: return nameof(TestEnum.Test5);
                case TestEnum.Test6: return nameof(TestEnum.Test6);
                case TestEnum.Test7: return nameof(TestEnum.Test7);
            }

            // FLAGS ENABLED
            // Returning null is the default behavior.
            if (!includeFlagNames)
                return null;

            string? flagResult = null;
            Int16 checkedMaskCurrent = (Int16)e;
            if ((checkedMaskCurrent & 65) == 65) { flagResult = flagResult is null ? nameof(TestEnum.Test7) : nameof(TestEnum.Test7) + ", " + flagResult; checkedMaskCurrent -= 65; }
            if ((checkedMaskCurrent & 64) == 64) { flagResult = flagResult is null ? nameof(TestEnum.Test6) : nameof(TestEnum.Test6) + ", " + flagResult; checkedMaskCurrent -= 64; }
            if ((checkedMaskCurrent & 16) == 16) { flagResult = flagResult is null ? nameof(TestEnum.Test5) : nameof(TestEnum.Test5) + ", " + flagResult; checkedMaskCurrent -= 16; }
            if ((checkedMaskCurrent & 4) == 4) { flagResult = flagResult is null ? nameof(TestEnum.Test4) : nameof(TestEnum.Test4) + ", " + flagResult; checkedMaskCurrent -= 4; }
            if ((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(TestEnum.Test2) : nameof(TestEnum.Test2) + ", " + flagResult; checkedMaskCurrent -= 2; }
            if ((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(TestEnum.Test1) : nameof(TestEnum.Test1) + ", " + flagResult; checkedMaskCurrent -= 1; }

            if (checkedMaskCurrent != default)
                return ((Int16)e).ToString();

            return flagResult ?? ((Int16)e).ToString();
        }

        private static string? ToStringFastInternal(this TestEnum e, bool includeFlagNames = false)
        {
            switch (e)
            {
                case TestEnum.Test1: return nameof(TestEnum.Test1);
                case TestEnum.Test2: return nameof(TestEnum.Test2);
                case TestEnum.Test4: return nameof(TestEnum.Test4);
                case TestEnum.Test5: return nameof(TestEnum.Test5);
                case TestEnum.Test6: return nameof(TestEnum.Test6);
                case TestEnum.Test7: return nameof(TestEnum.Test7);
            }

            if (!includeFlagNames)
                return null;

            string? flagResult = null;
            Int16 checkedMaskCurrent = (Int16)e;
            if ((checkedMaskCurrent & 65) == 65) { flagResult = flagResult is null ? nameof(TestEnum.Test7) : nameof(TestEnum.Test7) + ", " + flagResult; checkedMaskCurrent -= 65; }
            if ((checkedMaskCurrent & 64) == 64) { flagResult = flagResult is null ? nameof(TestEnum.Test6) : nameof(TestEnum.Test6) + ", " + flagResult; checkedMaskCurrent -= 64; }
            if ((checkedMaskCurrent & 16) == 16) { flagResult = flagResult is null ? nameof(TestEnum.Test5) : nameof(TestEnum.Test5) + ", " + flagResult; checkedMaskCurrent -= 16; }
            if ((checkedMaskCurrent & 4) == 4) { flagResult = flagResult is null ? nameof(TestEnum.Test4) : nameof(TestEnum.Test4) + ", " + flagResult; checkedMaskCurrent -= 4; }
            if ((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(TestEnum.Test2) : nameof(TestEnum.Test2) + ", " + flagResult; checkedMaskCurrent -= 2; }
            if ((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(TestEnum.Test1) : nameof(TestEnum.Test1) + ", " + flagResult; checkedMaskCurrent -= 1; }

            if (checkedMaskCurrent != default)
                return ((Int16)e).ToString();

            return flagResult ?? ((Int16)e).ToString();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
        /// <returns>The string representation of the value of this instance.</returns>
#if NETCOREAPP3_0_OR_GREATER
#else
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif
        public static string? ToStringFast(this TestEnum e)
        {
            return ToStringFastInternal(e, true);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case; <see langword="false"/> to regard case.</param>
        /// <param name="result">The result of the enumeration constant.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
        public static bool TryParseFast(string value, bool ignoreCase, out TestEnum result)
        {
            result = ParseFast(out var successful, value: value, ignoreCase: ignoreCase, throwOnFailure: false);
            return successful;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case; <see langword="false"/> to regard case.</param>
        /// <returns>The enumeration value whose value is represented by the given value.</returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/>, empty, whitespace, or cannot be converted to a defined value.</exception>
        public static TestEnum ParseFast(string value, bool ignoreCase = false)
        {
            return ParseFast(out _, value: value, ignoreCase: ignoreCase, throwOnFailure: true);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="successful"><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</param>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case; <see langword="false"/> to regard case.</param>
        /// <param name="throwOnFailure"><see langword="true"/> to throw on a failed conversion; <see langword="false"/> to return <see langword="default"/> instead.</param>
        /// <returns>The enumeration value whose value is represented by the given value.</returns>
        /// <exception cref="ArgumentException"><paramref name="throwOnFailure"/> is <see langword="true"/> and <paramref name="value"/> is <see langword="null"/>, empty, whitespace, or cannot be converted to a defined value.</exception>
        public static TestEnum ParseFast(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
        {
            successful = false;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (throwOnFailure)
                    throw new ArgumentException("Value can't be null or whitespace!", nameof(value));

                return default;
            }

            Int16 localResult = 0;
            bool parsed = false;
            string subValue;
            string originalValue = value;
            char firstChar = value[0];

            if (char.IsWhiteSpace(firstChar))
                firstChar = value.TrimStart()[0];

            if (char.IsDigit(firstChar) || firstChar == '-' || firstChar == '+')
            {
                if (Int16.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out var valueNumber))
                {
                    parsed = true;
                    localResult = valueNumber;
                }
            }
            else
            while (value != null && value.Length > 0)
            {
                parsed = false;

                int endIndex = value.IndexOf(',');

                if (endIndex < 0)
                {
                    // No next separator; use the remainder as the next value.
                    subValue = value.Trim();
                    value = null!;
                }
                else if (endIndex != value!.Length - 1)
                {
                    // Found a separator before the last char.
                    subValue = value.Substring(0, endIndex).Trim();
                    value = value.Substring(endIndex + 1);
                }
                else
                {
                    // Last char was a separator, which is invalid.
                    break;
                }

                if (!ignoreCase)
                {
                    switch (subValue)
                    {
                        case nameof(TestEnum.Test1): parsed = true; localResult |= 1; break;
                        case nameof(TestEnum.Test2): parsed = true; localResult |= 2; break;
                        case nameof(TestEnum.Test4): parsed = true; localResult |= 4; break;
                        case nameof(TestEnum.Test5): parsed = true; localResult |= 16; break;
                        case nameof(TestEnum.Test6): parsed = true; localResult |= 64; break;
                        case nameof(TestEnum.Test7): parsed = true; localResult |= 65; break;
                    }
                }
                else
                {
                    if (subValue.Equals(nameof(TestEnum.Test1), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 1; }
                    if (subValue.Equals(nameof(TestEnum.Test2), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 2; }
                    if (subValue.Equals(nameof(TestEnum.Test4), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 4; }
                    if (subValue.Equals(nameof(TestEnum.Test5), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 16; }
                    if (subValue.Equals(nameof(TestEnum.Test6), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 64; }
                    if (subValue.Equals(nameof(TestEnum.Test7), StringComparison.OrdinalIgnoreCase)) { parsed = true; localResult |= 65; }
                }

                if (!parsed)
                    break;
            }

            successful = true;

            if (!parsed)
            {
                successful = false;

                if (throwOnFailure)
                    throw new ArgumentException($"Could not convert the given value `{originalValue}`.", nameof(value));
            }

            return (TestEnum)localResult;
        }
    }
}

#nullable restore
```
