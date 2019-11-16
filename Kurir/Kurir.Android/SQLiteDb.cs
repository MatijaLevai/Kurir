using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Environment = System.Environment;
using Xamarin.Forms;
using Kurir.Droid;
using Kurir.Persistance;
using System.Security;

[assembly: Xamarin.Forms.Dependency(typeof(SQLiteDb))]
namespace Kurir.Droid
{
    class SQLiteDb : ISQLiteDb
    {
        [SecurityCritical]
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MySQLite.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}