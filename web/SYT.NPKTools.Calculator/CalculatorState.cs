using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Calculator;

/// <summary>
/// Everything a person typed in, in a form that can be stored, linked and filed.
/// </summary>
/// <remarks>
/// <para>
/// The app has no server, so there is nowhere to sync an account to. That leaves three mechanisms, and
/// they are not alternatives — each answers a different question:
/// </para>
/// <list type="bullet">
/// <item>
/// <b>Local storage</b> — "do not lose my work when I close the tab". Bound to one browser profile on
/// one origin, and cleared whenever site data is cleared. It cannot move between browsers; that is how
/// the browser works, not a shortcoming here.
/// </item>
/// <item>
/// <b>The link</b> — "let me carry this to another browser, or send it to someone". The whole setup
/// rides in the address, so a copied URL is the transfer. It also makes recipes bookmarkable.
/// </item>
/// <item>
/// <b>A file</b> — "let me keep several of these". Survives clearing site data and can be read and
/// edited by hand.
/// </item>
/// </list>
/// <para>
/// The link and the file encode the same state differently on purpose. A link must be short enough to
/// paste into a chat window, so it uses indices and drops anything at its default. A file is an archive
/// someone may open in a text editor years later, so it spells out salt names and includes the version
/// it came from.
/// </para>
/// </remarks>
public sealed class CalculatorState
{
    /// <summary>The target string, exactly as typed.</summary>
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    /// <summary>Source water in ppm, keyed by element symbol. Elements at zero may be omitted.</summary>
    [JsonPropertyName("water")]
    public Dictionary<string, double> Water { get; set; } = [];

    /// <summary>Names of the salts on the shelf.</summary>
    [JsonPropertyName("salts")]
    public List<string> Salts { get; set; } = [];

    /// <summary>Litres per concentrate tank, or null for no concentrate.</summary>
    [JsonPropertyName("concentrateLiters")]
    public double? ConcentrateLiters { get; set; }

    /// <summary>How the water was described. Absent in version 1, where it is inferred.</summary>
    [JsonPropertyName("waterMode")]
    public string? WaterMode { get; set; }

    /// <summary>The chosen water shape.</summary>
    [JsonPropertyName("waterPreset")]
    public string? WaterPresetId { get; set; }

    /// <summary>The meter reading, in whatever scale <see cref="WaterEcUnit"/> names.</summary>
    [JsonPropertyName("waterEc")]
    public double? WaterEc { get; set; }

    /// <summary>The scale the meter reading is in.</summary>
    [JsonPropertyName("waterEcUnit")]
    public string? WaterEcUnit { get; set; }

    /// <summary>General hardness in °dH, when measured.</summary>
    [JsonPropertyName("waterGh")]
    public double? WaterGh { get; set; }

    /// <summary>Carbonate hardness in °dKH, when measured.</summary>
    [JsonPropertyName("waterKh")]
    public double? WaterKh { get; set; }

    /// <summary>Whether the water is being acidified.</summary>
    [JsonPropertyName("acidEnabled")]
    public bool? AcidEnabled { get; set; }

    /// <summary>The chosen acid.</summary>
    [JsonPropertyName("acidId")]
    public string? AcidId { get; set; }

    /// <summary>The pH to reach.</summary>
    [JsonPropertyName("targetPh")]
    public double? TargetPh { get; set; }

    /// <summary>The pH of the untreated water.</summary>
    [JsonPropertyName("waterPh")]
    public double? WaterPh { get; set; }

    /// <summary>Salts the grower described themselves. Absent from links written before they existed.</summary>
    [JsonPropertyName("customSalts")]
    public List<CustomSalt> CustomSalts { get; set; } = [];

    // ---------------------------------------------------------------- file

    /// <summary>
    /// Writes the state as indented JSON, for a file someone might open and edit.
    /// </summary>
    /// <returns>The JSON text.</returns>
    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    /// <summary>
    /// Reads state from JSON, returning null rather than throwing on anything unreadable.
    /// </summary>
    /// <param name="json">The file's contents.</param>
    /// <returns>The state, or null when the text is not a state file.</returns>
    public static CalculatorState? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<CalculatorState>(json, JsonOptions);
        }
        catch (JsonException)
        {
            // A file someone picked by mistake is an everyday event, not an error worth a stack trace.
            return null;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    // ---------------------------------------------------------------- link

    /// <summary>
    /// Encodes the state into a URL fragment, compactly.
    /// </summary>
    /// <param name="catalogue">
    /// The full salt catalogue in its display order. Salts are stored as positions in it, which is what
    /// keeps a link short — names would be twenty to thirty characters each.
    /// </param>
    /// <returns>The fragment, without a leading <c>#</c>.</returns>
    /// <remarks>
    /// The catalogue size travels with the indices. If a later version of the library adds or removes a
    /// salt, the positions no longer mean what they meant, and <see cref="FromFragment"/> can say so
    /// instead of silently selecting the wrong salts.
    /// </remarks>
    public string ToFragment(IReadOnlyList<string> catalogue)
    {
        ArgumentNullException.ThrowIfNull(catalogue);

        StringBuilder builder = new();
        builder.Append("v=2");

        if (!string.IsNullOrWhiteSpace(Target))
        {
            builder.Append("&t=").Append(Uri.EscapeDataString(Target));
        }

        // Only what was actually entered. Sixteen zeroes would triple the length of a link for water
        // nobody described.
        IEnumerable<string> water = Water
            .Where(pair => pair.Value != 0)
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => $"{pair.Key}:{pair.Value.ToString("G", CultureInfo.InvariantCulture)}");

        string waterPart = string.Join(",", water);
        if (waterPart.Length > 0)
        {
            builder.Append("&w=").Append(Uri.EscapeDataString(waterPart));
        }

        // The shelf is stored as what is *missing*, because a first-time visitor has everything ticked
        // and the common case should add nothing to the link at all.
        List<int> excluded = [];
        for (int index = 0; index < catalogue.Count; index++)
        {
            if (!Salts.Contains(catalogue[index], StringComparer.Ordinal))
            {
                excluded.Add(index);
            }
        }

        builder.Append("&n=").Append(catalogue.Count);
        if (excluded.Count > 0)
        {
            builder.Append("&x=").Append(string.Join(".", excluded));
        }

        if (ConcentrateLiters is { } litres)
        {
            builder.Append("&c=").Append(litres.ToString("G", CultureInfo.InvariantCulture));
        }

        // Version 2. Each key is written only when it carries something, following the rule the older
        // keys already follow: a link has to stay short enough to paste into a chat window.
        Append(builder, "wm", WaterMode);
        Append(builder, "wp", WaterPresetId);
        Append(builder, "we", WaterEc);
        Append(builder, "wu", WaterEcUnit);
        Append(builder, "wg", WaterGh);
        Append(builder, "wk", WaterKh);

        if (AcidEnabled is true)
        {
            builder.Append("&ae=1");
        }

        Append(builder, "ay", AcidId);
        Append(builder, "ap", TargetPh);
        Append(builder, "aw", WaterPh);

        // One entry per custom salt, rather than one packed key, so a link stays legible when it is
        // truncated in a chat window and so a salt can be lost without taking the others with it.
        foreach (CustomSalt salt in CustomSalts)
        {
            string forms = string.Join(
                ";",
                salt.Percentages.Where(p => p.Value > 0)
                    .OrderBy(p => p.Key, StringComparer.Ordinal)
                    .Select(p => $"{p.Key}:{p.Value.ToString("R", CultureInfo.InvariantCulture)}"));

            string entry = string.Join('~', salt.Name, salt.Formula ?? string.Empty, salt.Tank, forms);
            builder.Append("&cs=").Append(Uri.EscapeDataString(entry));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Decodes a URL fragment written by <see cref="ToFragment"/>.
    /// </summary>
    /// <param name="fragment">The fragment, with or without a leading <c>#</c>.</param>
    /// <param name="catalogue">The full salt catalogue in its display order.</param>
    /// <returns>
    /// The state and whether the shelf survived. <c>SaltsUsable</c> is false when the link was written
    /// against a different catalogue — the rest of the state is still good, and the caller should say
    /// the salt selection was dropped rather than apply positions that now point elsewhere.
    /// </returns>
    public static (CalculatorState? State, bool SaltsUsable) FromFragment(
        string? fragment,
        IReadOnlyList<string> catalogue)
    {
        ArgumentNullException.ThrowIfNull(catalogue);

        if (string.IsNullOrWhiteSpace(fragment))
        {
            return (null, true);
        }

        Dictionary<string, string> parts = new(StringComparer.Ordinal);

        // Custom salts are the one repeated key, so they are collected as they go by. The dictionary
        // below keeps only the last value for a key, which would silently drop all but one.
        List<string> customEntries = [];

        foreach (string pair in fragment.TrimStart('#').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            int split = pair.IndexOf('=', StringComparison.Ordinal);
            if (split <= 0)
            {
                continue;
            }

            string key = pair[..split];
            string value = Uri.UnescapeDataString(pair[(split + 1)..]);

            if (key == "cs")
            {
                customEntries.Add(value);
            }
            else
            {
                parts[key] = value;
            }
        }

        // Both versions are read. The keys were always optional, so a reader that rejected a version
        // it did not recognise would be throwing away a link it could in fact understand.
        if (!parts.TryGetValue("v", out string? version) || (version != "1" && version != "2"))
        {
            return (null, true);
        }

        CalculatorState state = new()
        {
            Target = parts.GetValueOrDefault("t", string.Empty),
            WaterMode = parts.GetValueOrDefault("wm"),
            WaterPresetId = parts.GetValueOrDefault("wp"),
            WaterEc = Number(parts, "we"),
            WaterEcUnit = parts.GetValueOrDefault("wu"),
            WaterGh = Number(parts, "wg"),
            WaterKh = Number(parts, "wk"),
            AcidEnabled = parts.ContainsKey("ae") ? true : null,
            AcidId = parts.GetValueOrDefault("ay"),
            TargetPh = Number(parts, "ap"),
            WaterPh = Number(parts, "aw"),
        };

        foreach (string entry in customEntries)
        {
            string[] fields = entry.Split('~');
            if (fields.Length < 3)
            {
                continue;
            }

            CustomSalt salt = new()
            {
                Name = fields[0],
                Formula = string.IsNullOrWhiteSpace(fields[1]) ? null : fields[1],
                Tank = Enum.TryParse(fields[2], out ConcentrateType tank) ? tank : ConcentrateType.A,
            };

            if (fields.Length > 3)
            {
                foreach (string form in fields[3].Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] halves = form.Split(':', 2);
                    if (halves.Length == 2 &&
                        double.TryParse(halves[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double percent))
                    {
                        salt.Percentages[halves[0]] = percent;
                    }
                }
            }

            state.CustomSalts.Add(salt);
        }

        if (parts.TryGetValue("w", out string? water))
        {
            foreach (string entry in water.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] halves = entry.Split(':', 2);
                if (halves.Length == 2 &&
                    double.TryParse(halves[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    state.Water[halves[0]] = value;
                }
            }
        }

        if (parts.TryGetValue("c", out string? concentrate) &&
            double.TryParse(concentrate, NumberStyles.Float, CultureInfo.InvariantCulture, out double litres))
        {
            state.ConcentrateLiters = litres;
        }

        bool sizeMatches = parts.TryGetValue("n", out string? size) &&
            int.TryParse(size, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count) &&
            count == catalogue.Count;

        if (!sizeMatches)
        {
            state.Salts = [.. catalogue];
            return (state, false);
        }

        HashSet<int> excluded = [];
        if (parts.TryGetValue("x", out string? raw))
        {
            foreach (string index in raw.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(index, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
                {
                    excluded.Add(parsed);
                }
            }
        }

        state.Salts = [.. catalogue.Where((_, index) => !excluded.Contains(index))];
        return (state, true);
    }

    private static void Append(StringBuilder builder, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            builder.Append('&').Append(key).Append('=').Append(Uri.EscapeDataString(value));
        }
    }

    // "R" rather than "G": a round-trippable form, so a meter reading typed to two decimals comes back
    // as the same double rather than as one a hair below it.
    private static void Append(StringBuilder builder, string key, double? value)
    {
        if (value is { } number)
        {
            builder.Append('&').Append(key).Append('=')
                .Append(number.ToString("R", CultureInfo.InvariantCulture));
        }
    }

    private static double? Number(Dictionary<string, string> parts, string key) =>
        parts.TryGetValue(key, out string? raw) &&
        double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            ? value
            : null;
}
