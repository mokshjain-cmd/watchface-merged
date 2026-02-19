using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDB
{
    public class WatchDataContext: DbContext
    {
        public WatchDataContext(string path) 
        {
          this.Path = path;
        }

        public string Path { get; set; }
        public DbSet<WatchInfo> WatchInfos { get; set; }

        public DbSet<WatchType> WatchTypes { get; set; }

        public DbSet<WatchGroup> WatchGroups { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WatchInfo>().HasMany(b => b.WatchTypes).WithOne(w => w.WatchInfo);
           
            modelBuilder.Entity<WatchType>().HasMany(b => b.WatchGroups).WithOne(w => w.WatchType);

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($@"Data Source={Path}\watch.db;");
            }
        }


    }
}
