using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Persistance
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
