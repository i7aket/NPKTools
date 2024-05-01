using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record SodiumPpm(double Value) : ElementFieldBase(Value);