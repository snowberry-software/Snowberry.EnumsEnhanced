# Snowberry.EnumsEnhanced

[![License](https://img.shields.io/github/license/snowberry-software/Snowberry.EnumsEnhanced)](LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/Snowberry.EnumsEnhanced.svg?logo=nuget)](https://www.nuget.org/packages/Snowberry.EnumsEnhanced/)
[![CI](https://github.com/snowberry-software/Snowberry.EnumsEnhanced/actions/workflows/dotnet.yml/badge.svg)](https://github.com/snowberry-software/Snowberry.EnumsEnhanced/actions/workflows/dotnet.yml)

**Snowberry.EnumsEnhanced** is a C# [incremental source generator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md) that writes reflection-free versions of the `System.Enum` methods for every enum in your project. The generated `ToStringFast`, `ParseFast`, `IsDefinedFast` and related methods are plain `switch` statements and constants, so they return the same results as `System.Enum` without reflection or boxing.

- Works in any project the .NET SDK 8.0.100 or later builds: .NET, .NET Framework, and .NET Standard, from C# 7.3 up.
- Ships as an analyzer only: no runtime assembly and no dependencies end up in your output.
- Up to 128x faster than `System.Enum` on .NET Framework 4.8 and up to 12x faster on .NET 11 (see [Benchmarks](#benchmarks)).

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Generated Methods](#generated-methods)
- [Top Features at a Glance](#top-features-at-a-glance)
- [How It Works](#how-it-works)
- [Requirements](#requirements)
- [Diagnostics](#diagnostics)
- [Pitfalls](#pitfalls)
- [Benchmarks](#benchmarks)
- [Changelog](#changelog)
- [License](#license)

## Installation

```bash
dotnet add package Snowberry.EnumsEnhanced
```

The package is a development dependency, so NuGet adds it with `PrivateAssets="all"` and it does not flow to projects that reference yours.

## Quick Start

1. Declare an enum at namespace level (here `Permissions.cs`):

   ```csharp
   namespace MyApp;

   [Flags]
   public enum Permissions
   {
       None = 0,
       Read = 1,
       Write = 2,
       Delete = 4,
       ReadWrite = Read | Write,
   }
   ```

2. Build. The generator adds a static class named after the enum with an `Enhanced` suffix, `PermissionsEnhanced`, in the same namespace.

3. Call the generated methods: extension methods on the value, static methods on the class.

   ```csharp
   using MyApp;

   var permissions = Permissions.Read | Permissions.Delete;

   Console.WriteLine(permissions.ToStringFast());                    // Read, Delete
   Console.WriteLine(permissions.HasFlagFast(Permissions.Delete));   // True
   Console.WriteLine(Permissions.ReadWrite.GetNameFast());           // ReadWrite

   Permissions parsed = PermissionsEnhanced.ParseFast("Read, Write"); // Permissions.ReadWrite
   Console.WriteLine(parsed);

   if (PermissionsEnhanced.TryParseFast("delete", ignoreCase: true, out var delete))
       Console.WriteLine(delete);                                    // Delete

   Console.WriteLine(PermissionsEnhanced.IsDefinedFast("Execute"));  // False
   Console.WriteLine(string.Join(", ", PermissionsEnhanced.GetNamesFast())); // None, Read, Write, Delete, ReadWrite
   ```

## Generated Methods

For an enum `TEnum` with underlying type `TUnderlying`, the generator emits `TEnumEnhanced` with these members:

| Generated method | Replaces | Notes |
| --- | --- | --- |
| `e.ToStringFast()` | `e.ToString()` | Same output, including `"Read, Delete"` for flag combinations and the number for undefined values. |
| `e.GetNameFast(bool includeFlagNames = false)` | `Enum.GetName(typeof(TEnum), e)` | Returns `null` for values without a name, unless `includeFlagNames` is `true`. |
| `e.HasFlagFast(TEnum flag)` | `e.HasFlag(flag)` | Bitwise check on the underlying value. |
| `TEnumEnhanced.GetNamesFast()` | `Enum.GetNames(typeof(TEnum))` | Returns a new array on every call. |
| `TEnumEnhanced.GetValuesFast()` | `Enum.GetValues(typeof(TEnum))` | Returns a new, strongly typed array on every call. |
| `TEnumEnhanced.IsDefinedFast(string)` | `Enum.IsDefined(typeof(TEnum), name)` | Case-sensitive name match; throws `ArgumentNullException` for `null`. |
| `TEnumEnhanced.IsDefinedFast(TEnum)` and `IsDefinedFast(TUnderlying)` | `Enum.IsDefined(typeof(TEnum), value)` | |
| `TEnumEnhanced.ParseFast(string, bool ignoreCase = false)` | `Enum.Parse(typeof(TEnum), value, ignoreCase)` | Accepts names, comma-separated flag names, and numbers; throws `ArgumentException` on failure. |
| `TEnumEnhanced.TryParseFast(string, bool ignoreCase, out TEnum)` | `Enum.TryParse(value, ignoreCase, out result)` | |

The generated code is a static `partial` class with the enum's accessibility (`public` or `internal`). An excerpt for `Permissions`, reformatted and without the XML documentation comments:

```csharp
public static partial class PermissionsEnhanced
{
    public static string[] GetNamesFast()
    {
        return new string[] {
            "None", "Read", "Write", "Delete", "ReadWrite"
        };
    }

    public static bool IsDefinedFast(Permissions value)
    {
        switch (value)
        {
            case Permissions.None: return true;
            case Permissions.Read: return true;
            case Permissions.Write: return true;
            case Permissions.Delete: return true;
            case Permissions.ReadWrite: return true;
        }

        return false;
    }

    // HasFlagFast, GetValuesFast, GetNameFast, ToStringFast, ParseFast, TryParseFast ...
}
```

The complete output for several enum shapes is checked in as snapshot files under [src/Tests/Snowberry.EnumsEnhanced.Generator.Tests/Snapshots](src/Tests/Snowberry.EnumsEnhanced.Generator.Tests/Snapshots).

## Top Features at a Glance

- **Same results as `System.Enum`.** `ToStringFast` and `ParseFast` follow the BCL formatting and parsing rules, including flag decomposition, named composite values, and numeric strings; the test suite compares them value by value.
- **No allocations for names and checks.** `ToStringFast`, `GetNameFast`, `HasFlagFast` and `IsDefinedFast` return constants or compare values; only undefined values and flag combinations without a single name build a string.
- **Every underlying type.** `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long` and `ulong` enums, including negative values and `ulong.MaxValue`.
- **Incremental and cached.** The generator works on a value-equatable model of each enum, so editing one enum or an unrelated file does not regenerate the others.
- **Warning-free output.** The generated code compiles without warnings at the highest warning level, with or without nullable reference types, down to C# 7.3.

## How It Works

The generator runs inside the C# compiler during every build and in the IDE as you type:

```text
enum declaration          incremental generator              generated source
(namespace level)   --->  reads names, values, [Flags]  ---> TEnumEnhanced.g.cs
Permissions               and the underlying type            switch statements and
                                                             constants, compiled into
                                                             your assembly
```

Nothing is looked up at run time: every name, value and flag combination is known at compile time and written into the generated methods. To inspect the output, set `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` in your project; the files appear under `obj/<configuration>/<framework>/generated/`.

## Requirements

| Requirement | Minimum |
| --- | --- |
| Compiler | .NET SDK 8.0.100 or Visual Studio 2022 17.8 (Roslyn 4.8) |
| Language version | C# 7.3; nullable annotations are emitted from C# 8 |
| Target framework | Any: the generator runs in the compiler, not in your application |

## Diagnostics

| Id | Severity | Meaning |
| --- | --- | --- |
| `EE001` | Error | The underlying type of an enum could not be resolved, so no code is generated for it. |

## Pitfalls

- **Nested enums are skipped.** An enum declared inside a class or struct gets no generated methods; move it to namespace level to use them.
- **`GetNameFast` returns `null` for flag combinations by default.** That matches `Enum.GetName`. Pass `includeFlagNames: true`, or use `ToStringFast`, to get `"Read, Delete"`.
- **`GetNamesFast` and `GetValuesFast` allocate.** Like `Enum.GetNames` and `Enum.GetValues`, they return a new array on every call. Cache the result on hot paths.
- **The `TEnumEnhanced` name is taken.** Declaring your own non-partial type with that name in the same namespace causes a compile error; add members by declaring `static partial class TEnumEnhanced` instead.
- **`HasFlagFast` is no faster on modern .NET.** Since .NET Core 2.1 the JIT turns `Enum.HasFlag` into the same bitwise check; the gain is on .NET Framework.

## Benchmarks

Measured with BenchmarkDotNet on an AMD Ryzen 9 7950X3D, comparing each generated method with the non-generic `System.Enum` call:

| Operation | .NET Framework 4.8 | .NET 10 | .NET 11 |
| --- | ---: | ---: | ---: |
| `ToString` | 128x | 4.3x | 4.5x |
| `GetValues` | 125x | 10x | 12x |
| `IsDefined` (value) | 77x | 6.9x | 6.3x |
| `GetName` | 55x | 8.4x | 8.3x |
| `Parse` (name) | 5.0x | 1.9x | 2.0x |

All operations, the full result tables, and the test environment are in [BENCHMARKS.md](BENCHMARKS.md).

## Changelog

Release notes for every version are in [CHANGELOG.txt](CHANGELOG.txt).

## License

Licensed under the [MIT License](LICENSE).

Copyright (c) 2026 Snowberry Software
