//HintName: Sample.HugeEnhanced.g.cs
#nullable enable

            using System.Text;
            using System.Runtime.CompilerServices;
            using System.Collections.Generic;
            using System;
            using System;
            using System.Globalization;

            namespace Sample
            {
                /// <summary>
                /// Reflection free extension methods for the <see cref="Huge"/> type.
                /// </summary>
                public static partial class HugeEnhanced
                {
                    
    /// <summary>
    /// Determines whether one or more bit fields are set in the current instance.
    /// </summary>
    /// <param name="e">The value of the enum.</param>
    /// <param name="flag">The flag to check.</param>
    /// <returns><see langword="true"/> if the bit field or bit fields that are set in flag are also set in the current instance; otherwise, false.</returns>
    #if NETCOREAPP3_0_OR_GREATER
#else
[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif

    public static bool HasFlagFast(this Huge e, Huge flag)
    {
#if NETCOREAPP3_0_OR_GREATER
        UInt64 flagsValue = Unsafe.As<Huge, UInt64>(ref flag);
        return (Unsafe.As<Huge, UInt64>(ref e) & flagsValue) == flagsValue;
#else
        return ((UInt64)e & (UInt64)flag) == (UInt64)flag;
#endif
    }

    /// <summary>
    /// Retrieves an array of the names of the constants.
    /// </summary>
    /// <returns>A string array of the names of the constants.</returns>
    public static string[] GetNamesFast()
    {
        return new string[] {
            "Zero", "Mid", "Max"
        };
    }


    /// <summary>
    /// Retrieves an array of the values of the constants.
    /// </summary>
    /// <returns>An array that contains the values of the constants.</returns>
    public static Huge[] GetValuesFast()
    {
        return new Huge[] {
            Huge.Zero, Huge.Mid, Huge.Max
        };
    }


    /// <inheritdoc cref="IsDefinedFast(Huge)"/>
    public static bool IsDefinedFast(string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        switch(value)
        {
            case "Mid":
	return true;
case "Max":
	return true;
case "Zero":
	return true;

        }

        return false;
    }

    /// <inheritdoc cref="IsDefinedFast(Huge)"/>
    #if NETCOREAPP3_0_OR_GREATER
#else
[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif

    public static bool IsDefinedFast(UInt64 value)
    {
        return IsDefinedFast((Huge)value);
    }

    /// <summary>
    /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
    /// </summary>
    /// <param name="value">The value of the enumeration constant.</param>
    /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
    public static bool IsDefinedFast(Huge value)
    {
        switch(value)
        {
            case Huge.Mid:
	return true;
case Huge.Max:
	return true;
case Huge.Zero:
	return true;

        }

        return false;
    }
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return `EnumValue, EnumValue2`.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
public static string? GetNameFast(this Huge e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Huge.Zero:
	return nameof(Huge.Zero);

case Huge.Mid:
	return nameof(Huge.Mid);

case Huge.Max:
	return nameof(Huge.Max);


    }

    return ((UInt64)e).ToString();

    /*
    // FLAGS DISABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    var flagBuilder = new StringBuilder();
    UInt64 checkedMaskCurrent = (UInt64)e;
if((checkedMaskCurrent & 18446744073709551615) == 18446744073709551615) {
	flagBuilder.Insert(0, Huge.Max.GetNameFast(false)).Insert(0, ", ");
	checkedMaskCurrent -= 18446744073709551615; }

if((checkedMaskCurrent & 9223372036854775808) == 9223372036854775808) {
	flagBuilder.Insert(0, Huge.Mid.GetNameFast(false)).Insert(0, ", ");
	checkedMaskCurrent -= 9223372036854775808; }


    if(checkedMaskCurrent != default)
        return ((UInt64)e).ToString();

    return flagBuilder.ToString().Trim(s_flagTrimChars);

    */
}
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return `EnumValue, EnumValue2`.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
private static string? ToStringFastInternal(this Huge e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Huge.Zero:
	return nameof(Huge.Zero);

case Huge.Mid:
	return nameof(Huge.Mid);

case Huge.Max:
	return nameof(Huge.Max);


    }

    return ((UInt64)e).ToString();

    /*
    // FLAGS DISABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    var flagBuilder = new StringBuilder();
    UInt64 checkedMaskCurrent = (UInt64)e;
if((checkedMaskCurrent & 18446744073709551615) == 18446744073709551615) {
	flagBuilder.Insert(0, Huge.Max.ToStringFastInternal(false)).Insert(0, ", ");
	checkedMaskCurrent -= 18446744073709551615; }

if((checkedMaskCurrent & 9223372036854775808) == 9223372036854775808) {
	flagBuilder.Insert(0, Huge.Mid.ToStringFastInternal(false)).Insert(0, ", ");
	checkedMaskCurrent -= 9223372036854775808; }


    if(checkedMaskCurrent != default)
        return ((UInt64)e).ToString();

    return flagBuilder.ToString().Trim(s_flagTrimChars);

    */
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

    public static string? ToStringFast(this Huge e)
    {
        return ToStringFastInternal(e, true);
    }

    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// </summary>
    /// <param name="value">A string containing the name or value to convert.</param>
    /// <param name="ignoreCase"><see langword="true"/> to ignore case; false to regard case.</param>
    /// <param name="result">The result of the enumeration constant.</param>
    /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
    public static bool TryParseFast(string value, bool ignoreCase, out Huge result)
    {
        result = ParseFast(out var successful, value: value, ignoreCase: ignoreCase, throwOnFailure: false);
        return successful;
    }

    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// </summary>
    /// <param name="value">A string containing the name or value to convert.</param>
    /// <param name="ignoreCase"><see langword="true"/> to ignore case; false to regard case.</param>
    /// <returns>The enumeration value whose value is represented by the given value.</returns>
    public static Huge ParseFast(string value, bool ignoreCase = false)
    {
        return ParseFast(out _, value: value, ignoreCase: ignoreCase, throwOnFailure: true);
    }

    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// </summary>
    /// <param name="successful"><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</param>
    /// <param name="value">A string containing the name or value to convert.</param>
    /// <param name="ignoreCase"><see langword="true"/> to ignore case; false to regard case.</param>
    /// <param name="throwOnFailure">Determines whether to throw an <see cref="Exception"/> on errors or not.</param>
    /// <returns>The enumeration value whose value is represented by the given value.</returns>
    public static Huge ParseFast(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
    {
        successful = false;

        if (string.IsNullOrWhiteSpace(value))
        {
            if (throwOnFailure)
                throw new ArgumentException("Value can't be null or whitespace!", nameof(value));

            return default;
        }

        UInt64 localResult = 0;
        bool parsed = false;
        string subValue;
        string originalValue = value;
        char firstChar = value[0];

        if (char.IsWhiteSpace(firstChar))
            firstChar = value.TrimStart()[0];

        if (char.IsDigit(firstChar) || firstChar == '-' || firstChar == '+')
        {
            if(UInt64.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out var valueNumber))
            {
                parsed = true;
                localResult = valueNumber;
            }
        }
        else
        while(value != null && value.Length > 0)
        {
            parsed = false;

            int endIndex = value.IndexOf(',');

            if(endIndex < 0)
            {
                // No next separator; use the remainder as the next value.
                subValue = value.Trim();
                value = null!;
            }
            else if(endIndex != value!.Length - 1)
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

            if(!ignoreCase)
            {
                switch(subValue)
                {
                    case nameof(Huge.Mid):
	parsed = true;
	localResult |= 9223372036854775808;
	break;

case nameof(Huge.Max):
	parsed = true;
	localResult |= 18446744073709551615;
	break;

case nameof(Huge.Zero):
	parsed = true;
	localResult |= 0;
	break;


                }
            }
            else
            {
                if(subValue.Equals(nameof(Huge.Mid), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 9223372036854775808; }
if(subValue.Equals(nameof(Huge.Max), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 18446744073709551615; }
if(subValue.Equals(nameof(Huge.Zero), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 0; }

            }

            if(!parsed)
                break;
        }

        successful = true;

        if (!parsed)
        {
            successful = false;

            if (throwOnFailure)
                throw new ArgumentException($"Could not convert the given value `{originalValue}`.", nameof(value));
        }

        return (Huge)localResult;
    }


                }
            }

            #nullable restore