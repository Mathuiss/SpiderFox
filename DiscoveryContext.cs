using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InternetExplorer
{
    class DiscoveryContext : DbContext
    {
        public DbSet<Discovery> Discoverys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=internetexplorer.db;");
        }
    }
}
