using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record MagnesiumPpm(double Value) : ElementFieldBase(Value);