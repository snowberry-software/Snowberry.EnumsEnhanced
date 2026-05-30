//HintName: Sample.FileAccessEnhanced.g.cs
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
                /// Reflection free extension methods for the <see cref="FileAccess"/> type.
                /// </summary>
                public static partial class FileAccessEnhanced
                {
                    
    private static readonly char[] s_flagTrimChars = new char[] { ',', ' ' };

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

    public static bool HasFlagFast(this FileAccess e, FileAccess flag)
    {
#if NETCOREAPP3_0_OR_GREATER
        Int32 flagsValue = Unsafe.As<FileAccess, Int32>(ref flag);
        return (Unsafe.As<FileAccess, Int32>(ref e) & flagsValue) == flagsValue;
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
            "None", "Read", "Write", "ReadAlias", "ReadWrite"
        };
    }


    /// <summary>
    /// Retrieves an array of the values of the constants.
    /// </summary>
    /// <returns>An array that contains the values of the constants.</returns>
    public static FileAccess[] GetValuesFast()
    {
        return new FileAccess[] {
            FileAccess.None, FileAccess.Read, FileAccess.Write, FileAccess.ReadAlias, FileAccess.ReadWrite
        };
    }


    /// <inheritdoc cref="IsDefinedFast(FileAccess)"/>
    public static bool IsDefinedFast(string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        switch(value)
        {
            case "None":
	return true;
case "Read":
	return true;
case "Write":
	return true;
case "ReadAlias":
	return true;
case "ReadWrite":
	return true;

        }

        return false;
    }

    /// <inheritdoc cref="IsDefinedFast(FileAccess)"/>
    #if NETCOREAPP3_0_OR_GREATER
#else
[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
#endif

    public static bool IsDefinedFast(Int32 value)
    {
        return IsDefinedFast((FileAccess)value);
    }

    /// <summary>
    /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
    /// </summary>
    /// <param name="value">The value of the enumeration constant.</param>
    /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
    public static bool IsDefinedFast(FileAccess value)
    {
        switch(value)
        {
            case FileAccess.None:
	return true;
case FileAccess.Read:
	return true;
case FileAccess.Write:
	return true;
// Skipping duplicated constant value: FileAccess.ReadAlias -> 1

case FileAccess.ReadWrite:
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
public static string? GetNameFast(this FileAccess e, bool includeFlagNames = false)
{
    switch(e)
    {
        case FileAccess.None:
	return nameof(FileAccess.None);

case FileAccess.Read:
	return nameof(FileAccess.Read);

case FileAccess.Write:
	return nameof(FileAccess.Write);

case FileAccess.ReadWrite:
	return nameof(FileAccess.ReadWrite);


    }

    

    
    // FLAGS ENABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    var flagBuilder = new StringBuilder();
    Int32 checkedMaskCurrent = (Int32)e;
if((checkedMaskCurrent & 3) == 3) {
	flagBuilder.Insert(0, FileAccess.ReadWrite.GetNameFast(false)).Insert(0, ", ");
	checkedMaskCurrent -= 3; }

if((checkedMaskCurrent & 2) == 2) {
	flagBuilder.Insert(0, FileAccess.Write.GetNameFast(false)).Insert(0, ", ");
	checkedMaskCurrent -= 2; }

if((checkedMaskCurrent & 1) == 1) {
	flagBuilder.Insert(0, FileAccess.ReadAlias.GetNameFast(false)).Insert(0, ", ");
	checkedMaskCurrent -= 1; }


    if(checkedMaskCurrent != default)
        return ((Int32)e).ToString();

    return flagBuilder.ToString().Trim(s_flagTrimChars);

    
}
/// <summary>
/// Resolves the name of the given enum value.
/// </summary>
/// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
/// <param name="includeFlagNames">Determines whether the value has flags, so it will return `EnumValue, EnumValue2`.</param>
/// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
private static string? ToStringFastInternal(this FileAccess e, bool includeFlagNames = false)
{
    switch(e)
    {
        case FileAccess.None:
	return nameof(FileAccess.None);

case FileAccess.ReadAlias:
	return nameof(FileAccess.ReadAlias);

case FileAccess.Write:
	return nameof(FileAccess.Write);

case FileAccess.ReadWrite:
	return nameof(FileAccess.ReadWrite);


    }

    

    
    // FLAGS ENABLED
    // Returning null is the default behavior.
    if(!includeFlagNames)
        return null;
        //throw new Exception("Enum name could not be found!");


    var flagBuilder = new StringBuilder();
    Int32 checkedMaskCurrent = (Int32)e;
if((checkedMaskCurrent & 3) == 3) {
	flagBuilder.Insert(0, FileAccess.ReadWrite.ToStringFastInternal(false)).Insert(0, ", ");
	checkedMaskCurrent -= 3; }

if((checkedMaskCurrent & 2) == 2) {
	flagBuilder.Insert(0, FileAccess.Write.ToStringFastInternal(false)).Insert(0, ", ");
	checkedMaskCurrent -= 2; }

if((checkedMaskCurrent & 1) == 1) {
	flagBuilder.Insert(0, FileAccess.ReadAlias.ToStringFastInternal(false)).Insert(0, ", ");
	checkedMaskCurrent -= 1; }


    if(checkedMaskCurrent != default)
        return ((Int32)e).ToString();

    return flagBuilder.ToString().Trim(s_flagTrimChars);

    
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

    public static string? ToStringFast(this FileAccess e)
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
    public static bool TryParseFast(string value, bool ignoreCase, out FileAccess result)
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
    public static FileAccess ParseFast(string value, bool ignoreCase = false)
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
    public static FileAccess ParseFast(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
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
                    case nameof(FileAccess.None):
	parsed = true;
	localResult |= 0;
	break;

case nameof(FileAccess.Read):
	parsed = true;
	localResult |= 1;
	break;

case nameof(FileAccess.Write):
	parsed = true;
	localResult |= 2;
	break;

case nameof(FileAccess.ReadAlias):
	parsed = true;
	localResult |= 1;
	break;

case nameof(FileAccess.ReadWrite):
	parsed = true;
	localResult |= 3;
	break;


                }
            }
            else
            {
                if(subValue.Equals(nameof(FileAccess.None), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 0; }
if(subValue.Equals(nameof(FileAccess.Read), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 1; }
if(subValue.Equals(nameof(FileAccess.Write), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 2; }
if(subValue.Equals(nameof(FileAccess.ReadAlias), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 1; }
if(subValue.Equals(nameof(FileAccess.ReadWrite), StringComparison.OrdinalIgnoreCase)) {
	parsed = true;
	localResult |= 3; }

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

        return (FileAccess)localResult;
    }


                }
            }

            #nullable restore