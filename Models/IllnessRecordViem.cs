using System;

namespace IllnessesRecordingSystem.Models;

public class IllnessRecordViem
{
    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public string DepartmentName { get; set; }
    public string IllnessType  { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays {get; set;}
    
    public string? DiagnosisNote { get; set; }
}