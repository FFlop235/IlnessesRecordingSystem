using System;
using System.Collections.Generic;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class EmployeeRepository: BaseRepository<Employee>, IDisposable
{
    public EmployeeRepository()
    {
        OpenConnection();
    }

    public override Employee GetById(int id)
    {
        var cmd = new MySqlCommand(@"
            SELECT Id, FullName, DepartmentId, Position, HireDate
            FROM Employees
            WHERE Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", id);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Employee
            {
                Id = reader.GetInt32("Id"),
                FullName = reader.GetString("FullName"),
                DepartmentId = reader.GetInt32("DepartmentId"),
                Position = reader.GetString("Position"),
                HireDate = reader.GetDateTime("HireDate")
            };
        }
        
        return null;
    }

    public override List<Employee> GetAll()
    {
        var result = new List<Employee>();
        
        var cmd = new MySqlCommand(@"
            SELECT Id, FullName, DepartmentId, Position, HireDate
            FROM Employees
            ORDER BY FullName", connection);
        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new Employee
                {
                    Id = reader.GetInt32("Id"),
                    FullName = reader.GetString("FullName"),
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    Position = reader.GetString("Position"),
                    HireDate = reader.GetDateTime("HireDate")
                });
            }
        }
        
        return result;
    }

    public override void Add(Employee entity)
    {
        var cmd = new MySqlCommand(@"
            INSERT INTO Employees (FullName, DepartmentId, Position, HireDate)
            VALUES (@fullName, @departmentId, @position, @hireDate)", connection);
        
        cmd.Parameters.AddWithValue("@fullName", entity.FullName);
        cmd.Parameters.AddWithValue("@departmentId", entity.DepartmentId);
        cmd.Parameters.AddWithValue("@position", entity.Position);
        cmd.Parameters.AddWithValue("@hireDate", entity.HireDate);
        
        cmd.ExecuteNonQuery();
    }
    
    public override void Update(Employee entity)
    {
        var cmd = new MySqlCommand(@"
            UPDATE Employees
            SET  FullName = @fullName, 
                 DepartmentId = @departmentId, 
                 Position = @position,
                 HireDate = @hireDate
            WHERE Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", entity.Id);
        cmd.Parameters.AddWithValue("@fullName", entity.FullName);
        cmd.Parameters.AddWithValue("@departmentId", entity.DepartmentId);
        cmd.Parameters.AddWithValue("@position", entity.Position);
        cmd.Parameters.AddWithValue("@hireDate", entity.HireDate);
        
        cmd.ExecuteNonQuery();
    }
    
    public override void Delete(Employee entity)
    {
        var cmd = new  MySqlCommand(@"DELETE FROM Employees WHERE Id = @id", connection);
        cmd.Parameters.AddWithValue("@id", entity.Id);
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}