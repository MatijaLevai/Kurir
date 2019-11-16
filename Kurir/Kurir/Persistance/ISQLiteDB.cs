using SQLite;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Kurir.Persistance
{
    public interface ISQLiteDb
    {
        [SecurityCritical]
        SQLiteAsyncConnection GetConnection();
    }
}
