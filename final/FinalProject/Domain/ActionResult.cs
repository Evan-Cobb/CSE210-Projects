namespace FinalProject.Domain;

public sealed class ActionResult
{
    public bool Success { get; }
    public string Message { get; }

    public ActionResult(bool success, string message)
    {
        Success = success;
        Message = message ?? string.Empty;
    }

    public static ActionResult Ok(string message = "") => new ActionResult(true, message);
    public static ActionResult Fail(string message) => new ActionResult(false, message);
}
