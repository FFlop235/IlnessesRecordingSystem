using System;

namespace IllnessesRecordingSystem.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int DepartmentId { get; set; }
    public string Position { get; set; }
    public DateTime HireDate { get; set; }
}