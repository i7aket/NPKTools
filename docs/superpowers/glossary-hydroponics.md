# Hydroponics glossary for the eight interface languages

Date: 2026-08-03
Status: **Dutch and Spanish checked against primary sources; Russian, Ukrainian and German written
with confidence but unchecked; Polish and Turkish still to check.** Do not translate an entry that is
still marked ⚠.

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
| pl | alkaliczność ⚠ (also *zasadowość* — the more common word in water-treatment writing; prefer it if a reviewer confirms) |
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
| pl | twardość węglanowa |
| tr | karbonat sertliği |

**general hardness (GH)** — calcium and magnesium together, in degrees. The everyday sense of "hard
water".

| | |
|---|---|
| ru | общая жёсткость |
| uk | загальна жорсткість |
| de | Gesamthärte |
| nl | totale hardheid |
| es | dureza total |
| pl | twardość ogólna |
| tr | toplam sertlik |

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
| pl | ogólna zawartość soli (TDS) ⚠ |
| tr | toplam çözünmüş katı (TDS) ⚠ |

**source water** — what comes out of the tap, well or rain barrel, before anything is added. Not
"water supply" in the utility sense.

| | |
|---|---|
| ru | исходная вода |
| uk | вихідна вода |
| de | Ausgangswasser |
| nl | uitgangswater |
| es | agua de riego |
| pl | woda wyjściowa ⚠ |
| tr | kaynak suyu |

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
| pl | woda zmiękczona (wymiana jonowa) |
| tr | yumuşatılmış su (iyon değişimi) |

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
| tr | katyon fazlası ⚠ |

## Nutrients and their forms

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
| tr | karşı iyon ⚠ |

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
| tr | nitrat azotu / amonyum azotu / amid azotu ⚠ |

**Checked; both stay distinct, and in Spanish by law.** Royal Decree 824/2005 requires nitrogen to
be declared as *"nítrico, amoniacal, ureico y orgánico"*, so the three cannot merge on a Spanish
label — and note the fourth, `nitrógeno orgánico`, which anything parsing Spanish labels will meet.
Dutch attests all three, though the `-stikstof` vocabulary belongs to the arable world: greenhouse
practice reads NO₃⁻ and NH₄⁺ in mmol/l off an analysis and rarely mentions urea, which is scarce in
hydroponic recipes. `NO₃-N (nitraatstikstof)` reads to both audiences.

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
| pl | niechelatowany ⚠ |
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
| pl | sól nawozowa ⚠ |
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
| pl | koncentrat (roztwór zapasowy), zbiorniki A i B ⚠ |
| tr | konsantre (ana çözelti), tank A ve B |

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

| Unit | ru | uk | de | nl | es | pl | tr |
|---|---|---|---|---|---|---|---|
| mS/cm | мС/см | мС/см | mS/cm | mS/cm | mS/cm | mS/cm | mS/cm |
| µS/cm | мкСм/см | мкСм/см | µS/cm | µS/cm | µS/cm | µS/cm | µS/cm |
| meq/L | мэкв/л | мекв/л | meq/L | meq/L | meq/L | meq/L | meq/L |
| ppm | ppm | ppm | ppm | ppm | ppm | ppm | ppm |
| g/L | г/л | г/л | g/L | g/L | g/L | g/L | g/L |
| litre | л | л | L | L | L | L | L |
| °dH / °dKH | °dH / °dKH | °dH / °dKH | °dH / °dKH | °dH / °dKH | °dH / °dKH | °dH / °dKH | °dH / °dKH |

Russian and Ukrainian localise unit abbreviations; the other five keep the Latin forms, which is what
their own agronomic writing does.

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
| nl | **checked against primary sources**; three entries changed, one deleted as non-existent |
| es | **checked against primary sources**; three entries were wrong and are corrected |
| pl, tr | still to check — the ⚠ marks below stand |

**A note on method, because it changed the outcome twice.** Both reviews found that fetching a PDF and
letting a summariser describe it *invented terms that were not in the document*: one reported
`meststofzout` and misread `uitgangswater`, the other reported that a University of Almería project
contained `alcalinidad` and `agua de partida` when a local text extraction showed neither word occurs.
Both would have produced a wrong recommendation with a citation attached to it. Every decision above
was made from text extracted and searched locally, with the sentence quoted. Anyone continuing this
file should work the same way: a plausible term with a real URL beside it is the most dangerous shape a
mistake can take here.

## What a reviewer should check first

For **Polish and Turkish**, the entries marked ⚠, and then these four, because getting them wrong
changes what a grower does:

1. **alkalinity** vs **hardness** — different quantities, and Polish especially has two competing words
   for the first. Note what the Dutch and Spanish checks found: both languages name the ion,
   bicarbonate, rather than the abstract quantity. Polish may do the same.
2. **carbonate hardness** — must not drift towards "carbon" or "firmness", and the unit convention has
   to be established separately for water reports and for drop-test kits, which disagree in Spain.
3. **the three nitrogen forms** — collapsing them loses the pH behaviour that makes them worth showing.
4. **softened water** — if this reads as a synonym for soft water, the whole reason the preset exists
   disappears.

And one lesson that generalises: check whether a category word is a **legal** term before using it as a
description. `abono simple` was the trap in Spanish; Polish and Turkish fertilizer labelling law will
have its own.
