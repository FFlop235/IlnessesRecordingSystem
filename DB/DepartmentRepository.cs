using System;
using System.Collections.Generic;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class DepartmentRepository: BaseRepository<Department>, IDisposable
{
    
    public DepartmentRepository()
    {
        OpenConnection();
    }

    public override Department GetById(int id)
    {
        var cmd = new MySqlCommand("SELECT Id, Name, Floor FROM Departments WHERE Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", id);

        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return new Department
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                };
            }
            return null;
        }
    }

    public override List<Department> GetAll()
    {
        var result = new List<Department>();

        var cmd = new MySqlCommand("SELECT Id, Name, Floor FROM Departments", connection);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new Department
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Floor = reader.GetInt32("Floor")
                });
            }
        }
        
        return result;
    }

    public override void Add(Department entity)
    {
        throw new NotImplementedException();
    }

    public override void Update(Department entity)
    {
        throw new NotImplementedException();
    }

    public override void Delete(Department entity)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}