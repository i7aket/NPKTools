using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record SulfurPpm(double Value) : ElementFieldBase(Value);