using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record NitrogenPpmTarget(double Value) : ElementFieldBase(Value);