namespace NPKOptimizer.Common;
public sealed class ActionResult<T>
{
    public bool IsSuccess { get; private set; }
    public T Payload { get; private set; }
    public string ErrorMessage { get; private set; }
    private ActionResult() { }
    public static ActionResult<T> Success(T payload) => new ()
    {
        IsSuccess = true,
        Payload = payload
    };
    public static ActionResult<T> Fail(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}