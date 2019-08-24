using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer
{
    public class KurirDbContextFactory : IDesignTimeDbContextFactory<KurirDbContext>
    {
        public KurirDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            return new KurirDbContext(new DbContextOptionsBuilder<KurirDbContext>().Options, config);
        }
    }
}
