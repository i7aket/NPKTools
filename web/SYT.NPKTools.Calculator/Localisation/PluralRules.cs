namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// Chooses which plural form a count needs.
/// </summary>
/// <remarks>
/// <para>
/// "8 recipes" is one word in English and three different words in Russian, Ukrainian and Polish:
/// 1 рецепт, 2 рецепта, 5 рецептов. A dictionary lookup alone produces "5 рецепт", which is the
/// clearest sign of a localisation nobody checked.
/// </para>
/// <para>
/// The rules follow CLDR for integers, which is all this app counts — recipes, salts, elements. The
/// boundaries are the unintuitive part and are worth stating: 21 behaves like 1 in Russian but not in
/// Polish, and 11 behaves like neither in either.
/// </para>
/// </remarks>
public static class PluralRules
{
    /// <summary>
    /// The plural form a count needs in a language.
    /// </summary>
    /// <param name="language">The two-letter language tag.</param>
    /// <param name="count">How many.</param>
    /// <returns>One of <c>one</c>, <c>few</c>, <c>many</c> or <c>other</c>.</returns>
    public static string Select(string language, long count)
    {
        long n = Math.Abs(count);
        long mod10 = n % 10;
        long mod100 = n % 100;

        return language switch
        {
            "ru" or "uk" => mod10 == 1 && mod100 != 11 ? "one"
                : mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "few"
                : "many",

            // Polish keeps "one" for exactly one, so 21 is "many" where Russian would say "one".
            "pl" => n == 1 ? "one"
                : mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "few"
                : "many",

            _ => n == 1 ? "one" : "other",
        };
    }
}
