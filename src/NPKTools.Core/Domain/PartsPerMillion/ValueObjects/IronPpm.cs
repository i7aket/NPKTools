using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record IronPpm(double Value) : ElementFieldBase(Value);