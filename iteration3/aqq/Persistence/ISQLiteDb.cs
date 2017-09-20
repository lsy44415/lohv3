using SQLite;

namespace aqq
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}

