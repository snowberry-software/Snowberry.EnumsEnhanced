//HintName: Sample.DirectionEnhanced.g.cs
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
                /// Reflection free extension methods for the <see cref="Direction"/> type.
                /// </summary>
                public static partial class DirectionEnhanced
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

    public static bool HasFlagFast(this Direction e, Direction flag)
    {
#if NETCOREAPP3_0_OR_GREATER
        Int32 flagsValue = Unsafe.As<Direction, Int32>(ref flag);
        return (Unsafe.As<Direction, Int32>(ref e) & flagsValue) == flagsValue;
#else
        return ((Int32)e & (Int32)flag) == (Int32)flag;
#endif
    }

    /// <summary>
    /// Retrieves an array of the names of the constants.
    /// </summary>
    /// <returns>A string array of the names of the constants.</returns>
    public static string[] GetNamesFast()
    {
        return new string[] {
            "North", "East", "South", "West"
        };
    }


    /// <summary>
    /// Retrieves an array of the values of the constants.
    /// </summary>
    /// <returns>An array that contains the values of the constants.</returns>
    public static Direction[] GetValuesFast()
    {
        return new Direction[] {
            Direction.North, Direction.East, Direction.South, Direction.West
        };
    }


    /// <inheritdoc cref="IsDefinedFast(Direction)"/>
    /// <param name="value">The name of the enumeration constant.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsDefinedFast(string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        switch(value)
        {
            case "East":
	return true;
case "West":
	return true;
case "North":
	return true;
case "South":
	return true;

        }

        return false;
    }

    /// <inheritdoc cref="IsDefinedFast(Direction)"/>
    #if NETCOREAPP3_0_OR_GREATER
#else
[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif

    public static bool IsDefinedFast(Int32 value)
    {
        return IsDefinedFast((Direction)value);
    }

    /// <summary>
    /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
    /// </summary>
    /// <param name="value">The value of the enumeration constant.</param>
    /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
    public static bool IsDefinedFast(Direction value)
    {
        switch(value)
        {
            case Direction.East:
	return true;
case Direction.West:
	return true;
case Direction.North:
	return true;
case Direction.South:
	return true;

        }

        return false;
    }
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return <c>EnumValue, EnumValue2</c>.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
public static string? GetNameFast(this Direction e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Direction.North:
	return nameof(Direction.North);

case Direction.East:
	return nameof(Direction.East);

case Direction.South:
	return nameof(Direction.South);

case Direction.West:
	return nameof(Direction.West);


    }

    return ((Int32)e).ToString();

    /*
    // FLAGS DISABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    string? flagResult = null;
    Int32 checkedMaskCurrent = (Int32)e;
    if((checkedMaskCurrent & 3) == 3) { flagResult = flagResult is null ? nameof(Direction.West) : nameof(Direction.West) + ", " + flagResult; checkedMaskCurrent -= 3; }
if((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(Direction.South) : nameof(Direction.South) + ", " + flagResult; checkedMaskCurrent -= 2; }
if((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(Direction.East) : nameof(Direction.East) + ", " + flagResult; checkedMaskCurrent -= 1; }

    if(checkedMaskCurrent != default)
        return ((Int32)e).ToString();

    return flagResult ?? ((Int32)e).ToString();

    */
}
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return <c>EnumValue, EnumValue2</c>.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
private static string? ToStringFastInternal(this Direction e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Direction.North:
	return nameof(Direction.North);

case Direction.East:
	return nameof(Direction.East);

case Direction.South:
	return nameof(Direction.South);

case Direction.West:
	return nameof(Direction.West);


    }

    return ((Int32)e).ToString();

    /*
    // FLAGS DISABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    string? flagResult = null;
    Int32 checkedMaskCurrent = (Int32)e;
    if((checkedMaskCurrent & 3) == 3) { flagResult = flagResult is null ? nameof(Direction.West) : nameof(Direction.West) + ", " + flagResult; checkedMaskCurrent -= 3; }
if((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(Direction.South) : nameof(Direction.South) + ", " + flagResult; checkedMaskCurrent -= 2; }
if((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(Direction.East) : nameof(Direction.East) + ", " + flagResult; checkedMaskCurrent -= 1; }

    if(checkedMaskCurrent != default)
        return ((Int32)e).ToString();

    return flagResult ?? ((Int32)e).ToString();

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

    public static string? ToStringFast(this Direction e)
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
    public static bool TryParseFast(string value, bool ignoreCase, out Direction result)
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
    public static Direction ParseFast(string value, bool ignoreCase = false)
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
    public static Direction ParseFast(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
    {
        successful = false;

        if (string.IsNullOrWhiteSpace(value))
        {
            if (throwOnFailure)
                throw new ArgumentException("Value can't be null or whitespace!", nameof(value));

            return default;
        }

        Int32 localResult = 0;
        bool parsed = false;
        string subValue;
        string originalValue = value;
        char firstChar = value[0];

        if (char.IsWhiteSpace(firstChar))
            firstChar = value.TrimStart()[0];

        if (char.IsDigit(firstChar) || firstChar == '-' || firstChar == '+')
        {
            if(Int32.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out var valueNumber))
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
                    case nameof(Direction.East):
	parsed = true;
	localResult |= 1;
	break;

case nameof(Direction.West):
	parsed = true;
	localResult |= 3;
	break;

case nameof(Direction.North):
	parsed = true;
	localResult |= 0;
	break;

case nameof(Direction.South):
	parsed = true;
	localResult |= 2;
	break;


                }
            }
            else
            {
                if(subValue.Equals(nameof(Direction.East), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 1; }
if(subValue.Equals(nameof(Direction.West), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 3; }
if(subValue.Equals(nameof(Direction.North), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 0; }
if(subValue.Equals(nameof(Direction.South), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 2; }

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

        return (Direction)localResult;
    }


                }
            }

            #nullable restore