using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        builder.Append("v=1");

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
        foreach (string pair in fragment.TrimStart('#').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            int split = pair.IndexOf('=', StringComparison.Ordinal);
            if (split > 0)
            {
                parts[pair[..split]] = Uri.UnescapeDataString(pair[(split + 1)..]);
            }
        }

        if (!parts.TryGetValue("v", out string? version) || version != "1")
        {
            return (null, true);
        }

        CalculatorState state = new()
        {
            Target = parts.GetValueOrDefault("t", string.Empty),
        };

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
}
