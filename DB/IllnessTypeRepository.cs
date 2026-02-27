using System;
using System.Collections.Generic;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class IllnessTypeRepository: BaseRepository<IllnessType>, IDisposable
{
    public IllnessTypeRepository()
    {
        OpenConnection();
    }

    public override void Add(IllnessType entity)
    {
        throw new NotImplementedException();
    }

    public override void Update(IllnessType entity)
    {
        throw new NotImplementedException();
    }

    public override void Delete(IllnessType entity)
    {
        throw new NotImplementedException();
    }

    public override IllnessType GetById(int id)
    {
        var cmd = new MySqlCommand(@"SELECT Id, Name FROM IllnessTypes WHERE Id = @id", connection);
        
        cmd.Parameters.AddWithValue("@id", id);

        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return new IllnessType
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                };
            }
            return null;
        }
    }

    public override List<IllnessType> GetAll()
    {
        var result = new List<IllnessType>();
        
        var cmd = new MySqlCommand(@"SELECT Id, Name FROM IllnessTypes", connection);
        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new IllnessType
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                });
            }
        }
        
        return result;
    }

    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}