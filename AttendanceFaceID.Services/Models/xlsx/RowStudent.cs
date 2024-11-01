namespace AttendanceFaceID.Services.Models.xlsx;

public class RowStudent
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    
    public RowStudent() { }

    public RowStudent(string lastName, string firstName, string middleName, string group)
    {
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Group = group;
    }
    
    public string GetShortName()
    {
        return $"{LastName.ToUpper()} {FirstName.ToUpper().First()}.{MiddleName.ToUpper().First()}.";
    }
}