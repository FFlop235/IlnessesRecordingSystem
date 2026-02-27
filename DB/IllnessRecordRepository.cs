using System;
using System.Collections.Generic;
using System.Xml;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class IllnessRecordRepository: BaseRepository<IllnessRecordViem>, IPaginatedRepository<IllnessRecordViem>, IDisposable
{
    public IllnessRecordRepository()
    {
        OpenConnection();
    }

    public override IllnessRecordViem GetById(int id)
    {
        var cmd = new MySqlCommand(@"
            SELECT
                ir.Id,
                e.FullName AS EmployeeName,
                d.Name AS DepartmentName,
                it.Name AS IllnessType,
                ir.StartDate,
                ir.EndDate,
                DATEDIFF(ir.EndDate, ir.StartDate) AS DurationDays
                ir.DiagnosisNote
            FROM IllnessRecords ir
            JOIN Employees e ON ir.EmployeeId = e.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            WHERE ir.Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", id);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new IllnessRecordViem
            {
                Id = reader.GetInt32("Id"),
                EmployeeName = reader.GetString("EmployeeName"),
                DepartmentName = reader.GetString("DepartmentName"),
                IllnessType = reader.GetString("IllnessType"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
                DurationDays = reader.GetInt32("DurationDays"),
                DiagnosisNote = reader.IsDBNull(reader.GetOrdinal("DiagnosisNote"))
                    ? null
                    : reader.GetString("DiagnosisNote")
            };
        }

        return null;
    }

    public override IEnumerable<IllnessRecordViem> GetAll()
    {
        var result = new List<IllnessRecordViem>();

        var cmd = new MySqlCommand(@"
            SELECT
                ir.Id,
                e.FullName AS EmployeeName,
                d.Name AS DepartmentName,
                it.Name AS IllnessType,
                ir.StartDate,
                ir.EndDate,
                ir.DiagnosisNote
                DATEDIFF(ir.EndDate, ir.StartDate) AS DurationDays
            FROM IllnessRecords ir
            JOIN Employees e ON ir.EmployeeId = e.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            ORDER BY ir.StartDate DESC", connection);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new IllnessRecordViem
            {
                Id = reader.GetInt32("Id"),
                EmployeeName = reader.GetString("EmployeeName"),
                DepartmentName = reader.GetString("DepartmentName"),
                IllnessType = reader.GetString("IllnessType"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
                DurationDays = reader.GetInt32("DurationDays"),
                DiagnosisNote = reader.IsDBNull(reader.GetOrdinal("DiagnosisNote"))
                    ? null
                    : reader.GetString("DiagnosisNote")
            });
        }
        
        return result;
    }

    public override void Add(IllnessRecordViem entity)
    {
        var cmd = new MySqlCommand(@"
            INSERT INTO IllnessRecords (EmployeeId, IllnessTypeId, StartDate, EndDate, DiagnosisNote)
            VALUES (@employeeId, @illnessTypeId, @startDate, @endDate, @diagnosisNote)", connection);
        
        cmd.Parameters.AddWithValue("@employeeId", GetEmployeeIdByName(entity.EmployeeName));
        cmd.Parameters.AddWithValue("@illnessTypeId", GetIllnessTypeIdByName(entity.IllnessType));
        cmd.Parameters.AddWithValue("@startDate", entity.StartDate);
        cmd.Parameters.AddWithValue("@endDate", entity.EndDate);
        cmd.Parameters.AddWithValue("@diagnosisNote", entity.DiagnosisNote ?? (object)DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }

    public override void Update(IllnessRecordViem entity)
    {
        var cmd = new MySqlCommand(@"
            UPDATE  IllnessRecords
            SET EmployeeId = @employeeId,
                IllnessTypeId = @illnessTypeId,
                StartDate = @startDate,
                EndDate = @endDate
                DiagnosisNote = @diagnosisNote
            WHERE Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", entity.Id);
        cmd.Parameters.AddWithValue("@employeeId", GetEmployeeIdByName(entity.EmployeeName));
        cmd.Parameters.AddWithValue("@illnessTypeId", GetIllnessTypeIdByName(entity.IllnessType));
        cmd.Parameters.AddWithValue("@startDate", entity.StartDate);
        cmd.Parameters.AddWithValue("@endDate", entity.EndDate);
        cmd.Parameters.AddWithValue("@diagnosisNote", entity.DiagnosisNote ?? (object)DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }

    public override void Delete(IllnessRecordViem entity)
    {
        var cmd = new MySqlCommand("DELETE FROM IllnessRecords WHERE Id = @id", connection);
        cmd.Parameters.AddWithValue("@id", entity.Id);
        cmd.ExecuteNonQuery();
    }

    public List<IllnessRecordViem> GetPage(int pageIndex, int pageSize)
    {
        var result = new List<IllnessRecordViem>();
        
            var cmd = new MySqlCommand(@"
            SELECT 
                ir.Id,
                e.FullName AS EmployeeName,
                d.Name AS DepartmentName,
                it.Name AS IllnessType,
                ir.StartDate,
                ir.EndDate,
                DATEDIFF(ir.EndDate, ir.StartDate) AS DurationDays
            FROM IllnessRecords ir
            JOIN Employees e ON ir.EmployeeId = e.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            ORDER BY ir.StartDate DESC
            LIMIT @pageSize OFFSET @offset", connection);
            
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            cmd.Parameters.AddWithValue("@offset", (pageIndex - 1) * pageSize);
        
            using (var reader = cmd.ExecuteReader()) 
            {
                while (reader.Read())
                {
                    result.Add(new IllnessRecordViem
                    {
                        Id = reader.GetInt32("Id"),
                        EmployeeName = reader.GetString("EmployeeName"),
                        DepartmentName = reader.GetString("DepartmentName"),
                        IllnessType = reader.GetString("IllnessType"),
                        StartDate = reader.GetDateTime("StartDate"),
                        EndDate = reader.GetDateTime("EndDate"),
                        DurationDays = reader.GetInt32("DurationDays")
                    });
                }
            }
    
        return result;
    }

    public int GetEmployeeIdByName(string employeeName)
    {
        var cmd = new MySqlCommand(@"SELECT Id FROM Employees WHERE FullName = @employeeName", connection);
        
        cmd.Parameters.AddWithValue("@employeeName", employeeName);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32("Id");
        }

        // DEBUG
        Console.WriteLine($"Employee not found: {employeeName}");
        return 0;
    }

    public int GetIllnessTypeIdByName(string illnessTypeName)
    {
        var cmd = new MySqlCommand(@"SELECT Id FROM IllnessTypes WHERE Name = @illnessTypeName", connection);
        
        cmd.Parameters.AddWithValue("@illnessTypeName", illnessTypeName);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32("Id");
        }
        
        // DEBUG
        Console.WriteLine($"Illness type not found: {illnessTypeName}");
        return 0;
    }

    public int GetRowsCount()
    {
        string sql = "SELECT COUNT(Id) FROM IllnessRecords";
        try
        {
            using var mc = new MySqlCommand(sql, connection);
            return Convert.ToInt32(mc.ExecuteScalar());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 0;
        }
    }
    
    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}