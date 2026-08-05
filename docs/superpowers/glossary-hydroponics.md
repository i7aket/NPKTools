# Hydroponics glossary for the eight interface languages

Date: 2026-08-03
Status: **Dutch, Spanish, Polish and Turkish checked against primary sources — the four I was least
sure of. Russian, Ukrainian and German are written with confidence but have not been externally
checked.** Two entries remain marked ⚠ and say why; everything else is quoted from a source.

The interface is translated against this file rather than string by string, so one concept cannot
become three different words in three places. Each entry says what the term *means*, not only what it
is called: a translator who knows the language but not the subject needs the definition more than the
word.

## How to read the confidence marks

| Mark | Meaning |
|---|---|
| — | I am confident; standard usage in that market's agronomic writing |
| ⚠ | plausible but unverified — **check this one against a fertilizer label or an agronomy source** |

I am materially more reliable in Russian, Ukrainian and German than in Dutch, Polish and Turkish. The
marks are honest about that rather than uniform.

## Water chemistry

**alkalinity** — the water's capacity to neutralise acid, carried almost entirely by bicarbonate. Not
"basicity" and not pH: a water can sit at pH 7.5 with almost no alkalinity, or at the same pH with a
great deal. This is the number that decides how much acid a reservoir needs.

**Checked, and it changed two entries.** Neither Dutch nor Spanish practice routes the acid dose
through a word meaning "alkalinity" — both name the ion. Dunea's statutory drinking-water report
prints `Waterstofcarbonaat` in mg/l and `Totale hardheid`, and the word *alkaliniteit* does not appear
in it at all; Flemish horticulture works in `bicarbonaat` mmol/l with thresholds at >0,5 and
>2–3 mmol/l. Spanish laboratories print `Bicarbonatos (CO₃H⁻)` in **meq/L** — IFAPA's own guide to
reading an irrigation analysis says *"los valores de las diferentes sales, vienen expresados en
miliequivalentes/litro"* — and one Spanish lab spells the equivalence out: *"También nos solemos
referir a estas especies como Alcalinidad."* So the word is right and the label is not: show both, or
a grower cannot map the field to the paper in their hand. `alcalinidad` **is** idiomatic in Latin
America, where Intagri titles articles with it, which the dual label also serves.

**Polish answers the question this glossary was opened to settle, and then makes it moot.**
`zasadowość` is the normative word: it is the title of the Polish national standard
PN-EN ISO 9963 — *"Oznaczanie zasadowości"*, in mmol/l — and the headword in the Polish
hydrogeological dictionary, which defines it as *"zdolność do zobojętniania silnych kwasów"*.
`alkaliczność` is listed there only as a synonym and in practice belongs to marine and boiler-water
engineering. So: **`zasadowość`, not `alkaliczność`.**

And then the same thing happens as in the other three languages. A Polish laboratory selling water
analysis to greenhouse growers prints *"Zawartość wodorowęglanów (HCO₃) wyrażona w miligramach na litr
oraz milimolach na litr"* and no alkalinity line at all; Instytut Ogrodnictwa's own fertilizer
recommendations contain neither word and work from HCO₃⁻ directly — *"Niekorzystny wpływ HCO₃⁻ zaczyna
się, gdy jego zawartość w wodzie wynosi powyżej 60-90 mg/l"* — and professor Treder states the acid
rule the same way: the bicarbonate figure is what the acid dose is computed from, leaving about
43 mg/l. Four languages, four times the same answer: **name the ion, gloss the quantity.**

Unit trap for anyone importing these reports: Dutch and Spanish figures arrive as mg/L HCO₃⁻ and must
be divided by 61 to reach the mmol/L growers work in. Dunea's 174 mg/l is 2,85 mmol/l — a water that
needs acid. mg/L CaCO₃ is US practice and appears in neither market's horticultural writing.

| | |
|---|---|
| ru | щёлочность |
| uk | лужність |
| de | Alkalinität |
| nl | bicarbonaat (HCO₃⁻) — *alkaliniteit* is understood but not what a report prints |
| es | alcalinidad (bicarbonatos, HCO₃⁻) |
| pl | wodorowęglany (HCO₃⁻) — the quantity itself is `zasadowość`, **not** `alkaliczność` |
| tr | alkalinite |

**carbonate hardness (KH)** — alkalinity expressed in degrees, as a drop-test kit reports it. Nothing
to do with carbon, and not a kind of firmness.

| | |
|---|---|
| ru | карбонатная жёсткость |
| uk | карбонатна жорсткість |
| de | Karbonathärte |
| nl | carbonaathardheid (KH) ⚠ — aquarium register; professional writing says `tijdelijke hardheid` or names the ion |
| es | dureza de carbonatos (KH) |
| pl | twardość węglanowa (KH) |
| tr | karbonat sertliği (KH) — textbook synonym `geçici sertlik` |

**general hardness (GH)** — calcium and magnesium together, in degrees. The everyday sense of "hard
water".

| | |
|---|---|
| ru | общая жёсткость |
| uk | загальна жорсткість |
| de | Gesamthärte |
| nl | totale hardheid |
| es | dureza total |
| pl | twardość ogólna (GH) |
| tr | genel sertlik (GH) on kits, `toplam sertlik` in reports |

**The unit a drop test prints is not the unit a water report prints, and they differ by 1,78×.**
This matters more than the words. Spanish agronomic reports use French degrees — IFAPA: *"Se expresa
en grados franceses"*, and a real Almería lab report in a University of Almería project reads
*"16,29 °HTF"* — while the drop-test kits sold in Spain are German-made (JBL, Sera, Tetra) and print
°dH, one drop per degree. Dutch reports give mmol/l with °dH secondary; **Flanders uses French
degrees**, as De Watergroep says outright: *"Franse graden, die gebruiken we vooral in België."*
Since this app asks for a *drop-test* reading, °dH is the right default in both markets — but the
label must always carry the unit. A Flemish grower reading an unlabelled 8 as °fH when °dH was meant
is out by 1,78×.

`KH` and `GH` themselves are weaker than they look outside German-speaking markets. In Dutch they are
aquarium and pond vocabulary: no professional horticultural or water-utility source uses them, and the
trade press says `tijdelijke hardheid` and `blijvende hardheid` instead. In Spanish, `dureza de
carbonatos` and `dureza temporal` exist but were found in water-treatment and aquarium writing, not in
IFAPA, Cajamar or university texts, which use bare `dureza` for Ca+Mg and treat carbonates as a
separate line. Keep KH/GH as the labels a hobbyist arriving from aquarium tooling will recognise, and
do not build the interface's own vocabulary on them.

**electrical conductivity (EC)** — how well the solution conducts, and the everyday proxy for how
strong a feed is. Growers say "EC" in all eight languages; keep the abbreviation.

| | |
|---|---|
| ru | электропроводность (ЕС) |
| uk | електропровідність (EC) |
| de | Leitfähigkeit (EC) |
| nl | geleidbaarheid (EC) |
| es | conductividad eléctrica (CE) |
| pl | przewodność elektryczna (EC) |
| tr | elektriksel iletkenlik (EC) |

**total dissolved solids (TDS)** — conductivity multiplied by a fixed factor and printed as "ppm" by
some meters. Two scales exist, 500 and 700, and they disagree about the same water.

| | |
|---|---|
| ru | общая минерализация (TDS) |
| uk | загальна мінералізація (TDS) |
| de | Gesamtsalzgehalt (TDS) |
| nl | totaal opgeloste stoffen (TDS) |
| es | sólidos disueltos totales (TDS) |
| pl | TDS / `ogólna zawartość soli` — growers say TDS untranslated; agronomists say `zasolenie` |
| tr | toplam çözünmüş madde (TDS) |

**source water** — what comes out of the tap, well or rain barrel, before anything is added. Not
"water supply" in the utility sense.

| | |
|---|---|
| ru | исходная вода |
| uk | вихідна вода |
| de | Ausgangswasser |
| nl | uitgangswater |
| es | agua de riego |
| pl | woda do nawadniania (przed dodaniem nawozów) |
| tr | sulama suyu (gübre eklenmeden önce) |

**Checked, and my own assumption was backwards.** I had guessed that the everyday phrase for
irrigation water meant the water *after* mixing, and in both languages it does not.

Dutch `uitgangswater` is defined verbatim in a vocational horticulture textbook's glossary: *"Dit is
het water waaraan de tuinder voedingsstoffen toevoegt. Het uitgangswater bestaat uit bronwater,
regenwater of leidingwater of combinaties daarvan"* — which is exactly this app's concept. `Gietwater`
is not the mixed solution either: it is the irrigation-water *supply*, still pre-fertilizer, listed by
Glastuinbouw Nederland as rainwater, groundwater, RO or mains. It overlaps `uitgangswater` loosely, so
`uitgangswater` is the precise term and the one to use.

Spanish `agua de riego` **is** the source water, and it is what Almería's own literature calls it:
Cajamar's pepper-in-substrate manual says *"si se sospecha que ha cambiado la composición del agua de
riego, se debe analizar ésta"*, and infoagro defines the mix as something else — *"A la mezcla formada
por el agua de riego y los elementos minerales disueltos en ella se le denomina 'solución
nutritiva'"*. My `agua de origen` and the candidate `agua de partida` occur in none of the four
agronomic documents checked; `agua bruta` is a water-utility term for raw water before potabilization
and would not cover RO output at all.

The mixed solution is `voedingsoplossing` and `solución nutritiva`, both already in this glossary, and
`druppelwater` at the emitter. Those are the words that must never be used for the input.

**Turkish agrees with both, and gives the cleanest disambiguation.** `Sulama suyu` is the water
*before* fertilizer: the MEB soilless-growing module opens the recipe with *"Besin çözeltisi
hazırlığında yapılması gereken ilk işlem sulama suyunun tahlil edilmesidir"* — the first thing to do is
analyse the irrigation water — and later mixes the stock *into* it. `Besin çözeltisi` is the mixture.
One ambiguity to design around: fertigation writing reuses the phrase family for the mix as `gübreli
sulama suyu`, so where the two fields sit side by side, `Sulama suyu (ham su)` costs nothing.
`Ham su` is the attested term for raw feed water.

**Polish makes it three out of three, and `woda wyjściowa` was worse than merely unattested.**
Treder, of Instytut Ogrodnictwa, writes that the contents of the irrigation water *"powinny być odjęte
od wyjściowego składu pożywki"* — subtracted from the **starting composition of the recipe**. In Polish
horticultural writing `wyjściowy` attaches to the recipe, not to the water, so `woda wyjściowa` would
point a grower at the wrong one of the two things this app subtracts between. Use `woda do
nawadniania`. `woda surowa` is defined relative to a treatment step and would be self-contradictory for
RO permeate; `woda zasilająca` is boiler feed-water.

**reverse osmosis** — water with essentially nothing dissolved in it. Growers shorten it to "osmosis"
or "RO"; keep whichever is idiomatic.

| | |
|---|---|
| ru | обратный осмос |
| uk | зворотний осмос |
| de | Umkehrosmose |
| nl | omgekeerde osmose |
| es | ósmosis inversa |
| pl | odwrócona osmoza |
| tr | ters ozmoz |

**softened water (ion exchange)** — water from a domestic softener, which trades calcium for sodium.
High conductivity, no calcium. The distinction matters: treated as merely hard, it produces a recipe
that starves the plant of calcium while loading it with sodium.

| | |
|---|---|
| ru | умягчённая вода (натрий-катионирование) |
| uk | зм'якшена вода (натрій-катіонування) |
| de | enthärtetes Wasser (Ionenaustausch) |
| nl | onthard water (ionenwisselaar, Na⁺) |
| es | agua descalcificada (intercambio iónico sódico) — LatAm `agua suavizada` / `ablandada` |
| pl | woda zmiękczona (z wymiennika sodowego) — not `woda miękka` |
| tr | yumuşatılmış su (iyon değiştirici) — never `yumuşak su` |

**Never label this one with the word for naturally soft water.** That collapse is not
hypothetical: Dutch and Spanish consumer softener marketing routinely advertises softened water *as*
`zacht water` and `agua blanda`, which is the whole distinction this profile exists to make. `Onthard`
and `descalcificada` are past participles — "was made soft by treatment" — and carry it in the word
itself. Keep a separate `van nature zacht water` / `agua blanda` for rain and RO.

Confidence here is medium rather than high, and the reason is worth recording: professional
horticulture in neither market softens by sodium exchange — the Netherlands uses reverse osmosis,
Almería desalinates — so the sodium warnings that exist come from softener vendors rather than from
agronomic sources. The defensible framing is arithmetic, not a quotation: growers' own limit is sodium
below 0,5 mmol/l in irrigation water, softening adds roughly 0,35 mmol/l of sodium per °dH removed, so
at a Dutch tap hardness of 8 °dH the result is about 2,8 mmol/l — five times the limit. That is why the
profile is separate, and it needs no vendor to say so.

Turkish carries the distinction in a participle exactly as Dutch does — `yumuşatılmış` is
"softened", `yumuşak` is "soft" — and Turkish consumer copy blurs them just as badly: one vendor's
comparison table is headed `Yumuşak Su` while its own mineral row reads *"Düşük mineral, yüksek sodyum
iyonları içerir"*. A Turkish water-treatment supplier states the mechanism better than any agronomic
source could: *"sertlik iyonları tutulurken, sodyum iyonları suya katılır… suyun iyon dengesinde
herhangi bir değişiklik olmaz"* — the hardness ions are held back, sodium goes in, and the water's ion
balance does not change. That last clause is this profile's whole point, in a Turkish sentence.

Polish carries it in a participle as well — `zmiękczona` against `miękka` — and Polish hydroponic
writing defines `woda miękka` by low EC, which is exactly what softened water is not. The softener
literature frames it the same way everywhere: calcium out, sodium in, and *"wysokie stężenie sodu
utrudnia również wchłanianie innych składników mineralnych, w tym potasu"*.

**cation surplus** — the gap between positive and negative charge in a water analysis. Not an error:
it measures the bicarbonate nobody entered.

| | |
|---|---|
| ru | избыток катионов |
| uk | надлишок катіонів |
| de | Kationenüberschuss |
| nl | kationenoverschot |
| es | exceso de cationes |
| pl | nadmiar kationów |
| tr | katyon toplamının anyon toplamını aşan kısmı — no noun exists; `katyon–anyon farkı` if a label must be short |

## Nutrients and their forms

**Turkish has no noun for this, and the near-miss is dangerous.** `katyon-anyon dengesi` is a real
Turkish term — but for the ion-balance *check* a laboratory runs to validate an analysis, not for the
residual. Worse, the ministry's own irrigation guidance defines `RSC`, *"artık karbonat miktarı"*, as
`(CO₃ + HCO₃) − (Ca + Mg)`: the mirror image of what this app computes, close enough that a Turkish
agronomist would read a bare noun as that. The ministry solves the naming problem the same way this
glossary now does — with a descriptive phrase, `…toplamından fazla olan …konsantrasyonu`. Do not coin
a noun here. If a short label is unavoidable, `katyon–anyon farkı`; the surplus suffix in this field is
`-lık`, as in the attested `sodyum fazlalığı`, not `-ı`.

**nutrient** — an element a plant takes up. In this app always an element, never a product.

| | |
|---|---|
| ru | элемент питания |
| uk | елемент живлення |
| de | Nährstoff |
| nl | nutriënt / voedingselement |
| es | nutriente |
| pl | składnik pokarmowy |
| tr | besin elementi |

**macronutrient / micronutrient** — the six taken up in large amounts (N, P, K, Ca, Mg, S) and the
trace elements. Both words exist in all eight languages.

| | |
|---|---|
| ru | макроэлемент / микроэлемент |
| uk | макроелемент / мікроелемент |
| de | Makronährstoff / Mikronährstoff |
| nl | macronutriënt / micronutriënt |
| es | macronutriente / micronutriente |
| pl | makroelement / mikroelement |
| tr | makro element / mikro element |

**counter-ion** — an ion that arrives with the nutrient you wanted rather than being dosed for.
Chloride and sodium in this app.

| | |
|---|---|
| ru | противоион |
| uk | протиіон |
| de | Gegenion |
| nl | tegenion |
| es | contraión |
| pl | przeciwjon |
| tr | ⚠ unattested in agronomy — name the ions, or `yük dengeleyici iyon` |

**nitrate nitrogen / ammonium nitrogen / amide nitrogen** — the three forms nitrogen arrives in. They
behave differently at the root: ammonium acidifies, nitrate alkalises, and urea's amide nitrogen
carries no charge at all. Never collapse them into one word.

| | |
|---|---|
| ru | нитратный азот / аммонийный азот / амидный азот |
| uk | нітратний азот / амонійний азот / амідний азот |
| de | Nitratstickstoff / Ammoniumstickstoff / Amidstickstoff |
| nl | nitraatstikstof / ammoniumstikstof / amidestikstof (ureum) |
| es | nitrógeno nítrico / nitrógeno amoniacal / nitrógeno ureico |
| pl | azot azotanowy / azot amonowy / azot amidowy |
| tr | nitrat azotu / amonyum azotu / **üre** azotu |

**Checked; both stay distinct, and in Spanish by law.** Royal Decree 824/2005 requires nitrogen to
be declared as *"nítrico, amoniacal, ureico y orgánico"*, so the three cannot merge on a Spanish
label — and note the fourth, `nitrógeno orgánico`, which anything parsing Spanish labels will meet.
Dutch attests all three, though the `-stikstof` vocabulary belongs to the arable world: greenhouse
practice reads NO₃⁻ and NH₄⁺ in mmol/l off an analysis and rarely mentions urea, which is scarce in
hydroponic recipes. `NO₃-N (nitraatstikstof)` reads to both audiences.

**Turkish `amid azotu` was wrong, and the correction comes from label law.** The annex to the
Turkish chemical-fertilizer regulation, decoded locally, contains `nitrat azotu` 49 times, `amonyak
azotu` 29, `üre azotu` 30 — and **`amid azotu` zero times**; the only `amid` in it is the chemical name
`disiyandiamid`. Ministry registration certificates for water-soluble NPK confirm it on the package
text itself: *"Toplam N %:18 Amonyum N:7,6 Nitrat N:4,2 Üre N:6,2"*. So the third form is `üre azotu`,
and all three are declared side by side, which is the strongest possible guarantee they stay distinct.
`Amid` does have a legitimate Turkish use, but as an adjective for the fertilizer class — Toros Tarım
on urea: *"azotun NH2 formunda olması nedeniyle amidli bir gübredir"*. Prefer `amonyum azotu` over the
older `amonyak azotu` of the 2004 annex: it is chemically right and it is what current labels print.

Polish keeps all three and puts them in four columns on the bag — Grupa Azoty's label table reads
`Azot N ogółem | Azot N azotanowy | Azot N amonowy | Azot N amidowy` — so unlike Turkish, `amidowy`
**is** the Polish label word. Watch the fourth column: `azot ogółem` is the total and must not merge
with any of the three. Yara Poland's fertigation tables use `NO3-N`, `NH4-N`, `NH2-N`.

**chelate / chelated** — a metal held by an organic molecule so it stays available. `EDTA`, `DTPA`,
`EDDHA` and `HBED` are written as they are in every language. Nothing to do with shells.

| | |
|---|---|
| ru | хелат / в хелатной форме |
| uk | хелат / у хелатній формі |
| de | Chelat / chelatiert |
| nl | chelaat / gechelateerd |
| es | quelato / quelatado |
| pl | chelat / schelatowany |
| tr | şelat / şelatlı |

**non-chelated** — the plain mineral salt form, as opposed to chelated.

| | |
|---|---|
| ru | нехелатная форма |
| uk | нехелатна форма |
| de | nicht chelatiert |
| nl | niet-gechelateerd |
| es | no quelatado |
| pl | nieschelatowany — with the **s** |
| tr | şelatsız |

## Fertilizers and recipes

**fertilizer salt** — one dry compound on the shelf, such as potassium nitrate. In this interface
"salt" is always this, never table salt.

| | |
|---|---|
| ru | удобрение (соль) |
| uk | добриво (сіль) |
| de | Düngesalz |
| nl | enkelvoudige meststof |
| es | sal fertilizante — **not** `abono simple`, see below |
| pl | nawóz pojedynczy |
| tr | gübre tuzu |

**`meststofzout` is deleted, not corrected: it does not exist.** Zero occurrences across the three
professional Dutch documents searched, and it appears to have been produced by a page summariser rather
than found in a source. Dutch has an established opposition for exactly this distinction —
`enkelvoudige meststof` against `samengestelde meststof`, defined in the vocational textbook and used
as a live product category by Royal Brinkman, whose tree runs *Meststoffen › Wateroplosbare
Meststoffen › Enkelvoudige Meststoffen* over Kalisalpeter, Kalksalpeter and Monokaliumfosfaat.

**Spanish has a trap here that plausibility would have walked straight into.** `abono simple` is a
regulatory category, not a description: RD 824/2005 defines it as a fertilizer declaring *a single*
primary nutrient. Potassium nitrate declares both N and K, so it is legally an `abono compuesto` — and
calling it simple would put the interface at odds with the category on the supplier's own invoice.
Spanish price lists sidestep the question and name the chemistry: *"Nitrato potásico cristalino
13-0-46, 25 kg"*. `sal fertilizante` is attested for the generic concept and is what this interface
means, but it is academic in Spain; prefer `fertilizante` over `abono` in shared strings, since in much
of Latin America `abono` means manure or compost.

Product names worth carrying into the Dutch translation, because a grower recognises the trade name
before the chemistry: KNO₃ is `Kalisalpeter`, Ca(NO₃)₂ is `Kalksalpeter`, MKP is `Monokaliumfosfaat`.

**Polish repeats the Spanish legal trap exactly, in the same regulation.** EU 2019/1009 in Polish
defines a `prosty` fertilizer as declaring *"tylko jednego makroskładnika pokarmowego"* — only one —
so potassium nitrate, declaring N and K, is `wieloskładnikowy`. `nawóz jednoskładnikowy` would
therefore be wrong for most of this app's shelf. The horticultural term of art is `nawóz pojedynczy`,
and Instytut Ogrodnictwa uses it for exactly this set: *"Pożywki do fertygacji róż… możemy
przygotowywać z nawozów pojedynczych lub wieloskładnikowych"*, with saletra potasowa named among them.
`sól nawozowa` is intelligible but belongs to the planted-aquarium hobby. KNO₃ on a Polish sack is
`saletra potasowa`; MKP is `fosforan jednopotasowy`.

Polish spells non-chelated with an **s**: the verb is *schelatować*, so the participle is
*schelatowany* and the negative is `nieschelatowany`, solid, as in ADOB's own chart caption *"w postaci
nieschelatowanej (FeSO₄)"*. `niechelatowany` and the hyphenated forms produced no attestations.

**target** — the nutrient profile in ppm that the recipe has to hit. Not a "goal" in the motivational
sense.

| | |
|---|---|
| ru | целевой профиль |
| uk | цільовий профіль |
| de | Zielprofil |
| nl | streefwaarden |
| es | perfil objetivo |
| pl | profil docelowy |
| tr | hedef profil |

**One term I could not confirm at all, and am recording as such.** `karşı iyon` returns nothing
from the Turkish Language Association's dictionaries, nothing from two downloaded Turkish theses that
were grepped for it, and nothing from any ministry, MEGEP or agronomy document checked. Turkish
ion-exchange writing says `karşı yük` — counter-*charge* — instead. The word may exist in Turkish
colloid chemistry, but it has no currency in agronomy, and a grower in Antalya will not recognise it.
The interface should name the ions and describe them rather than reach for a noun. This is the one
entry in the file I would not ship without a Turkish agronomist reading it.

**recipe** — one set of weights that reaches the target. The app produces several.

| | |
|---|---|
| ru | рецепт |
| uk | рецепт |
| de | Rezept |
| nl | recept |
| es | receta |
| pl | receptura |
| tr | reçete |

**reservoir** — the tank the finished solution is mixed in, sized in litres.

| | |
|---|---|
| ru | бак |
| uk | бак |
| de | Vorratsbehälter / Tank |
| nl | voorraadvat / tank |
| es | depósito |
| pl | zbiornik |
| tr | depo / tank |

**nutrient solution** — the finished, diluted liquid the plants get.

| | |
|---|---|
| ru | питательный раствор |
| uk | живильний розчин |
| de | Nährlösung |
| nl | voedingsoplossing |
| es | solución nutritiva |
| pl | pożywka |
| tr | besin çözeltisi |

**concentrate (stock solution), tanks A and B** — the strong solution made up in advance and diluted
at feeding. Two tanks because calcium precipitates with sulfate and phosphate. `A` and `B` stay as
letters.

| | |
|---|---|
| ru | концентрат (маточный раствор), баки A и B |
| uk | концентрат (маточний розчин), баки A і B |
| de | Konzentrat (Stammlösung), Behälter A und B |
| nl | concentraat (stockoplossing), tanks A en B |
| es | concentrado (solución madre), tanques A y B |
| pl | koncentrat, zbiornik A i zbiornik B — the diluted feed is `pożywka robocza` |
| tr | konsantre (ana çözelti), tank A ve B |

`roztwór zapasowy` is deleted for the same reason as `meststofzout`: zero attestations. Polish
already carries the distinction this app needs — `koncentrat` for the strong solution against `pożywka
robocza` for the diluted feed, from a nursery fertigation article that also gives the A/B split in the
same terms this app uses: nitrates and iron chelate in `zbiornik A`, sulfates and phosphates in
`zbiornik B`.

**solubility** — how much of a salt dissolves in a litre before it stops dissolving.

| | |
|---|---|
| ru | растворимость |
| uk | розчинність |
| de | Löslichkeit |
| nl | oplosbaarheid |
| es | solubilidad |
| pl | rozpuszczalność |
| tr | çözünürlük |

**saturated / saturation** — the point where no more will dissolve. The app warns when a concentrate
is asked to hold more than that.

| | |
|---|---|
| ru | насыщение / насыщенный |
| uk | насичення / насичений |
| de | Sättigung / gesättigt |
| nl | verzadiging / verzadigd |
| es | saturación / saturado |
| pl | nasycenie / nasycony |
| tr | doygunluk / doygun |

## Acid

**acidification** — adding acid to bring the solution to a working pH by neutralising the water's
alkalinity.

| | |
|---|---|
| ru | подкисление |
| uk | підкислення |
| de | Ansäuerung |
| nl | aanzuren (*het aanzuren*) — not `verzuring` |
| es | acidificación |
| pl | zakwaszanie |
| tr | asitlendirme |

**In Dutch the two words are not synonyms and the wrong one blames the grower.** `Aanzuren` is the
act: Van Iperen's advisory is titled *"Aanzuren van druppelwater"* and never uses the alternative.
`Verzuring` is the unwanted outcome — Royal Brinkman reserves it for exactly that, *"ongewenste
verzuring van het wortelmilieu"*, and the register is environmental besides: the Dutch national
monitoring network for acid deposition is called *TrendMeetnet Verzuring*. A card headed `Verzuring`
would read as a warning about damage rather than a tool for doing the job. `Zuurdosering` serves well
for the computed dose; practitioners say `wegzuren` for removing bicarbonate specifically.

**nitric / phosphoric / sulfuric acid** — the three used. Note the trap: a bag's "phosphoric" figure
is often quoted as P₂O₅.

| | |
|---|---|
| ru | азотная / ортофосфорная / серная кислота |
| uk | азотна / ортофосфорна / сірчана кислота |
| de | Salpetersäure / Phosphorsäure / Schwefelsäure |
| nl | salpeterzuur / fosforzuur / zwavelzuur |
| es | ácido nítrico / fosfórico / sulfúrico |
| pl | kwas azotowy / fosforowy / siarkowy |
| tr | nitrik / fosforik / sülfürik asit |

## Readings and units

Element symbols and chemical formulas are **never translated**: `N`, `P`, `K`, `Ca`, `Ca(NO₃)₂·4H₂O`
are written identically in all eight markets.

**Units are shown in Latin in all eight languages, and that is a deliberate limitation rather than a
claim about language.** The app prints them from the markup — `<span class="unit">µS/cm</span>` — not
from these resource files, so they are the same string everywhere.

For five of the languages that is also what their own agronomic writing does. For **Russian and
Ukrainian it is not**: those markets write `мСм/см`, `мкСм/см`, `мэкв/л` / `мекв/л`, `г/л`, `л`. Both
translators noticed the mismatch and left the Latin forms alone rather than putting `мэкв/л` beside
`meq/L` in the same panel, which is what localising only the resource file would have produced. That
was the right call, and it leaves a real gap rather than a solved problem.

If Cyrillic units are wanted, it is a change to the components and not to a translation: about eight
unit strings move out of the markup into keys, and the layout has to be re-measured, because `мкСм/см`
is wider than `µS/cm` in a metric tile that is already tight. Recorded here so the decision gets made
rather than inherited.

The hardness unit is the exception that already lives in a key, because it differs by market rather than
by language: `°dH` everywhere except Polish, whose kits print German degrees as `°n`.

An earlier version of this section gave `мС/см`. That was wrong: `См` is siemens, `С` alone is coulomb.

## The oxide trap, for every translator

Fertilizer labels in all eight markets quote phosphorus and potassium as **oxides** — P₂O₅ and K₂O —
while this app works in elements. A figure copied straight off a bag overstates phosphorus by 2.29×
and potassium by 1.20×. Wherever the interface says "as elements, not oxides", that warning is the
point of the sentence and must survive translation intact.

| | |
|---|---|
| ru | в элементах, не в оксидах |
| uk | в елементах, не в оксидах |
| de | als Element, nicht als Oxid |
| nl | als element, niet als oxide |
| es | como elemento, no como óxido |
| pl | jako pierwiastek, nie jako tlenek |
| tr | element olarak, oksit olarak değil |

## The oxide convention is law, not custom, and it reaches further than P and K

Spanish Royal Decree 824/2005 does not merely permit the oxide form, it requires it: *"el nitrógeno
únicamente en forma de elemento (N); el fósforo únicamente en forma de pentóxido de fósforo (P2O5); el
potasio únicamente en forma de óxido de potasio (K2O)"*. And it goes on — *"con la excepción del calcio
(CaO) y el magnesio (MgO), en que se utilizan igualmente los óxidos"*. So a Spanish calcium nitrate bag
may declare **CaO rather than Ca**, which this app's oxide warning does not currently mention. The
Spanish word for the declared percentage is `riqueza`.

## What has been checked, and what has not

| Language | State |
|---|---|
| ru, uk, de | written with confidence, not externally checked |
| nl | checked; three entries changed, one deleted as non-existent |
| es | checked; three entries were wrong |
| pl | checked; six entries changed, including the one this file was opened to settle |
| tr | checked; four entries changed, one could not be attested at all |

**Fourteen entries were wrong across the four languages, and only six of them carried a ⚠.** The marks
were a fair record of where I felt uncertain and a poor predictor of where I was mistaken: `abono
simple` and `nawóz jednoskładnikowy` both read as obviously right and are both legally wrong for
potassium nitrate, while `alkaliczność`, which I flagged, was merely the less standard of two real
words.

**A note on method, because it changed the outcome twice.** Both reviews found that fetching a PDF and
letting a summariser describe it *invented terms that were not in the document*: one reported
`meststofzout` and misread `uitgangswater`, the other reported that a University of Almería project
contained `alcalinidad` and `agua de partida` when a local text extraction showed neither word occurs.
Both would have produced a wrong recommendation with a citation attached to it. Every decision above
was made from text extracted and searched locally, with the sentence quoted. Anyone continuing this
file should work the same way: a plausible term with a real URL beside it is the most dangerous shape a
mistake can take here.

## What a reviewer should check first

Two entries are still open, and both are honest gaps rather than pending work:

- **`carbonaathardheid` / KH in Dutch** is real but belongs to the aquarium and pond register; no
  professional horticultural or utility source uses KH or GH, and the trade press says `tijdelijke
  hardheid` and `blijvende hardheid`. Keep KH/GH as the label a hobbyist arriving from aquarium tooling
  will look for, and know it is not the professional word.
- **`karşı iyon` in Turkish could not be attested at all** — nothing in the Turkish Language
  Association's dictionaries, nothing in two theses grepped for it, nothing in any ministry or
  ziraat-fakültesi document. Turkish ion-exchange writing says `karşı yük`, counter-*charge*. The
  interface should name sodium and chloride and describe them. This is the one entry I would not ship
  without a Turkish agronomist reading it.

## Four patterns that held across every language checked

Each was a surprise once and then stopped being one.

1. **Nobody routes the acid dose through a word meaning "alkalinity".** Dutch, Spanish, Polish and
   Turkish reports and horticultural writing all name the bicarbonate ion, in mmol/L or mg/L. The
   abstract noun exists in all four and is correct in all four; it is simply not what a grower reads off
   the paper in their hand. Label the field with the ion and gloss it with the quantity.
2. **The everyday phrase for "irrigation water" means the water *before* fertilizer, not after.** I
   assumed the opposite and was wrong in Spanish, Polish and Turkish alike. The mixed solution has its
   own word everywhere — `voedingsoplossing`, `solución nutritiva`, `pożywka`, `besin çözeltisi` — and
   those are the words that must never leak into the input field.
3. **Check whether a category word is a legal term before using it as a description.** `abono simple`
   and `nawóz jednoskładnikowy` are both defined by regulation as declaring a *single* macronutrient, so
   both are wrong for potassium nitrate, which declares two. The same regulation, in two languages, in
   both cases.
4. **The oxide convention reaches further than P and K, and it is law rather than custom.** Spain,
   Poland and Turkey all mandate the oxide form and all three extend it to calcium and magnesium —
   CaO, MgO — with Turkey adding Na₂O and SO₃ and Poland publishing the conversion factors in the same
   table as P and K. An app that warns about P₂O₅ and K₂O alone is warning about half of it.

## Three things this changes in the app, not in this file

1. **The ppm-meter factor.** Turkish irrigation literature converts with **×640** — the ministry's own
   guidance states `ppm = mg/l = (EC mmhos/cm) x 640`. The interface offers 500 and 700 only, so a
   Turkish grower comparing against their own report will find a mismatch it does not explain.
2. **The hardness unit label per language.** Polish drop-test kits print German degrees but write them
   **`°n`**; Spanish reports use French degrees while the kits sold in Spain are German and print °dH;
   Flanders uses French degrees where the Netherlands uses °dH and mmol/l. The app asks for a drop-test
   reading, so °dH is the right default everywhere — but the label must carry the unit in the form that
   market prints, and confusing °dH with °fH is a factor of 1,78.
3. **The oxide warning.** It names P₂O₅ and K₂O. Calcium and magnesium are declared as oxides on Spanish,
   Polish and Turkish bags too, and a Turkish professor's own summary is the sentence worth borrowing:
   *"Ambalaj üzerindekiler form değil sadece bir simgedir"* — what is on the packaging is a symbol, not
   a form.
