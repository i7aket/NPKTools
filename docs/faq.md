# Questions the numbers raise

Most of these are not bugs. They are the calculator telling you something about your water, your shelf
or your bag that is easy to mistake for a fault — and each answer says which.

If something here turns out to be a real fault, the fastest bug report is **the link the app puts in the
address bar**: it carries the whole setup, so the problem is reproducible exactly.

## "No recipe for this combination"

Two different failures wear the same words, and the message tells you which one you have.

**"Nothing on the shelf supplies X."** Nothing you ticked contains that element, so a target asking for
it cannot be met. Tick a source. Worth knowing how literal this is not: the element is *reported*, not
enforced — a shelf with no magnesium will still solve a target whose magnesium is zero — so the message
appears when the search found nothing and there is also a gap worth naming.

**"The selected salts cannot reach this target."** Everything asked for is available somewhere on the
shelf, but no mix lands inside tolerance. Two causes, in order of likelihood. The target may be
internally impossible with those salts: potassium nitrate carries nitrogen with its potassium, so high
potassium with low nitrogen needs a potassium source that is not a nitrate — add potassium sulfate. Or
the mix is over-constrained: every non-zero element is solved as an exact equality by default, and a
short shelf has too few degrees of freedom to hit six numbers at once. A longer shelf is the practical
answer; a library caller can loosen `RangeFactor` instead.

## The tank has more calcium (or nitrogen) than I asked for

Your water already supplied it. **Fertilizer only adds** — there is no salt that removes calcium from
water — so once the water brings 160 ppm of calcium and the target is 140, no recipe can reach the
target and the app says so instead of pretending. Raise the target, dilute the water with rain or
osmosis, or accept it.

The same happens with an acid: nitric acid carries nitrogen, and at a high alkalinity the dose alone can
overshoot a modest nitrogen target. The acid card names an acid that would fit when one does, which is
usually phosphoric or sulfuric depending on which element you have room for.

## The predicted EC does not match my meter

Several honest reasons, in the order they are likely:

- **A TDS meter is not an EC meter.** It measures conductivity and multiplies by a factor its
  manufacturer chose — 500 or 700 are the common ones, and Turkish irrigation literature uses 640.
  Two meters can disagree by 40% with both being right. The app shows `TDS ×500` so you know which
  convention it applied.
- **ppm of an element is not the ppm on a TDS meter.** The app's nutrient figures are milligrams of that
  element per litre. A TDS reading is a conductivity proxy for everything dissolved. They are different
  quantities and should not be compared.
- **You acidified.** The recipe's EC still counts the bicarbonate the acid removed, so after acidifying
  the prediction reads high — by roughly 44.5 µS/cm per meq/L neutralised. The acid card says so with
  the figure for your water.
- **The solution is stronger than the model was checked against.** The EC model is validated against
  certified KCl standards up to the ionic strength of a normal feed; a concentrate at 1:100 is an order
  of magnitude past that. When you are outside the validated range the app says *EC out of range* and
  the figure should be read as an ordering, not a measurement.
- **Calibration and temperature.** A meter reads what it was last calibrated to believe.

## My bag says 46% K₂O. What do I type?

Divide first. Labels quote nutrients as **oxides**, and in the EU, Spain, Poland and Turkey that is
required by law rather than being a convention — so it is not only phosphorus and potassium:

| On the bag | Divide by | To get |
|---|---|---|
| P₂O₅ | 2.29 | P |
| K₂O | 1.20 | K |
| CaO | 1.40 | Ca |
| MgO | 1.66 | Mg |
| SO₃ | 2.5 | S |

A 46% K₂O potassium nitrate is 38% K. Typing 46 overstates potassium by a fifth; on phosphorus the same
mistake is a factor of 2.29.

Better still, use the **formula** tab when you can and let the app derive the percentages — that is what
it is for, and it cannot make this mistake.

## The formula tab says my salt "looks like a chelate"

Because it does, and a formula cannot tell the app what matters. Iron EDTA — `C10H12N2O8FeNa` — parses
fine and yields 7.6% nitrogen, but that nitrogen is holding the iron, not feeding the plant. Entered by formula, the
salt would offer nitrogen it does not have, and the optimizer would count it. Describe a chelate by
percentages instead, where the agent is named and the nitrogen is not claimed.

## "Tank B is at 136% of saturation"

The salts in that tank together need more water than the tank holds. Each one alone might dissolve; they
compete for the same water. Use a larger concentrate volume — a 1:50 concentrate instead of 1:100 — or
move a salt to the other tank if the pairing allows it.

The related warning, **"Tank B needs 20.3 g/L of X, which dissolves to 18 g/L"**, is about one salt
rather than the tank: that salt cannot dissolve at that strength at 20 °C whatever else is in there.
Calcium monobasic phosphate is the usual culprit at 18 g/L, the lowest figure in the table.

## Why does it insist on two tanks?

Calcium and sulfate make gypsum, and calcium and phosphate make a precipitate too. At working strength
they stay in solution; at concentrate strength they do not. So calcium goes in tank A and sulfates and
phosphates in tank B, and the app warns if your own salt would put them together.

A single salt that contains both internally — monocalcium phosphate — is *not* flagged, because it is a
soluble compound rather than two reagents that happen to be adjacent. A check that fired on that would
train you to ignore it.

## The acid dose is less than my alkalinity. Is that a rounding error?

No, it is the carbonate equilibrium. Neutralising the *whole* alkalinity would take the water to a pH
below where you want it; at pH 5.8 about three quarters of the alkalinity needs neutralising, not all of
it. The common rule of thumb overstates the dose by roughly a quarter.

One caveat the app states rather than hides: it is worked for a **closed** vessel. An open reservoir
loses CO₂ and drifts back up, so the dose you settle on in practice will be nearer the full figure.

## Which pH do I enter?

**Water pH** is the pH of the source water, and it is the one place a pH reading carries real
information: it locates the water on its titration curve, which is what turns an alkalinity into a dose.
It says nothing about composition, which is why the app asks for it nowhere else.

**Target pH** is where you want the reservoir. Between 5.5 and 6.2 for most things in soilless growing.

## I typed 1,5 and it read 1.5. Or: why is everything shown with a dot?

Deliberate, in both directions. A comma is accepted as a decimal point because half of Europe types one.
Display is always a dot, because the app is built without ICU on purpose: a culture-aware parser can
decide a comma means thousands and read **1,5 grams as 15**, which is a tenfold weighing error. One
unambiguous output form is worth more than local familiarity when the number is going to be read aloud,
photographed and pasted across borders.

## There is no acidification card

Your water has no alkalinity to neutralise — which is the case on reverse osmosis, distilled or rain
water, where there is nothing to decide. Switch the water to EC or Analysis mode and describe real water
and the card appears.

(If you are on an older build and the card never appears at all, that was a bug: the panel was not being
redrawn. Fixed.)

## Does any of this leave my browser?

No. There is no server, no account and no telemetry; the whole calculation runs in the page. That is
also why the app can be used offline once loaded, and why "sync" is a link you copy rather than a service
you log into.

## Why is the link so long?

It carries the whole setup — the target, the water, the acid, the ticked salts, your own salts — in the
address itself, so pasting it into another browser reproduces exactly what you were looking at. Nothing
was uploaded anywhere to make that work.

If a link was made against a different version of the catalogue, the app applies everything it can, says
that the salt selection was ignored, and ticks everything rather than silently dropping salts.

## The estimated analysis is not an analysis

It says so in the heading. Given a meter reading, the app scales a typical water profile until the
computed conductivity matches your meter — so the proportions between ions come from the profile you
chose, not from your water. If you enter a hardness drop test as well, those measurements are pinned and
only what is left over is scaled. It is a much better starting point than assuming pure water, and it is
not a laboratory result.

If the readings cannot describe the same water — a hardness that already accounts for more conductivity
than the meter read — the app says that too, rather than producing a profile that is arithmetic rather
than water.

## Building and running it

- **.NET 10 SDK.** `global.json` pins `10.0.100` with `rollForward: latestFeature`; a 9.x SDK will not do.
- The solution is **`SYT.NPKTools.slnx`**. Bare `dotnet build` and `dotnet test` find it.
- **A published app served from a plain file server shows nothing.** The WebAssembly runtime needs
  `application/wasm` on the `.wasm` files and a fallback to `index.html` for deep links.
  `scripts/serve.py` does both; `python3 -m http.server` does neither.
- **`dotnet publish -o <dir>` does not clean the directory.** Stale assemblies accumulate and you debug a
  build that is not the one you think. Remove it first.

For the conventions, the test harness and how an interface change is verified, see
[CONTRIBUTING](../CONTRIBUTING.md). For the shape of the code, [architecture](architecture.md).
