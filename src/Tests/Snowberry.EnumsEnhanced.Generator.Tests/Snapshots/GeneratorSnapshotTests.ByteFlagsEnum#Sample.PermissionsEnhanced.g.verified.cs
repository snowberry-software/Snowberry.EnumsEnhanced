//HintName: Sample.PermissionsEnhanced.g.cs
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
                /// Reflection free extension methods for the <see cref="Permissions"/> type.
                /// </summary>
                public static partial class PermissionsEnhanced
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

    public static bool HasFlagFast(this Permissions e, Permissions flag)
    {
#if NETCOREAPP3_0_OR_GREATER
        Byte flagsValue = Unsafe.As<Permissions, Byte>(ref flag);
        return (Unsafe.As<Permissions, Byte>(ref e) & flagsValue) == flagsValue;
#else
        return ((Byte)e & (Byte)flag) == (Byte)flag;
#endif
    }

    /// <summary>
    /// Retrieves an array of the names of the constants.
    /// </summary>
    /// <returns>A string array of the names of the constants.</returns>
    public static string[] GetNamesFast()
    {
        return new string[] {
            "None", "A", "B", "C", "All"
        };
    }


    /// <summary>
    /// Retrieves an array of the values of the constants.
    /// </summary>
    /// <returns>An array that contains the values of the constants.</returns>
    public static Permissions[] GetValuesFast()
    {
        return new Permissions[] {
            Permissions.None, Permissions.A, Permissions.B, Permissions.C, Permissions.All
        };
    }


    /// <inheritdoc cref="IsDefinedFast(Permissions)"/>
    /// <param name="value">The name of the enumeration constant.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsDefinedFast(string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        switch(value)
        {
            case "A":
	return true;
case "B":
	return true;
case "C":
	return true;
case "All":
	return true;
case "None":
	return true;

        }

        return false;
    }

    /// <inheritdoc cref="IsDefinedFast(Permissions)"/>
    #if NETCOREAPP3_0_OR_GREATER
#else
[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif

    public static bool IsDefinedFast(Byte value)
    {
        return IsDefinedFast((Permissions)value);
    }

    /// <summary>
    /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
    /// </summary>
    /// <param name="value">The value of the enumeration constant.</param>
    /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
    public static bool IsDefinedFast(Permissions value)
    {
        switch(value)
        {
            case Permissions.A:
	return true;
case Permissions.B:
	return true;
case Permissions.C:
	return true;
case Permissions.All:
	return true;
case Permissions.None:
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
public static string? GetNameFast(this Permissions e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Permissions.None:
	return nameof(Permissions.None);

case Permissions.A:
	return nameof(Permissions.A);

case Permissions.B:
	return nameof(Permissions.B);

case Permissions.C:
	return nameof(Permissions.C);

case Permissions.All:
	return nameof(Permissions.All);


    }

    

    
    // FLAGS ENABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    string? flagResult = null;
    Byte checkedMaskCurrent = (Byte)e;
    if((checkedMaskCurrent & 7) == 7) { flagResult = flagResult is null ? nameof(Permissions.All) : nameof(Permissions.All) + ", " + flagResult; checkedMaskCurrent -= 7; }
if((checkedMaskCurrent & 4) == 4) { flagResult = flagResult is null ? nameof(Permissions.C) : nameof(Permissions.C) + ", " + flagResult; checkedMaskCurrent -= 4; }
if((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(Permissions.B) : nameof(Permissions.B) + ", " + flagResult; checkedMaskCurrent -= 2; }
if((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(Permissions.A) : nameof(Permissions.A) + ", " + flagResult; checkedMaskCurrent -= 1; }

    if(checkedMaskCurrent != default)
        return ((Byte)e).ToString();

    return flagResult ?? ((Byte)e).ToString();

    
}
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return <c>EnumValue, EnumValue2</c>.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
private static string? ToStringFastInternal(this Permissions e, bool includeFlagNames = false)
{
    switch(e)
    {
        case Permissions.None:
	return nameof(Permissions.None);

case Permissions.A:
	return nameof(Permissions.A);

case Permissions.B:
	return nameof(Permissions.B);

case Permissions.C:
	return nameof(Permissions.C);

case Permissions.All:
	return nameof(Permissions.All);


    }

    

    
    // FLAGS ENABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    string? flagResult = null;
    Byte checkedMaskCurrent = (Byte)e;
    if((checkedMaskCurrent & 7) == 7) { flagResult = flagResult is null ? nameof(Permissions.All) : nameof(Permissions.All) + ", " + flagResult; checkedMaskCurrent -= 7; }
if((checkedMaskCurrent & 4) == 4) { flagResult = flagResult is null ? nameof(Permissions.C) : nameof(Permissions.C) + ", " + flagResult; checkedMaskCurrent -= 4; }
if((checkedMaskCurrent & 2) == 2) { flagResult = flagResult is null ? nameof(Permissions.B) : nameof(Permissions.B) + ", " + flagResult; checkedMaskCurrent -= 2; }
if((checkedMaskCurrent & 1) == 1) { flagResult = flagResult is null ? nameof(Permissions.A) : nameof(Permissions.A) + ", " + flagResult; checkedMaskCurrent -= 1; }

    if(checkedMaskCurrent != default)
        return ((Byte)e).ToString();

    return flagResult ?? ((Byte)e).ToString();

    
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

    public static string? ToStringFast(this Permissions e)
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
    public static bool TryParseFast(string value, bool ignoreCase, out Permissions result)
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
    public static Permissions ParseFast(string value, bool ignoreCase = false)
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
    public static Permissions ParseFast(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
    {
        successful = false;

        if (string.IsNullOrWhiteSpace(value))
        {
            if (throwOnFailure)
                throw new ArgumentException("Value can't be null or whitespace!", nameof(value));

            return default;
        }

        Byte localResult = 0;
        bool parsed = false;
        string subValue;
        string originalValue = value;
        char firstChar = value[0];

        if (char.IsWhiteSpace(firstChar))
            firstChar = value.TrimStart()[0];

        if (char.IsDigit(firstChar) || firstChar == '-' || firstChar == '+')
        {
            if(Byte.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out var valueNumber))
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
                    case nameof(Permissions.A):
	parsed = true;
	localResult |= 1;
	break;

case nameof(Permissions.B):
	parsed = true;
	localResult |= 2;
	break;

case nameof(Permissions.C):
	parsed = true;
	localResult |= 4;
	break;

case nameof(Permissions.All):
	parsed = true;
	localResult |= 7;
	break;

case nameof(Permissions.None):
	parsed = true;
	localResult |= 0;
	break;


                }
            }
            else
            {
                if(subValue.Equals(nameof(Permissions.A), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 1; }
if(subValue.Equals(nameof(Permissions.B), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 2; }
if(subValue.Equals(nameof(Permissions.C), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 4; }
if(subValue.Equals(nameof(Permissions.All), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 7; }
if(subValue.Equals(nameof(Permissions.None), StringComparison.OrdinalIgnoreCase)) {
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

        return (Permissions)localResult;
    }


                }
            }

            #nullable restore