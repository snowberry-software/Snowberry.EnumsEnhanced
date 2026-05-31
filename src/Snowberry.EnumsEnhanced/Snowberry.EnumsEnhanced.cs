using System.Collections;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Snowberry.EnumsEnhanced;

[Generator]
internal class EnumsEnhanced : IIncrementalGenerator
{
    /// <summary>
    /// Tracking name applied to the incremental transform step, used to inspect caching behavior
    /// (for example from incrementality tests via <see cref="GeneratorRunResult.TrackedSteps"/>).
    /// </summary>
    public const string c_TrackingName = "EnumsToGenerate";

    /// <summary>
    /// Diagnostic reported when the underlying type of an enumeration cannot be resolved.
    /// </summary>
    public static readonly DiagnosticDescriptor s_UnderlyingEnumerationTypeNotFound
      = new("EE001",
            "Underlying Enumeration Type not found",
            "The underlying type of the enumeration '{0}' could not be resolved",
            nameof(EnumsEnhanced),
            DiagnosticSeverity.Error,
            true);

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarationsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is EnumDeclarationSyntax,
            static (n, _) => TransformEnum(n))
            .Where(static m => m is not null)
            .WithTrackingName(c_TrackingName);

        context.RegisterSourceOutput(enumDeclarationsProvider, static (sourceProductionContext, model) =>
        {
            var enumToGenerate = model!.Value;

            if (!enumToGenerate.HasUnderlyingType)
            {
                sourceProductionContext.ReportDiagnostic(
                    Diagnostic.Create(s_UnderlyingEnumerationTypeNotFound,
                    Location.None,
                    enumToGenerate.Name));

                return;
            }

            var sb = new StringBuilder();

            GenerateEnumMethods(enumToGenerate, sb);

            string classCode = GetClassTemplate(enumToGenerate, out string hintName)
                .Replace("{CLASS_BODY}", sb.ToString());

            sourceProductionContext.AddSource(hintName, classCode.Trim());
        });
    }

    /// <summary>
    /// Builds the value-equatable model for an enum declaration so the incremental pipeline can cache it.
    /// </summary>
    /// <param name="context">The syntax context for the candidate <see cref="EnumDeclarationSyntax"/>.</param>
    /// <returns>
    /// The model describing the enum, or <see langword="null"/> for nested enums (which are intentionally skipped).
    /// </returns>
    private static EnumToGenerate? TransformEnum(GeneratorSyntaxContext context)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol symbol)
            return null;

        // Nested enums are not supported: skip them silently.
        if (symbol.ContainingType != null)
            return null;

        string accessModifier = AccessibilityToAccessModifier(symbol.DeclaredAccessibility);
        string @namespace = symbol.ContainingNamespace.ToDisplayString();

        var enumUnderlyingType = symbol.EnumUnderlyingType;

        // Defensive guard only: a valid EnumDeclarationSyntax always has a non-null underlying type
        // (Roslyn defaults it to Int32), so this branch is effectively unreachable.
        if (enumUnderlyingType == null)
        {
            return new EnumToGenerate(
                symbol.Name,
                @namespace,
                accessModifier,
                underlyingTypeName: string.Empty,
                isFlags: false,
                hasUnderlyingType: false,
                members: new EquatableArray<EnumMember>(System.Array.Empty<EnumMember>()));
        }

        bool isFlags = symbol
            .GetAttributes()
            .Any(a => SymbolEqualityComparer.Default.Equals(
                a.AttributeClass,
                context.SemanticModel.Compilation.GetTypeByMetadataName("System.FlagsAttribute")));

        var members = symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Select(static f => new EnumMember(f.Name, f.HasConstantValue, f.HasConstantValue ? f.ConstantValue : null))
            .ToArray();

        return new EnumToGenerate(
            symbol.Name,
            @namespace,
            accessModifier,
            enumUnderlyingType.Name,
            isFlags,
            hasUnderlyingType: true,
            new EquatableArray<EnumMember>(members));

        static string AccessibilityToAccessModifier(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Internal or Accessibility.Private => "internal",
                _ => "public"
            };
        }
    }

    /// <summary>
    /// Emits the body (all extension methods) for a single enum into <paramref name="methodSb"/>.
    /// </summary>
    /// <param name="e">The model describing the enum to generate methods for.</param>
    /// <param name="methodSb">The builder that receives the generated class body.</param>
    private static void GenerateEnumMethods(EnumToGenerate e, StringBuilder methodSb)
    {
        string enumName = e.Name;
        string underlyingName = e.UnderlyingTypeName;
        bool isFlags = e.IsFlags;
        var memberSymbols = e.Members;

        var methodImplAttributeText = new StringBuilder();
        methodImplAttributeText.AppendLine("#if NETCOREAPP3_0_OR_GREATER");
        //methodImplAttributeText.AppendLine($"[{nameof(MethodImplAttribute)}({nameof(MethodImplOptions)}.{nameof(MethodImplOptions.AggressiveInlining)} | {nameof(MethodImplOptions)}.AggressiveOptimization)]");
        methodImplAttributeText.AppendLine("#else");
        methodImplAttributeText.AppendLine($"[{nameof(MethodImplAttribute)}({nameof(MethodImplOptions)}.{nameof(MethodImplOptions.AggressiveInlining)})]");
        methodImplAttributeText.AppendLine("#endif");

        const string hasFlagMethodName = "HasFlagFast";

        // HasFlagFast
        {
            methodSb.AppendLine($$"""

                /// <summary>
                /// Determines whether one or more bit fields are set in the current instance.
                /// </summary>
                /// <param name="e">The value of the enum.</param>
                /// <param name="flag">The flag to check.</param>
                /// <returns><see langword="true"/> if the bit field or bit fields that are set in <paramref name="flag"/> are also set in <paramref name="e"/>; otherwise, <see langword="false"/>.</returns>
                {{methodImplAttributeText}}
                public static bool {{hasFlagMethodName}}(this {{enumName}} e, {{enumName}} flag)
                {
            #if NETCOREAPP3_0_OR_GREATER
                    {{underlyingName}} flagsValue = Unsafe.As<{{enumName}}, {{underlyingName}}>(ref flag);
                    return (Unsafe.As<{{enumName}}, {{underlyingName}}>(ref e) & flagsValue) == flagsValue;
            #else
                    return (({{underlyingName}})e & ({{underlyingName}})flag) == ({{underlyingName}})flag;
            #endif
                }
            """);
        }

        // GetNamesFast
        const string getNamesMethodName = "GetNamesFast";
        methodSb.AppendLine($$"""

            /// <summary>
            /// Retrieves an array of the names of the constants.
            /// </summary>
            /// <returns>A string array of the names of the constants.</returns>
            public static string[] {{getNamesMethodName}}()
            {
                return new string[] {
                    {{string.Join(", ", memberSymbols.Select(x => $"\"{x.Name}\""))}}
                };
            }

        """);

        // GetValuesFast
        const string getValuesMethodName = "GetValuesFast";
        methodSb.AppendLine($$"""

            /// <summary>
            /// Retrieves an array of the values of the constants.
            /// </summary>
            /// <returns>An array that contains the values of the constants.</returns>
            public static {{enumName}}[] {{getValuesMethodName}}()
            {
                return new {{enumName}}[] {
                    {{string.Join(", ", memberSymbols.Select(x => $"{enumName}.{x.Name}"))}}
                };
            }

        """);

        // IsDefinedFast
        var constantValuesChecked = new List<object>();
        var switchCases = new StringBuilder();
        {
            const string isDefinedMethodName = "IsDefinedFast";

            var switchCasesValue = new StringBuilder();

            foreach (var member in memberSymbols.OrderBy(x => x.Name.Length))
            {
                string memberRef = $"{enumName}.{member.Name}";

                switchCases.AppendLine($"case \"{member.Name}\":");
                switchCases.AppendLine("\treturn true;");

                if (!member.HasConstantValue)
                    continue;

                if (constantValuesChecked.Contains(member.ConstantValue!))
                {
                    string skipText = $"// Skipping duplicated constant value: {memberRef} -> {member.ConstantValue}";
                    switchCasesValue.AppendLine(skipText);
                    switchCasesValue.AppendLine();
                    continue;
                }

                switchCasesValue.AppendLine($"case {memberRef}:");
                switchCasesValue.AppendLine("\treturn true;");

                constantValuesChecked.Add(member.ConstantValue!);
            }

            constantValuesChecked.Clear();

            methodSb.AppendLine($$"""

                /// <inheritdoc cref="{{isDefinedMethodName}}({{enumName}})"/>
                /// <param name="value">The name of the enumeration constant.</param>
                /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
                public static bool {{isDefinedMethodName}}(string value)
                {
                    _ = value ?? throw new ArgumentNullException(nameof(value));

                    switch(value)
                    {
                        {{switchCases}}
                    }

                    return false;
                }

                /// <inheritdoc cref="{{isDefinedMethodName}}({{enumName}})"/>
                {{methodImplAttributeText}}
                public static bool {{isDefinedMethodName}}({{underlyingName}} value)
                {
                    return {{isDefinedMethodName}}(({{enumName}})value);
                }

                /// <summary>
                /// Returns a <see cref="bool"/> telling whether its given value exists in the enumeration.
                /// </summary>
                /// <param name="value">The value of the enumeration constant.</param>
                /// <returns><see langword="true"/> if a constant is defined with the given value from the <paramref name="value"/>.</returns>
                public static bool {{isDefinedMethodName}}({{enumName}} value)
                {
                    switch(value)
                    {
                        {{switchCasesValue}}
                    }

                    return false;
                }
            """);
        }

        // GetNameFast
        switchCases.Clear();
        {
            const string getNameMethodName = "GetNameFast";
            const string toStringMethodName = "ToStringFast";

            var flagCases = new StringBuilder();
            var flagCasesToString = new StringBuilder();
            var switchCasesToString = new StringBuilder();

            var groupedByValue = memberSymbols
                .Where(x => x.HasConstantValue)
                .GroupBy(x => x.ConstantValue);

            foreach (var group in groupedByValue)
            {
                var memberFirst = group.First();
                var memberLast = isFlags ? group.Last() : group.First();

                // GetNameFast uses FIRST name (matches Enum.GetName behavior)
                string memberRefFirst = $"{enumName}.{memberFirst.Name}";
                switchCases.AppendLine($"case {memberRefFirst}:");
                switchCases.AppendLine($"\treturn nameof({memberRefFirst});");
                switchCases.AppendLine();

                // ToStringFast uses LAST name (matches Enum.ToString behavior)
                string memberRefLast = $"{enumName}.{memberLast.Name}";
                switchCasesToString.AppendLine($"case {memberRefLast}:");
                switchCasesToString.AppendLine($"\treturn nameof({memberRefLast});");
                switchCasesToString.AppendLine();
            }

            var sortedByConstantValueDescGrouped = memberSymbols
                .Where(x => x.HasConstantValue)
                .OrderByDescending(x => x.ConstantValue)
                .GroupBy(x => x.ConstantValue);

            const string c_ToStringFastInternal = "ToStringFastInternal";
            GenerateFlagCases(flagCases, enumName, sortedByConstantValueDescGrouped, useFirstName: true);
            GenerateFlagCases(flagCasesToString, enumName, sortedByConstantValueDescGrouped, useFirstName: false);

            AppendGetNameMethod(methodSb, "public", enumName, underlyingName, getNameMethodName, switchCases, flagCases, isFlags);
            AppendGetNameMethod(methodSb, "private", enumName, underlyingName, c_ToStringFastInternal, switchCasesToString, flagCasesToString, isFlags);

            methodSb.AppendLine($$"""

                /// <summary>
                /// Converts the value of this instance to its equivalent string representation.
                /// </summary>
                /// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
                /// <returns>The string representation of the value of this instance.</returns>
                {{methodImplAttributeText}}
                public static string? {{toStringMethodName}}(this {{enumName}} e)
                {
                    return {{c_ToStringFastInternal}}(e, true);
                }
            """);

            static void AppendGetNameMethod(
                StringBuilder sb,
                string accessModifier,
                string enumName,
                string underlyingName,
                string methodName,
                StringBuilder switchCases,
                StringBuilder flagCases,
                bool writeFlags)
            {
                const string c_CheckedMaskNameCurrent = "checkedMaskCurrent";

                sb.AppendLine($$"""
                    /// <summary>
                    /// Resolves the name of the given enum value.
                    /// </summary>
                    /// <param name="e">The value of a particular enumerated constant in terms of its underlying type.</param>
                    /// <param name="includeFlagNames">Determines whether the value has flags, so it will return <c>EnumValue, EnumValue2</c>.</param>
                    /// <returns> A string containing the name of the enumerated constant or <see langword="null"/> if the enum has multiple flags set but <paramref name="includeFlagNames"/> is not enabled.</returns>
                    {{accessModifier}} static string? {{methodName}}(this {{enumName}} e, bool includeFlagNames = false)
                    {
                        switch(e)
                        {
                            {{switchCases}}
                        }

                        {{(writeFlags ? "" : $"return (({underlyingName})e).ToString();")}}

                        {{(!writeFlags ? "/*" : "")}}
                        // FLAGS {{(writeFlags ? "ENABLED" : "DISABLED")}}
                        // Returning null is the default behavior.
                        if(!includeFlagNames)
                            return null;
                            //throw new Exception("Enum name could not be found!");


                        string? flagResult = null;
                        {{underlyingName}} {{c_CheckedMaskNameCurrent}} = ({{underlyingName}})e;
                        {{flagCases}}
                        if({{c_CheckedMaskNameCurrent}} != default)
                            return (({{underlyingName}})e).ToString();

                        return flagResult ?? (({{underlyingName}})e).ToString();

                        {{(!writeFlags ? "*/" : "")}}
                    }
                    """);
            }

            // Emits the greedy flag-decomposition. Flags are matched largest-value-first (so named
            // composites are consumed greedily) and each matched name is prepended onto a string with a
            // separator only between entries. This yields BCL-identical ascending output while avoiding a
            // StringBuilder, a trailing Trim, and any runtime name lookups (names are baked in as nameof
            // literals). For the common 1-2 flag case this is a single allocation and beats the BCL.
            static void GenerateFlagCases(
                StringBuilder flagCasesBuilder,
                string enumName,
                IEnumerable<IGrouping<object?, EnumMember>> fields,
                bool useFirstName)
            {
                const string c_CheckedMaskNameCurrent = "checkedMaskCurrent";
                const string c_FlagResultName = "flagResult";

                bool foundZero = false;
                foreach (var group in fields)
                {
                    object? value = group.Key;

                    // The zero value never participates in flag composition.
                    if (!foundZero && value?.ToString() == "0")
                    {
                        foundZero = true;
                        continue;
                    }

                    // GetNameFast uses the first name, ToStringFast uses the last (alias) name.
                    var member = useFirstName ? group.First() : group.Last();
                    string nameOfRef = $"nameof({enumName}.{member.Name})";

                    flagCasesBuilder.AppendLine(@$"if(({c_CheckedMaskNameCurrent} & {value}) == {value}) {{ {c_FlagResultName} = {c_FlagResultName} is null ? {nameOfRef} : {nameOfRef} + "", "" + {c_FlagResultName}; {c_CheckedMaskNameCurrent} -= {value}; }}");
                }
            }
        }

        // ParseFast
        {
            const string parseMethodName = "ParseFast";
            const string tryParseMethodName = "TryParseFast";
            switchCases.Clear();

            var ifCases = new StringBuilder();

            constantValuesChecked.Clear();

            foreach (var member in memberSymbols.OrderBy(x => x.Name.Length))
            {
                if (!member.HasConstantValue)
                    continue;

                if (member.ConstantValue == null)
                    continue;

                string memberRef = $"{enumName}.{member.Name}";

                string constantValueString = member.ConstantValue is IConvertible convertible ? convertible.ToString(CultureInfo.InvariantCulture) : member.ConstantValue.ToString();

                switchCases.AppendLine($"case nameof({memberRef}):");
                switchCases.AppendLine($"\tparsed = true;");
                switchCases.AppendLine($"\tlocalResult |= {constantValueString};");
                switchCases.AppendLine($"\tbreak;");
                switchCases.AppendLine();

                ifCases.AppendLine($"if(subValue.Equals(nameof({memberRef}), {nameof(StringComparison)}.{nameof(StringComparison.OrdinalIgnoreCase)})) {{");
                ifCases.AppendLine($"\tparsed = true;");
                ifCases.AppendLine($"\tlocalResult |= {constantValueString}; }}");
            }

            methodSb.AppendLine($$"""

                /// <summary>
                /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
                /// </summary>
                /// <param name="value">A string containing the name or value to convert.</param>
                /// <param name="ignoreCase"><see langword="true"/> to ignore case; <see langword="false"/> to regard case.</param>
                /// <param name="result">The result of the enumeration constant.</param>
                /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
                public static bool {{tryParseMethodName}}(string value, bool ignoreCase, out {{enumName}} result)
                {
                    result = {{parseMethodName}}(out var successful, value: value, ignoreCase: ignoreCase, throwOnFailure: false);
                    return successful;
                }

                /// <summary>
                /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
                /// </summary>
                /// <param name="value">A string containing the name or value to convert.</param>
                /// <param name="ignoreCase"><see langword="true"/> to ignore case; <see langword="false"/> to regard case.</param>
                /// <returns>The enumeration value whose value is represented by the given value.</returns>
                /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/>, empty, whitespace, or cannot be converted to a defined value.</exception>
                public static {{enumName}} {{parseMethodName}}(string value, bool ignoreCase = false)
                {
                    return {{parseMethodName}}(out _, value: value, ignoreCase: ignoreCase, throwOnFailure: true);
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
                public static {{enumName}} {{parseMethodName}}(out bool successful, string value, bool ignoreCase = false, bool throwOnFailure = true)
                {
                    successful = false;

                    if (string.{{nameof(string.IsNullOrWhiteSpace)}}(value))
                    {
                        if (throwOnFailure)
                            throw new {{nameof(ArgumentException)}}("Value can't be null or whitespace!", nameof(value));

                        return default;
                    }

                    {{underlyingName}} localResult = 0;
                    bool parsed = false;
                    string subValue;
                    string originalValue = value;
                    char firstChar = value[0];

                    if (char.{{nameof(char.IsWhiteSpace)}}(firstChar))
                        firstChar = value.TrimStart()[0];

                    if (char.{{nameof(char.IsDigit)}}(firstChar) || firstChar == '-' || firstChar == '+')
                    {
                        if({{underlyingName}}.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out var valueNumber))
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
                                {{switchCases}}
                            }
                        }
                        else
                        {
                            {{ifCases}}
                        }

                        if(!parsed)
                            break;
                    }

                    successful = true;

                    if (!parsed)
                    {
                        successful = false;

                        if (throwOnFailure)
                            throw new {{nameof(ArgumentException)}}($"Could not convert the given value `{originalValue}`.", nameof(value));
                    }

                    return ({{enumName}})localResult;
                }

            """);
        }
    }

    /// <summary>
    /// Produces the surrounding class template (namespace, usings, partial class) with a
    /// <c>{CLASS_BODY}</c> placeholder for the generated members.
    /// </summary>
    /// <param name="e">The model describing the enum to generate the wrapper class for.</param>
    /// <param name="hintName">The unique source hint name passed to <see cref="SourceProductionContext.AddSource(string, string)"/>.</param>
    /// <returns>The class template containing a <c>{CLASS_BODY}</c> placeholder.</returns>
    private static string GetClassTemplate(EnumToGenerate e, out string hintName)
    {
        string className = $"{e.Name}Enhanced";

        // Include a sanitized namespace discriminator so same-named enums in different
        // namespaces don't collide on AddSource.
        hintName = $"{SanitizeHintName(e.Namespace)}.{className}.g.cs";

        return @$"

            #nullable enable

            using {typeof(StringBuilder).Namespace};
            using {typeof(Unsafe).Namespace};
            using {typeof(IEnumerable<>).Namespace};
            using {typeof(ArgumentNullException).Namespace};
            using {typeof(StringComparison).Namespace};
            using {typeof(NumberStyles).Namespace};

            namespace {e.Namespace}
            {{
                /// <summary>
                /// Reflection free extension methods for the <see cref=""{e.Name}""/> type.
                /// </summary>
                {e.AccessModifier} static partial class {className}
                {{
                    {{CLASS_BODY}}
                }}
            }}

            #nullable restore
        ";

        static string SanitizeHintName(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
                sb.Append(char.IsLetterOrDigit(c) || c == '.' || c == '_' ? c : '_');

            return sb.Length == 0 ? "global" : sb.ToString();
        }
    }
}

/// <summary>
/// Value-equatable model describing an enum to generate extension methods for.
/// </summary>
internal readonly struct EnumToGenerate : IEquatable<EnumToGenerate>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToGenerate"/> struct.
    /// </summary>
    /// <param name="name">The simple name of the enum type.</param>
    /// <param name="namespace">The display string of the enum's containing namespace.</param>
    /// <param name="accessModifier">The access modifier to emit for the generated class (<c>public</c> or <c>internal</c>).</param>
    /// <param name="underlyingTypeName">The name of the enum's underlying integral type.</param>
    /// <param name="isFlags">Whether the enum is annotated with <see cref="FlagsAttribute"/>.</param>
    /// <param name="hasUnderlyingType">Whether the underlying type was successfully resolved.</param>
    /// <param name="members">The enum's members.</param>
    public EnumToGenerate(
        string name,
        string @namespace,
        string accessModifier,
        string underlyingTypeName,
        bool isFlags,
        bool hasUnderlyingType,
        EquatableArray<EnumMember> members)
    {
        Name = name;
        Namespace = @namespace;
        AccessModifier = accessModifier;
        UnderlyingTypeName = underlyingTypeName;
        IsFlags = isFlags;
        HasUnderlyingType = hasUnderlyingType;
        Members = members;
    }

    /// <summary>Gets the simple name of the enum type.</summary>
    public string Name { get; }

    /// <summary>Gets the display string of the enum's containing namespace.</summary>
    public string Namespace { get; }

    /// <summary>Gets the access modifier emitted for the generated class.</summary>
    public string AccessModifier { get; }

    /// <summary>Gets the name of the enum's underlying integral type.</summary>
    public string UnderlyingTypeName { get; }

    /// <summary>Gets a value indicating whether the enum is annotated with <see cref="FlagsAttribute"/>.</summary>
    public bool IsFlags { get; }

    /// <summary>Gets a value indicating whether the underlying type was successfully resolved.</summary>
    public bool HasUnderlyingType { get; }

    /// <summary>Gets the enum's members.</summary>
    public EquatableArray<EnumMember> Members { get; }

    /// <inheritdoc/>
    public bool Equals(EnumToGenerate other)
    {
        return Name == other.Name
            && Namespace == other.Namespace
            && AccessModifier == other.AccessModifier
            && UnderlyingTypeName == other.UnderlyingTypeName
            && IsFlags == other.IsFlags
            && HasUnderlyingType == other.HasUnderlyingType
            && Members.Equals(other.Members);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EnumToGenerate other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Name?.GetHashCode() ?? 0);
            hash = hash * 31 + (Namespace?.GetHashCode() ?? 0);
            hash = hash * 31 + (AccessModifier?.GetHashCode() ?? 0);
            hash = hash * 31 + (UnderlyingTypeName?.GetHashCode() ?? 0);
            hash = hash * 31 + IsFlags.GetHashCode();
            hash = hash * 31 + HasUnderlyingType.GetHashCode();
            hash = hash * 31 + Members.GetHashCode();
            return hash;
        }
    }
}

/// <summary>
/// Value-equatable model describing a single enum member.
/// </summary>
internal readonly struct EnumMember : IEquatable<EnumMember>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMember"/> struct.
    /// </summary>
    /// <param name="name">The member's name.</param>
    /// <param name="hasConstantValue">Whether the member has a resolved constant value.</param>
    /// <param name="constantValue">The boxed constant value, or <see langword="null"/> when unresolved.</param>
    public EnumMember(string name, bool hasConstantValue, object? constantValue)
    {
        Name = name;
        HasConstantValue = hasConstantValue;
        ConstantValue = constantValue;
    }

    /// <summary>Gets the member's name.</summary>
    public string Name { get; }

    /// <summary>Gets a value indicating whether the member has a resolved constant value.</summary>
    public bool HasConstantValue { get; }

    /// <summary>
    /// Gets the boxed constant value of the member in its original primitive runtime type, or
    /// <see langword="null"/> when the member has no resolved constant value.
    /// </summary>
    public object? ConstantValue { get; }

    /// <inheritdoc/>
    public bool Equals(EnumMember other)
    {
        return Name == other.Name
            && HasConstantValue == other.HasConstantValue
            && Equals(ConstantValue, other.ConstantValue);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EnumMember other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Name?.GetHashCode() ?? 0);
            hash = hash * 31 + HasConstantValue.GetHashCode();
            hash = hash * 31 + (ConstantValue?.GetHashCode() ?? 0);
            return hash;
        }
    }
}

/// <summary>
/// An immutable array wrapper that implements structural (element-by-element) equality, unlike
/// <see cref="ImmutableArray{T}"/> which compares by reference.
/// </summary>
/// <typeparam name="T">The element type, which must itself be value-equatable.</typeparam>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly T[]? _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableArray{T}"/> struct.
    /// </summary>
    /// <param name="array">The backing array to wrap.</param>
    public EquatableArray(T[] array) => _array = array;

    /// <summary>Gets the number of elements.</summary>
    public int Count => _array?.Length ?? 0;

    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="index">The zero-based element index.</param>
    /// <returns>The element at <paramref name="index"/>.</returns>
    public T this[int index] => _array![index];

    /// <inheritdoc/>
    public bool Equals(EquatableArray<T> other)
    {
        if (_array is null)
            return other._array is null;

        if (other._array is null)
            return false;

        if (_array.Length != other._array.Length)
            return false;

        for (int i = 0; i < _array.Length; i++)
        {
            if (!_array[i].Equals(other._array[i]))
                return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_array is null)
            return 0;

        unchecked
        {
            int hash = 17;
            foreach (var item in _array)
                hash = hash * 31 + (item?.GetHashCode() ?? 0);

            return hash;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
        => ((IEnumerable<T>)(_array ?? System.Array.Empty<T>())).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
