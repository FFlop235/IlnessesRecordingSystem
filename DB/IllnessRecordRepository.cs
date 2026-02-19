using System;
using System.Collections.Generic;
using IlnessesRecordingSystem.Models;
using MySqlConnector;

namespace IlnessesRecordingSystem.DB;

public class IllnessRecordRepository: BaseRepository<IllnessRecordViem>, IDisposable
{
    public IllnessRecordRepository()
    {
        OpenConnection();
    }

    public override List<IllnessRecordViem> GetPage(int pageIndex, int pageSize)
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