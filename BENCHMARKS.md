# Benchmarks

Performance of the generated `*Fast` methods compared with the `System.Enum` APIs, measured with
[BenchmarkDotNet](https://benchmarkdotnet.org/) on .NET Framework 4.8, .NET 10, and .NET 11.

How to run them and how the benchmark project is laid out is described in
[src/Tests/Snowberry.EnumsEnhanced.Benchmark/BENCHMARKS.md](src/Tests/Snowberry.EnumsEnhanced.Benchmark/BENCHMARKS.md).

## Table of Contents

- [Summary](#summary)
- [Environment](#environment)
- [ToString](#tostring)
- [GetName](#getname)
- [GetNames](#getnames)
- [GetValues](#getvalues)
- [HasFlag](#hasflag)
- [IsDefined (name)](#isdefined-name)
- [IsDefined (value)](#isdefined-value)
- [Parse (name)](#parse-name)
- [Parse (value)](#parse-value)

## Summary

Speed-up of the generated method over the non-generic `System.Enum` call, from the geometric mean of the
parameter cases in each category. Higher is better; 1x means the same speed.

| Operation | .NET Framework 4.8 | .NET 10 | .NET 11 |
| --- | ---: | ---: | ---: |
| ToString | 128x | 4.3x | 4.5x |
| GetName | 55x | 8.4x | 8.3x |
| GetNames | 2.6x | 2.7x | 1.4x |
| GetValues | 125x | 10x | 12x |
| HasFlag | 15x | 0.95x | 1.1x |
| IsDefined (name) | 7.0x | 3.9x | 4.5x |
| IsDefined (value) | 77x | 6.9x | 6.3x |
| Parse (name) | 5.0x | 1.9x | 2.0x |
| Parse (name, ignore case) | 2.0x | 1.8x | 1.6x |
| Parse (value) | 2.9x | 1.4x | 1.7x |

- On .NET Framework 4.8 most operations are 55x to 128x faster and allocate nothing where `System.Enum` allocates.
- On .NET 10 and .NET 11 the JIT already optimizes `Enum.HasFlag`, so `HasFlagFast` is on par with it.
- `GetNamesFast` and `GetValuesFast` return a new array on every call, as `System.Enum` does, so both allocate.
- The parse benchmarks return `object`, so the 24 B they report for `ParseFast` is the boxing of the result.

## Environment

.NET 10 and .NET 11 run:

```text
BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9974/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 7950X3D 4.20GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 63.21 GB Total, 16.63 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GVKUBM : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  Job-IHFIKV : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
```

.NET Framework 4.8 run:

```text
BenchmarkDotNet v0.16.0-preview.2, Windows .9974
AMD Ryzen 9 7950X3D 4.20GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 63.21 GB Total, 34.76 GB Available
  [Host]     : .NET Framework 4.8.1 (4.8.9363.0), X64 RyuJIT VectorSize=256
  Job-UGENHH : .NET Framework 4.8.1 (4.8.9363.0), X64 RyuJIT VectorSize=256
```

## ToString

`ToStringFast()` vs `Enum.ToString()`.

### .NET 10 and .NET 11

| Method       | Runtime            | Value        | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------- |------------------- |------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **ToString_BCL** | **.NET 10.0**          | **Test1**        |  **9.634 ns** | **0.1900 ns** | **0.2470 ns** |  **9.606 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| ToStringFast | .NET 10.0          | Test1        |  1.651 ns | 0.0315 ns | 0.0324 ns |  1.654 ns |  0.17 |    0.01 |      - |         - |        0.00 |
|              |                    |              |           |           |           |           |       |         |        |           |             |
| ToString_BCL | .NET 11.0          | Test1        | 12.612 ns | 0.4625 ns | 1.3417 ns | 12.682 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| ToStringFast | .NET 11.0          | Test1        |  1.955 ns | 0.0386 ns | 0.0697 ns |  1.949 ns |  0.16 |    0.02 |      - |         - |        0.00 |
|              |                    |              |           |           |           |           |       |         |        |           |             |
| **ToString_BCL** | **.NET 10.0**          | **Test4, Test7** | **23.908 ns** | **0.4752 ns** | **1.2848 ns** | **23.974 ns** |  **1.00** |    **0.00** | **0.0014** |      **72 B** |        **1.00** |
| ToStringFast | .NET 10.0          | Test4, Test7 |  7.710 ns | 0.1542 ns | 0.4522 ns |  7.661 ns |  0.32 |    0.03 | 0.0010 |      48 B |        0.67 |
|              |                    |              |           |           |           |           |       |         |        |           |             |
| ToString_BCL | .NET 11.0          | Test4, Test7 | 19.517 ns | 0.3837 ns | 0.7664 ns | 19.469 ns |  1.00 |    0.00 | 0.0014 |      72 B |        1.00 |
| ToStringFast | .NET 11.0          | Test4, Test7 |  6.103 ns | 0.1205 ns | 0.3278 ns |  5.993 ns |  0.31 |    0.02 | 0.0010 |      48 B |        0.67 |

### .NET Framework 4.8

| Method       | Value        | Mean       | Error      | StdDev     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------- |------------- |-----------:|-----------:|-----------:|------:|-------:|-------:|----------:|------------:|
| **ToString_BCL** | **Test1**        | **865.295 ns** | **17.1841 ns** | **21.7324 ns** | **1.000** | **0.0534** | **0.0010** |     **337 B** |        **1.00** |
| ToStringFast | Test1        |   2.517 ns |  0.0486 ns |  0.1264 ns | 0.003 |      - |      - |         - |        0.00 |
|              |              |            |            |            |       |        |        |           |             |
| **ToString_BCL** | **Test4, Test7** | **890.308 ns** | **17.5127 ns** | **34.1572 ns** |  **1.00** | **0.0553** | **0.0010** |     **353 B** |        **1.00** |
| ToStringFast | Test4, Test7 |  18.831 ns |  0.3737 ns |  0.9976 ns |  0.02 | 0.0089 | 0.0000 |      56 B |        0.16 |

## GetName

`GetNameFast()` vs `Enum.GetName(Type, object)` and `Enum.GetName<T>()`.

### .NET 10 and .NET 11

| Method          | Runtime            | NameValue | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------- |------------------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **GetName**         | **.NET 10.0**          | **Test1**     | **21.197 ns** | **0.9305 ns** | **2.7437 ns** | **21.218 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| GetName_Generic | .NET 10.0          | Test1     |  5.219 ns | 0.1035 ns | 0.2520 ns |  5.221 ns |  0.25 |    0.04 |      - |         - |        0.00 |
| GetNameFast     | .NET 10.0          | Test1     |  2.582 ns | 0.0434 ns | 0.0406 ns |  2.582 ns |  0.12 |    0.02 |      - |         - |        0.00 |
|                 |                    |           |           |           |           |           |       |         |        |           |             |
| GetName         | .NET 11.0          | Test1     | 21.051 ns | 0.3621 ns | 0.4024 ns | 21.021 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| GetName_Generic | .NET 11.0          | Test1     |  3.559 ns | 0.1993 ns | 0.5876 ns |  3.577 ns |  0.17 |    0.03 |      - |         - |        0.00 |
| GetNameFast     | .NET 11.0          | Test1     |  2.211 ns | 0.0885 ns | 0.2608 ns |  2.163 ns |  0.11 |    0.01 |      - |         - |        0.00 |
|                 |                    |           |           |           |           |           |       |         |        |           |             |
| **GetName**         | **.NET 10.0**          | **Test6**     | **15.491 ns** | **0.4323 ns** | **1.2192 ns** | **15.027 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| GetName_Generic | .NET 10.0          | Test6     |  3.584 ns | 0.0635 ns | 0.1096 ns |  3.563 ns |  0.23 |    0.02 |      - |         - |        0.00 |
| GetNameFast     | .NET 10.0          | Test6     |  1.797 ns | 0.0283 ns | 0.0265 ns |  1.788 ns |  0.12 |    0.01 |      - |         - |        0.00 |
|                 |                    |           |           |           |           |           |       |         |        |           |             |
| GetName         | .NET 11.0          | Test6     | 13.214 ns | 0.2576 ns | 0.2410 ns | 13.138 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| GetName_Generic | .NET 11.0          | Test6     |  3.241 ns | 0.0524 ns | 0.0465 ns |  3.239 ns |  0.25 |    0.01 |      - |         - |        0.00 |
| GetNameFast     | .NET 11.0          | Test6     |  1.803 ns | 0.0216 ns | 0.0257 ns |  1.803 ns |  0.14 |    0.00 |      - |         - |        0.00 |

### .NET Framework 4.8

| Method      | NameValue | Mean       | Error     | StdDev     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------ |---------- |-----------:|----------:|-----------:|------:|-------:|-------:|----------:|------------:|
| **GetName**     | **Test1**     | **125.587 ns** | **8.6585 ns** | **25.5299 ns** |  **1.00** | **0.0076** | **0.0001** |      **48 B** |        **1.00** |
| GetNameFast | Test1     |   1.787 ns | 0.0319 ns |  0.0533 ns |  0.01 |      - |      - |         - |        0.00 |
|             |           |            |           |            |       |        |        |           |             |
| **GetName**     | **Test6**     |  **83.723 ns** | **1.3276 ns** |  **1.2418 ns** |  **1.00** | **0.0076** | **0.0001** |      **48 B** |        **1.00** |
| GetNameFast | Test6     |   1.946 ns | 0.0255 ns |  0.0239 ns |  0.02 |      - |      - |         - |        0.00 |

## GetNames

`GetNamesFast()` vs `Enum.GetNames(Type)` and `Enum.GetNames<T>()`.

### .NET 10 and .NET 11

| Method           | Runtime            | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------- |------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| GetNames         | .NET 10.0          | 13.777 ns | 1.4479 ns | 4.2693 ns | 12.060 ns |  1.00 |    0.00 | 0.0014 |      72 B |        1.00 |
| GetNames_Generic | .NET 10.0          | 11.915 ns | 0.1614 ns | 0.1348 ns | 11.886 ns |  0.95 |    0.29 | 0.0014 |      72 B |        1.00 |
| GetNamesFast     | .NET 10.0          |  5.141 ns | 0.2821 ns | 0.8048 ns |  4.917 ns |  0.41 |    0.14 | 0.0014 |      72 B |        1.00 |
|                  |                    |           |           |           |           |       |         |        |           |             |
| GetNames         | .NET 11.0          | 10.288 ns | 0.5345 ns | 1.5335 ns |  9.805 ns |  1.00 |    0.00 | 0.0014 |      72 B |        1.00 |
| GetNames_Generic | .NET 11.0          | 14.261 ns | 0.8510 ns | 2.5093 ns | 15.213 ns |  1.41 |    0.31 | 0.0014 |      72 B |        1.00 |
| GetNamesFast     | .NET 11.0          |  7.241 ns | 0.3439 ns | 1.0139 ns |  7.596 ns |  0.72 |    0.14 | 0.0014 |      72 B |        1.00 |

### .NET Framework 4.8

| Method       | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------- |---------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| GetNames     | 30.86 ns | 0.330 ns | 0.293 ns |  1.00 |    0.00 | 0.0114 | 0.0001 |      72 B |        1.00 |
| GetNamesFast | 12.06 ns | 0.313 ns | 0.861 ns |  0.39 |    0.03 | 0.0115 | 0.0000 |      72 B |        1.00 |

## GetValues

`GetValuesFast()` vs `Enum.GetValues(Type)` and `Enum.GetValues<T>()`.

### .NET 10 and .NET 11

| Method            | Runtime            | Mean      | Error     | StdDev     | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------ |------------------- |----------:|----------:|-----------:|----------:|------:|--------:|-------:|----------:|------------:|
| GetValues         | .NET 10.0          | 51.426 ns | 1.0164 ns |  1.6982 ns | 50.897 ns |  1.00 |    0.00 | 0.0008 |      40 B |        1.00 |
| GetValues_Generic | .NET 10.0          | 22.119 ns | 1.5521 ns |  4.5765 ns | 20.796 ns |  0.43 |    0.09 | 0.0008 |      40 B |        1.00 |
| GetValuesFast     | .NET 10.0          |  5.000 ns | 0.1777 ns |  0.5240 ns |  5.097 ns |  0.10 |    0.01 | 0.0008 |      40 B |        1.00 |
|                   |                    |           |           |            |           |       |         |        |           |             |
| GetValues         | .NET 11.0          | 70.020 ns | 3.8350 ns | 11.3077 ns | 72.156 ns |  1.00 |    0.00 | 0.0008 |      40 B |        1.00 |
| GetValues_Generic | .NET 11.0          | 28.192 ns | 0.4308 ns |  0.3819 ns | 28.210 ns |  0.41 |    0.08 | 0.0008 |      40 B |        1.00 |
| GetValuesFast     | .NET 11.0          |  5.634 ns | 0.0904 ns |  0.0801 ns |  5.645 ns |  0.08 |    0.02 | 0.0008 |      40 B |        1.00 |

### .NET Framework 4.8

| Method        | Mean       | Error      | StdDev      | Median     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |-----------:|-----------:|------------:|-----------:|------:|-------:|-------:|----------:|------------:|
| GetValues     | 561.750 ns | 35.3284 ns | 104.1667 ns | 526.200 ns | 1.000 | 0.0291 | 0.0005 |     185 B |        1.00 |
| GetValuesFast |   4.502 ns |  0.0873 ns |   0.1435 ns |   4.519 ns | 0.008 | 0.0064 | 0.0000 |      40 B |        0.22 |

## HasFlag

`HasFlagFast()` vs `Enum.HasFlag()`.

### .NET 10 and .NET 11

| Method      | Runtime            | TestEnumValue | TestEnumFlagCheck    | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------ |------------------- |-------------- |--------------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| HasFlag     | .NET 10.0          | Test4, Test7  | Test2(...)Test7 [26] | 1.1272 ns | 0.0261 ns | 0.0770 ns |  1.00 |    0.00 |         - |          NA |
| HasFlagFast | .NET 10.0          | Test4, Test7  | Test2(...)Test7 [26] | 1.1930 ns | 0.0168 ns | 0.0157 ns |  1.06 |    0.08 |         - |          NA |
|             |                    |               |                      |           |           |           |       |         |           |             |
| HasFlag     | .NET 11.0          | Test4, Test7  | Test2(...)Test7 [26] | 1.0202 ns | 0.0299 ns | 0.0881 ns |  1.00 |    0.00 |         - |          NA |
| HasFlagFast | .NET 11.0          | Test4, Test7  | Test2(...)Test7 [26] | 0.9640 ns | 0.0182 ns | 0.0426 ns |  0.95 |    0.09 |         - |          NA |

### .NET Framework 4.8

| Method      | TestEnumValue | TestEnumFlagCheck    | Mean      | Error     | StdDev    | Median    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------ |-------------- |--------------------- |----------:|----------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| HasFlag     | Test4, Test7  | Test2(...)Test7 [26] | 16.522 ns | 0.3690 ns | 1.0881 ns | 16.783 ns |  1.00 | 0.0076 | 0.0000 |      48 B |        1.00 |
| HasFlagFast | Test4, Test7  | Test2(...)Test7 [26] |  1.115 ns | 0.0222 ns | 0.0500 ns |  1.136 ns |  0.07 |      - |      - |         - |        0.00 |

## IsDefined (name)

`IsDefinedFast(string)` vs `Enum.IsDefined(Type, string)`.

### .NET 10 and .NET 11

| Method                   | Runtime            | NameValue | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |------------------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
| **IsDefined_StringName**     | **.NET 10.0**          | **Invalid**   | **10.495 ns** | **0.4482 ns** | **1.3215 ns** | **10.115 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | .NET 10.0          | Invalid   |  3.379 ns | 0.1648 ns | 0.4756 ns |  3.528 ns |  0.33 |    0.06 |         - |          NA |
|                          |                    |           |           |           |           |           |       |         |           |             |
| IsDefined_StringName     | .NET 11.0          | Invalid   | 11.356 ns | 0.2539 ns | 0.7486 ns | 11.556 ns |  1.00 |    0.00 |         - |          NA |
| IsDefinedFast_StringName | .NET 11.0          | Invalid   |  2.441 ns | 0.0485 ns | 0.0783 ns |  2.425 ns |  0.22 |    0.02 |         - |          NA |
|                          |                    |           |           |           |           |           |       |         |           |             |
| **IsDefined_StringName**     | **.NET 10.0**          | **Test1**     |  **6.796 ns** | **0.0915 ns** | **0.0764 ns** |  **6.787 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | .NET 10.0          | Test1     |  1.637 ns | 0.0220 ns | 0.0195 ns |  1.636 ns |  0.24 |    0.00 |         - |          NA |
|                          |                    |           |           |           |           |           |       |         |           |             |
| IsDefined_StringName     | .NET 11.0          | Test1     |  6.868 ns | 0.1231 ns | 0.2251 ns |  6.775 ns |  1.00 |    0.00 |         - |          NA |
| IsDefinedFast_StringName | .NET 11.0          | Test1     |  1.593 ns | 0.0143 ns | 0.0134 ns |  1.596 ns |  0.23 |    0.01 |         - |          NA |
|                          |                    |           |           |           |           |           |       |         |           |             |
| **IsDefined_StringName**     | **.NET 10.0**          | **Test6**     | **13.227 ns** | **0.1961 ns** | **0.1834 ns** | **13.212 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | .NET 10.0          | Test6     |  2.947 ns | 0.0435 ns | 0.0363 ns |  2.946 ns |  0.22 |    0.00 |         - |          NA |
|                          |                    |           |           |           |           |           |       |         |           |             |
| IsDefined_StringName     | .NET 11.0          | Test6     | 13.633 ns | 0.2622 ns | 0.3020 ns | 13.575 ns |  1.00 |    0.00 |         - |          NA |
| IsDefinedFast_StringName | .NET 11.0          | Test6     |  2.942 ns | 0.0564 ns | 0.0527 ns |  2.936 ns |  0.22 |    0.01 |         - |          NA |

### .NET Framework 4.8

| Method                   | NameValue | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |---------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
| **IsDefined_StringName**     | **Invalid**   | **52.035 ns** | **2.2801 ns** | **6.7229 ns** | **53.701 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | Invalid   |  7.915 ns | 0.1567 ns | 0.2346 ns |  7.965 ns |  0.15 |    0.02 |         - |          NA |
|                          |           |           |           |           |           |       |         |           |             |
| **IsDefined_StringName**     | **Test1**     | **44.182 ns** | **1.8768 ns** | **5.5337 ns** | **45.466 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | Test1     |  2.800 ns | 0.0227 ns | 0.0201 ns |  2.800 ns |  0.06 |    0.01 |         - |          NA |
|                          |           |           |           |           |           |       |         |           |             |
| **IsDefined_StringName**     | **Test6**     | **56.717 ns** | **3.1806 ns** | **9.3779 ns** | **59.346 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| IsDefinedFast_StringName | Test6     | 17.084 ns | 0.3327 ns | 0.4086 ns | 17.078 ns |  0.31 |    0.06 |         - |          NA |

## IsDefined (value)

`IsDefinedFast(value)` vs `Enum.IsDefined(Type, object)` and `Enum.IsDefined<T>()`.

### .NET 10 and .NET 11

| Method                  | Runtime            | EnumValue | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------ |------------------- |---------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **IsDefined_Value**         | **.NET 10.0**          | **Test1**     | **12.040 ns** | **0.1198 ns** | **0.1121 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| IsDefined_Value_Generic | .NET 10.0          | Test1     |  2.504 ns | 0.0289 ns | 0.0270 ns |  0.21 |    0.00 |      - |         - |        0.00 |
| IsDefinedFast_Value     | .NET 10.0          | Test1     |  1.771 ns | 0.0213 ns | 0.0200 ns |  0.15 |    0.00 |      - |         - |        0.00 |
|                         |                    |           |           |           |           |       |         |        |           |             |
| IsDefined_Value         | .NET 11.0          | Test1     | 11.673 ns | 0.1522 ns | 0.1424 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| IsDefined_Value_Generic | .NET 11.0          | Test1     |  2.360 ns | 0.0463 ns | 0.0433 ns |  0.20 |    0.00 |      - |         - |        0.00 |
| IsDefinedFast_Value     | .NET 11.0          | Test1     |  1.604 ns | 0.0238 ns | 0.0223 ns |  0.14 |    0.00 |      - |         - |        0.00 |
|                         |                    |           |           |           |           |       |         |        |           |             |
| **IsDefined_Value**         | **.NET 10.0**          | **Test7**     | **12.644 ns** | **0.2516 ns** | **0.4664 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| IsDefined_Value_Generic | .NET 10.0          | Test7     |  3.433 ns | 0.0393 ns | 0.0367 ns |  0.27 |    0.01 |      - |         - |        0.00 |
| IsDefinedFast_Value     | .NET 10.0          | Test7     |  1.802 ns | 0.0302 ns | 0.0497 ns |  0.14 |    0.01 |      - |         - |        0.00 |
|                         |                    |           |           |           |           |       |         |        |           |             |
| IsDefined_Value         | .NET 11.0          | Test7     | 12.214 ns | 0.1491 ns | 0.1245 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| IsDefined_Value_Generic | .NET 11.0          | Test7     |  5.329 ns | 0.1696 ns | 0.5001 ns |  0.44 |    0.04 |      - |         - |        0.00 |
| IsDefinedFast_Value     | .NET 11.0          | Test7     |  2.248 ns | 0.0314 ns | 0.0294 ns |  0.18 |    0.00 |      - |         - |        0.00 |

### .NET Framework 4.8

| Method              | EnumValue | Mean       | Error      | StdDev     | Median     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------------- |---------- |-----------:|-----------:|-----------:|-----------:|------:|-------:|-------:|----------:|------------:|
| **IsDefined_Value**     | **Test1**     | **153.983 ns** | **11.1352 ns** | **32.8325 ns** | **141.031 ns** |  **1.00** | **0.0076** | **0.0002** |      **48 B** |        **1.00** |
| IsDefinedFast_Value | Test1     |   2.303 ns |  0.0386 ns |  0.0361 ns |   2.308 ns |  0.02 |      - |      - |         - |        0.00 |
|                     |           |            |            |            |            |       |        |        |           |             |
| **IsDefined_Value**     | **Test7**     | **186.498 ns** |  **3.6998 ns** |  **4.6791 ns** | **186.106 ns** |  **1.00** | **0.0076** | **0.0002** |      **48 B** |        **1.00** |
| IsDefinedFast_Value | Test7     |   2.122 ns |  0.0253 ns |  0.0237 ns |   2.128 ns |  0.01 |      - |      - |         - |        0.00 |

## Parse (name)

`ParseFast(string)` vs `Enum.Parse(Type, string)` and `Enum.Parse<T>()`, case-sensitive and ignoring case.

### .NET 10 and .NET 11

| Method                       | Runtime            | NameValue | NameValueUpper | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------- |------------------- |---------- |--------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **Parse_Name**                   | **.NET 10.0**          | **Test1**     | **TEST1**          | **22.130 ns** | **1.4296 ns** | **4.2153 ns** | **23.263 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Name_Generic           | .NET 10.0          | Test1     | TEST1          | 19.231 ns | 0.3834 ns | 0.8962 ns | 18.913 ns |  0.91 |    0.21 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 10.0          | Test1     | TEST1          |  7.617 ns | 0.1344 ns | 0.2951 ns |  7.523 ns |  0.36 |    0.08 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 10.0          | Test1     | TEST1          | 14.760 ns | 0.1956 ns | 0.1830 ns | 14.763 ns |  0.70 |    0.16 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 10.0          | Test1     | TEST1          | 17.787 ns | 0.2914 ns | 0.2726 ns | 17.814 ns |  0.84 |    0.19 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 10.0          | Test1     | TEST1          | 10.352 ns | 0.2004 ns | 0.1874 ns | 10.350 ns |  0.49 |    0.11 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| Parse_Name                   | .NET 11.0          | Test1     | TEST1          | 13.313 ns | 0.2611 ns | 0.3745 ns | 13.283 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Name_Generic           | .NET 11.0          | Test1     | TEST1          |  7.806 ns | 0.1206 ns | 0.1128 ns |  7.764 ns |  0.59 |    0.02 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 11.0          | Test1     | TEST1          |  7.670 ns | 0.1497 ns | 0.2542 ns |  7.646 ns |  0.58 |    0.02 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 11.0          | Test1     | TEST1          | 13.755 ns | 0.2119 ns | 0.2355 ns | 13.806 ns |  1.03 |    0.03 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 11.0          | Test1     | TEST1          |  8.267 ns | 0.1396 ns | 0.1237 ns |  8.268 ns |  0.62 |    0.02 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 11.0          | Test1     | TEST1          | 10.836 ns | 0.2163 ns | 0.4063 ns | 10.790 ns |  0.81 |    0.04 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| **Parse_Name**                   | **.NET 10.0**          | **Test1**     | **TEST6**          | **13.808 ns** | **0.2481 ns** | **0.2200 ns** | **13.867 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Name_Generic           | .NET 10.0          | Test1     | TEST6          | 18.428 ns | 0.2732 ns | 0.2421 ns | 18.501 ns |  1.33 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 10.0          | Test1     | TEST6          |  7.785 ns | 0.1546 ns | 0.2266 ns |  7.696 ns |  0.56 |    0.02 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 10.0          | Test1     | TEST6          | 28.086 ns | 0.8355 ns | 2.3973 ns | 27.083 ns |  2.03 |    0.18 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 10.0          | Test1     | TEST6          | 37.648 ns | 2.2157 ns | 6.5331 ns | 39.039 ns |  2.73 |    0.47 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 10.0          | Test1     | TEST6          | 11.171 ns | 0.2757 ns | 0.7956 ns | 10.783 ns |  0.81 |    0.06 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| Parse_Name                   | .NET 11.0          | Test1     | TEST6          | 14.326 ns | 0.1865 ns | 0.1745 ns | 14.311 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Name_Generic           | .NET 11.0          | Test1     | TEST6          |  8.009 ns | 0.1025 ns | 0.0909 ns |  8.013 ns |  0.56 |    0.01 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 11.0          | Test1     | TEST6          |  7.534 ns | 0.1181 ns | 0.1105 ns |  7.479 ns |  0.53 |    0.01 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 11.0          | Test1     | TEST6          | 23.091 ns | 0.2422 ns | 0.2023 ns | 23.088 ns |  1.61 |    0.02 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 11.0          | Test1     | TEST6          | 16.600 ns | 0.3140 ns | 0.3224 ns | 16.581 ns |  1.16 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 11.0          | Test1     | TEST6          | 11.128 ns | 0.2206 ns | 0.5655 ns | 10.994 ns |  0.78 |    0.04 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| **Parse_Name**                   | **.NET 10.0**          | **Test6**     | **TEST1**          | **20.369 ns** | **0.6845 ns** | **1.9858 ns** | **19.903 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Name_Generic           | .NET 10.0          | Test6     | TEST1          | 34.638 ns | 0.8792 ns | 2.5924 ns | 34.712 ns |  1.72 |    0.20 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 10.0          | Test6     | TEST1          | 15.629 ns | 0.8317 ns | 2.4524 ns | 16.831 ns |  0.77 |    0.14 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 10.0          | Test6     | TEST1          | 15.250 ns | 0.3032 ns | 0.2368 ns | 15.232 ns |  0.76 |    0.07 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 10.0          | Test6     | TEST1          | 18.513 ns | 0.3676 ns | 0.3438 ns | 18.389 ns |  0.92 |    0.08 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 10.0          | Test6     | TEST1          | 10.764 ns | 0.2073 ns | 0.1939 ns | 10.736 ns |  0.53 |    0.05 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| Parse_Name                   | .NET 11.0          | Test6     | TEST1          | 18.654 ns | 0.3709 ns | 0.7056 ns | 18.531 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Name_Generic           | .NET 11.0          | Test6     | TEST1          | 12.593 ns | 0.2067 ns | 0.1933 ns | 12.610 ns |  0.68 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 11.0          | Test6     | TEST1          |  9.536 ns | 0.1557 ns | 0.1456 ns |  9.503 ns |  0.51 |    0.02 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 11.0          | Test6     | TEST1          | 14.401 ns | 0.3101 ns | 0.8695 ns | 14.088 ns |  0.77 |    0.05 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 11.0          | Test6     | TEST1          | 13.595 ns | 0.2703 ns | 0.7623 ns | 13.587 ns |  0.73 |    0.05 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 11.0          | Test6     | TEST1          | 13.269 ns | 0.9309 ns | 2.7447 ns | 12.888 ns |  0.71 |    0.15 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| **Parse_Name**                   | **.NET 10.0**          | **Test6**     | **TEST6**          | **18.421 ns** | **0.3593 ns** | **0.3360 ns** | **18.384 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Name_Generic           | .NET 10.0          | Test6     | TEST6          | 25.255 ns | 0.2795 ns | 0.2615 ns | 25.397 ns |  1.37 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 10.0          | Test6     | TEST6          |  9.174 ns | 0.0971 ns | 0.0908 ns |  9.187 ns |  0.50 |    0.01 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 10.0          | Test6     | TEST6          | 25.177 ns | 0.4089 ns | 0.3825 ns | 25.120 ns |  1.37 |    0.03 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 10.0          | Test6     | TEST6          | 32.337 ns | 1.4550 ns | 4.1746 ns | 30.363 ns |  1.76 |    0.23 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 10.0          | Test6     | TEST6          | 12.810 ns | 0.5302 ns | 1.5550 ns | 12.309 ns |  0.70 |    0.08 | 0.0005 |      24 B |        1.00 |
|                              |                    |           |                |           |           |           |           |       |         |        |           |             |
| Parse_Name                   | .NET 11.0          | Test6     | TEST6          | 24.291 ns | 1.7292 ns | 5.0985 ns | 22.763 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Name_Generic           | .NET 11.0          | Test6     | TEST6          | 16.545 ns | 1.7019 ns | 5.0181 ns | 13.559 ns |  0.71 |    0.25 | 0.0005 |      24 B |        1.00 |
| ParseFast_Name               | .NET 11.0          | Test6     | TEST6          | 10.027 ns | 0.1949 ns | 0.3465 ns |  9.931 ns |  0.43 |    0.08 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase         | .NET 11.0          | Test6     | TEST6          | 23.818 ns | 0.4709 ns | 0.4625 ns | 23.753 ns |  1.02 |    0.19 | 0.0005 |      24 B |        1.00 |
| Parse_NameIgnoreCase_Generic | .NET 11.0          | Test6     | TEST6          | 16.562 ns | 0.2504 ns | 0.2342 ns | 16.535 ns |  0.71 |    0.13 | 0.0005 |      24 B |        1.00 |
| ParseFast_NameIgnoreCase     | .NET 11.0          | Test6     | TEST6          | 10.375 ns | 0.1768 ns | 0.1654 ns | 10.331 ns |  0.44 |    0.08 | 0.0005 |      24 B |        1.00 |

### .NET Framework 4.8

| Method                   | NameValue | NameValueUpper | Mean      | Error    | StdDev    | Median    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------- |---------- |--------------- |----------:|---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **Parse_Name**               | **Test1**     | **TEST1**          | **156.77 ns** | **3.118 ns** |  **8.323 ns** | **159.22 ns** |  **1.00** |    **0.00** | **0.0204** | **0.0001** |     **128 B** |        **1.00** |
| ParseFast_Name           | Test1     | TEST1          |  26.70 ns | 0.693 ns |  2.043 ns |  26.32 ns |  0.17 |    0.02 | 0.0038 | 0.0000 |      24 B |        0.19 |
| Parse_NameIgnoreCase     | Test1     | TEST1          | 149.41 ns | 8.316 ns | 24.521 ns | 149.88 ns |  0.96 |    0.17 | 0.0203 | 0.0002 |     128 B |        1.00 |
| ParseFast_NameIgnoreCase | Test1     | TEST1          |  86.92 ns | 2.703 ns |  7.927 ns |  88.34 ns |  0.56 |    0.06 | 0.0038 | 0.0001 |      24 B |        0.19 |
|                          |           |                |           |          |           |           |       |         |        |        |           |             |
| **Parse_Name**               | **Test1**     | **TEST6**          | **139.97 ns** | **3.432 ns** | **10.120 ns** | **141.93 ns** |  **1.00** |    **0.00** | **0.0204** | **0.0001** |     **128 B** |        **1.00** |
| ParseFast_Name           | Test1     | TEST6          |  26.08 ns | 0.518 ns |  1.126 ns |  26.35 ns |  0.19 |    0.02 | 0.0038 | 0.0000 |      24 B |        0.19 |
| Parse_NameIgnoreCase     | Test1     | TEST6          | 200.71 ns | 5.971 ns | 17.606 ns | 204.09 ns |  1.44 |    0.17 | 0.0203 | 0.0002 |     128 B |        1.00 |
| ParseFast_NameIgnoreCase | Test1     | TEST6          |  86.80 ns | 3.219 ns |  9.493 ns |  89.22 ns |  0.62 |    0.08 | 0.0038 | 0.0001 |      24 B |        0.19 |
|                          |           |                |           |          |           |           |       |         |        |        |           |             |
| **Parse_Name**               | **Test6**     | **TEST1**          | **169.35 ns** | **4.370 ns** | **12.886 ns** | **172.52 ns** |  **1.00** |    **0.00** | **0.0204** | **0.0001** |     **128 B** |        **1.00** |
| ParseFast_Name           | Test6     | TEST1          |  38.13 ns | 0.739 ns |  0.961 ns |  38.13 ns |  0.23 |    0.02 | 0.0038 | 0.0000 |      24 B |        0.19 |
| Parse_NameIgnoreCase     | Test6     | TEST1          | 164.10 ns | 3.260 ns |  5.265 ns | 164.24 ns |  0.98 |    0.09 | 0.0204 | 0.0001 |     128 B |        1.00 |
| ParseFast_NameIgnoreCase | Test6     | TEST1          |  92.01 ns | 1.831 ns |  4.727 ns |  92.58 ns |  0.55 |    0.06 | 0.0038 | 0.0001 |      24 B |        0.19 |
|                          |           |                |           |          |           |           |       |         |        |        |           |             |
| **Parse_Name**               | **Test6**     | **TEST6**          | **181.19 ns** | **3.528 ns** |  **6.085 ns** | **182.91 ns** |  **1.00** |    **0.00** | **0.0204** | **0.0001** |     **128 B** |        **1.00** |
| ParseFast_Name           | Test6     | TEST6          |  40.81 ns | 0.777 ns |  0.894 ns |  40.76 ns |  0.23 |    0.01 | 0.0038 | 0.0000 |      24 B |        0.19 |
| Parse_NameIgnoreCase     | Test6     | TEST6          | 213.19 ns | 3.819 ns |  3.189 ns | 212.16 ns |  1.18 |    0.04 | 0.0203 | 0.0002 |     128 B |        1.00 |
| ParseFast_NameIgnoreCase | Test6     | TEST6          |  89.18 ns | 1.772 ns |  2.366 ns |  89.44 ns |  0.49 |    0.02 | 0.0038 | 0.0001 |      24 B |        0.19 |

## Parse (value)

`ParseFast(string)` with a numeric string vs `Enum.Parse(Type, string)` and `Enum.Parse<T>()`.

### .NET 10 and .NET 11

| Method              | Runtime            | Value | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |------------------- |------ |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **Parse_Value**         | **.NET 10.0**          | **1**     | **22.716 ns** | **0.2220 ns** | **0.1734 ns** | **22.759 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Value_Generic | .NET 10.0          | 1     | 16.548 ns | 0.3181 ns | 0.3124 ns | 16.503 ns |  0.73 |    0.01 | 0.0005 |      24 B |        1.00 |
| ParseFast_Value     | .NET 10.0          | 1     | 14.633 ns | 0.3807 ns | 1.1224 ns | 14.851 ns |  0.64 |    0.05 | 0.0005 |      24 B |        1.00 |
|                     |                    |       |           |           |           |           |       |         |        |           |             |
| Parse_Value         | .NET 11.0          | 1     | 20.543 ns | 0.4094 ns | 0.7382 ns | 20.748 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Value_Generic | .NET 11.0          | 1     | 16.887 ns | 0.3200 ns | 0.2994 ns | 16.971 ns |  0.82 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_Value     | .NET 11.0          | 1     | 12.295 ns | 0.7122 ns | 2.0999 ns | 12.055 ns |  0.60 |    0.10 | 0.0005 |      24 B |        1.00 |
|                     |                    |       |           |           |           |           |       |         |        |           |             |
| **Parse_Value**         | **.NET 10.0**          | **64**    | **18.396 ns** | **0.3139 ns** | **0.2936 ns** | **18.424 ns** |  **1.00** |    **0.00** | **0.0005** |      **24 B** |        **1.00** |
| Parse_Value_Generic | .NET 10.0          | 64    | 16.052 ns | 0.3093 ns | 0.4815 ns | 16.068 ns |  0.87 |    0.03 | 0.0005 |      24 B |        1.00 |
| ParseFast_Value     | .NET 10.0          | 64    | 14.161 ns | 1.2705 ns | 3.7462 ns | 11.944 ns |  0.77 |    0.20 | 0.0005 |      24 B |        1.00 |
|                     |                    |       |           |           |           |           |       |         |        |           |             |
| Parse_Value         | .NET 11.0          | 64    | 15.688 ns | 0.2358 ns | 0.2206 ns | 15.635 ns |  1.00 |    0.00 | 0.0005 |      24 B |        1.00 |
| Parse_Value_Generic | .NET 11.0          | 64    | 14.339 ns | 0.0887 ns | 0.0786 ns | 14.313 ns |  0.91 |    0.01 | 0.0005 |      24 B |        1.00 |
| ParseFast_Value     | .NET 11.0          | 64    |  8.902 ns | 0.1049 ns | 0.0930 ns |  8.870 ns |  0.57 |    0.01 | 0.0005 |      24 B |        1.00 |

### .NET Framework 4.8

| Method          | Value | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------- |------ |----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| **Parse_Value**     | **1**     | **207.88 ns** | **4.112 ns** | **6.401 ns** |  **1.00** |    **0.00** | **0.0114** | **0.0002** |      **72 B** |        **1.00** |
| ParseFast_Value | 1     |  68.75 ns | 1.288 ns | 1.205 ns |  0.33 |    0.01 | 0.0038 | 0.0001 |      24 B |        0.33 |
|                 |       |           |          |          |       |         |        |        |           |             |
| **Parse_Value**     | **64**    | **215.02 ns** | **4.273 ns** | **6.778 ns** |  **1.00** |    **0.00** | **0.0114** | **0.0002** |      **72 B** |        **1.00** |
| ParseFast_Value | 64    |  75.49 ns | 1.496 ns | 2.458 ns |  0.35 |    0.02 | 0.0038 | 0.0001 |      24 B |        0.33 |
