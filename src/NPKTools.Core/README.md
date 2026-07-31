# NPKTools.Core

The foundation of the [NPKTools](https://github.com/i7aket/NPKTools) suite. It contains the domain
model every other NPKTools package builds on, and has no dependencies of its own.

Targets **.NET 10**.

## What's inside

| Type | Purpose |
| --- | --- |
| `Fertilizer` | A fertilizer's identity (name, formula, concentrate type) and its full nutrient composition |
| `Ppm` | Measured nutrient concentrations in parts per million, for a given water volume |
| `PpmTarget` | The nutrient concentrations an optimization should hit |
| `SolutionFinderSettings` | Per-element precision and the global range factor that bound the search |
| `Solution` / `Solutions` | An optimizer result — fertilizers with weights plus the water volume — and a set of such results |

Each nutrient is a small value object (`FertilizerNitrogen`, `IronPpm`, `CalciumPpmTarget`, …) that
validates its own range on construction, so a negative nutrient value cannot be represented.
Nitrogen is split into nitrate, ammonium and amine; iron and the other chelatable elements track
each chelation form (EDTA, DTPA, EDDHA, HBED) separately.

## Building domain objects

The domain types are immutable and have no parameterless constructor. Use the builders, which reject
setting the same property twice:

```csharp
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.Fertilizers.Enums;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;

Fertilizer calciumNitrate = new FertilizerBuilder()
    .AddName("Calcium Nitrate Tetrahydrate")
    .AddFormula("Ca(NO₃)2*4H₂O")
    .AddType(ConcentrateType.A)
    .AddNo3(11.863)
    .AddCaNonChelated(16.972)
    .Build();

PpmTarget target = new PpmTargetBuilder()
    .AddN(150)
    .AddP(50)
    .AddK(200)
    .Build();
```

`PpmBuilder` and `SolutionFinderSettingsBuilder` work the same way.

## Reports

`Fertilizer.Report()`, `Fertilizer.GetNutrientSummary()` and `Ppm.Report()` render human-readable
output listing only the non-zero nutrients. All three use the invariant culture, so output does not
change with the machine's regional settings.

```csharp
Console.WriteLine(calciumNitrate.GetNutrientSummary()); // N 11.86 | Ca 16.97
```

## Breaking changes in 2.0.0

The parameterless constructors and property setters on `Fertilizer`, `FertilizerAttributes`, `Ppm`,
`PpmTarget` and `SolutionFinderSettings` are gone, `Solution`/`Solutions` no longer derive from
`List<T>`, and the `NPKTools.Core.Const` namespace is now `NPKTools.Core.Constants`. See the
[changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md) for the full migration table.

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
