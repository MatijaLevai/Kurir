using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kurir.Persistance
{
    public interface ISQLiteHelper
    {
        Task<bool> UpdateSQLiteDbWithPayAndDelTypes();
    }
}