# NPKTools.Optimizer.PpmTargetParser

Part of the [NPKTools](https://github.com/i7aket/NPKTools) suite. Turns a compact text description of
a nutrient profile into a `PpmTarget` the rest of the suite can consume — useful for CLI arguments,
config files and web form input.

Targets **.NET 10**.

## Format

`element=value` pairs separated by spaces, commas, or both. Element names are case-insensitive.

```
N=150 P=50 K=200 Ca=100 Mg=50 L=100
n=150, p=50, k=200
```

| Accepted | Meaning |
| --- | --- |
| `N` `P` `K` `Ca` `Mg` `S` | Macronutrients, ppm |
| `Fe` `Cu` `Mn` `Zn` `B` `Mo` `Cl` `Si` `Se` `Na` | Micronutrients, ppm |
| `L` | Water volume in liters (defaults to `1` when omitted) |

Any element you leave out defaults to `0`. Values are always parsed with the invariant culture, so
`1.5` means one and a half no matter what the machine's regional settings are.

`Parse` throws `FormatException` for a malformed pair, an unrecognised element, or a duplicated
element, and `ArgumentException` when the input is null, empty or whitespace.

## Setup

`ServiceCollection` and `BuildServiceProvider()` come from
**Microsoft.Extensions.DependencyInjection** — this package references only the abstractions, so in a
console app add the implementation too (`dotnet add package Microsoft.Extensions.DependencyInjection`).

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer.PpmTargetParser;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsPpmTargetParser()
    .BuildServiceProvider();

IPpmTargetParser parser = provider.GetRequiredService<IPpmTargetParser>();
```

Or just `new PpmTargetParser()` — it is stateless.

## Example

```csharp
using NPKTools.Core.Domain.PpmTarget;

try
{
    PpmTarget target = parser.Parse("N=150, P=50, K=200, Ca=40, Mg=30, L=100");

    Console.WriteLine($"N {target.N.Value} ppm in {target.Liters.Value} L");
}
catch (FormatException ex)
{
    // "The element 'Xx' is not recognized as a valid input."
    // "Unable to parse 'N' as an element=value pair."
    // "Duplicate element 'N' found in input."
    Console.WriteLine($"Bad target: {ex.Message}");
}
```

## Fixed in 2.0.0

`Na` was missing from the accepted-element list even though `PpmTarget` exposes a sodium target, so
`"Na=5"` threw `FormatException` and the sodium target was always zero. Sodium now parses correctly.
See the [changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md).

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
