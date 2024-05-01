using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record ChlorinePpm(double Value) : ElementFieldBase(Value);