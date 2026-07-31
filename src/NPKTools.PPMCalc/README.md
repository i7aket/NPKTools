# NPKTools.PPMCalc

Part of the [NPKTools](https://github.com/i7aket/NPKTools) suite. Calculates the concentration of
each nutrient, in parts per million, for a collection of fertilizers dissolved in a given volume of
water.

Targets **.NET 10**. Fully managed — no native dependencies, so it also runs under WebAssembly.

> **Renamed in 2.0.0.** This package used to ship as `NPKTools.Optimizer.PPMCalc`. The id now
> matches the assembly and namespace (`NPKTools.PPMCalc`), as it does for the rest of the suite.

## What it computes

Nitrogen — broken out into nitrate (NO₃), ammonium (NH₄) and amine (NH₂) — plus phosphorus,
potassium, magnesium, sulfur, calcium, and the trace elements iron, copper, manganese, zinc, boron,
molybdenum, chlorine, silicon, selenium and sodium. The water volume is a parameter, so the same
fertilizer mix can be measured at any dilution.

## Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.PPMCalc;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsPpmCalc()
    .BuildServiceProvider();

IPpmCalculationService calculator = provider.GetRequiredService<IPpmCalculationService>();
```

Or just `new PpmCalculationService()` — it is stateless.

## Example

```csharp
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.PartsPerMillion;

// Weights are in grams; nutrient values are percentages by weight.
IReadOnlyList<Fertilizer> mix =
[
    new FertilizerBuilder()
        .AddName("Calcium Nitrate")
        .AddWeight(0.589)
        .AddNo3(11.863)
        .AddCaNonChelated(16.972)
        .Build(),

    new FertilizerBuilder()
        .AddName("Magnesium Sulfate")
        .AddWeight(0.325)
        .AddMgNonChelated(9.861)
        .AddS(13.008)
        .Build(),

    new FertilizerBuilder()
        .AddName("MKP")
        .AddWeight(0.22)
        .AddP(22.761)
        .AddK(28.731)
        .Build()
];

Ppm ppm = calculator.CalculatePpm(mix, waterLiters: 1);

Console.WriteLine(ppm.Report());
Console.WriteLine($"Nitrate only: {ppm.Nitrogen.Nitrate:F2} ppm");
```

`Ppm.Report()` lists every non-zero nutrient and always uses the invariant culture, so its output
does not shift with the machine's regional settings.

## Measuring an optimizer result

A `Solution` from `NPKTools.Optimizer` is an `IReadOnlyList<Fertilizer>`, so it can be passed
straight in, along with the water volume it was solved for:

```csharp
Ppm actual = calculator.CalculatePpm(solution, solution.WaterLiters);
```

This is the usual way to verify that an optimized mix really hits the target it was solved against.

## Breaking changes in 2.0.0

Besides the package rename, `CalculatePpm` now takes `IReadOnlyList<Fertilizer>` rather than
`IList<Fertilizer>`. See the
[changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md).

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
