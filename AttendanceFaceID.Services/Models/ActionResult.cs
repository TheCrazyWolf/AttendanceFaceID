namespace AttendanceFaceID.Services.Models;

public class ActionResult
{
    public bool IsSuccess { get; set; }
    public string SystemMessage { get; set; } = string.Empty;

    public ActionResult()
    {
        
    }

    public ActionResult(bool isSuccess, string systemMessage)
    {
        IsSuccess = isSuccess;
        SystemMessage = systemMessage;
    }
}
