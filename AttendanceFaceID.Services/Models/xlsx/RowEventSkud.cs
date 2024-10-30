namespace AttendanceFaceID.Services.Models.xlsx;

public class RowEventSkud
{
    public string DateTime { get; set; } = string.Empty;
    public string ObjectInit { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string FaceMode { get; set; } = string.Empty;
    
    public RowEventSkud() { }

    public RowEventSkud(string dateTime, string objectInit, string shortName, string groupName, string faceMode)
    {
        DateTime = dateTime;
        ObjectInit = objectInit;
        ShortName = shortName;
        GroupName = groupName;
        FaceMode = faceMode;
    }
}