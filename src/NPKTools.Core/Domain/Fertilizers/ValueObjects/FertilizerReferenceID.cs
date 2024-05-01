using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents a unique identifier for referencing a specific fertilizer entity within the system.
/// This ID is essential for linking the incoming fertilizer data to the corresponding outgoing fertilizer,
/// ensuring that each fertilizer input is accurately associated with its relevant output processes.
/// </summary>
public record FertilizerReferenceId
{
    public Guid Value { get; }

    public FertilizerReferenceId(Guid value)
    {
        ThrowIf.Default(value);
        Value = value;
    }
}