
using NPKTools.Core.Domain.PpmTarget;

namespace NPKTools.Optimizer.PpmTargetParser;

/// <summary>
/// Defines a contract for a parser that converts a string of element-value pairs into a PpmTarget object.
/// </summary>
public interface IPpmTargetParser
{
    /// <summary>
    /// Parses a given string containing elements and their corresponding values expressed in parts per million (ppm)
    /// into a PpmTarget object. The input must be in the format "element=value", separated by spaces or commas.
    /// </summary>
    /// <param name="input">The input string to parse, containing element-value pairs.</param>
    /// <returns>A PpmTarget object populated with the parsed values.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="FormatException">Thrown if the input is not in the correct format, contains unrecognized elements, or has duplicate elements.</exception>
    PpmTarget Parse(string input);
}