using System.Collections.Generic;

namespace IllnessesRecordingSystem.DB;

public interface IPaginatedRepository<T> : IRepository<T> where T: class
{
    List<T> GetPage(int pageIndex, int pageSize);
    int GetRowsCount();
}