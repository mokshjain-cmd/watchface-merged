
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDB;
using WatchDB_DIY;

namespace WatchDBDIYJieLi
{
    public class WatchDataContext_DIYJieLi : DbContext
    {
        public WatchDataContext_DIYJieLi(string path)
        {
            this.Path = path;
        }

        public string Path { get; set; }
        public DbSet<WatchInfo_DIYJieLi> WatchInfos { get; set; }

        public DbSet<WatchType> WatchTypes { get; set; }

        public DbSet<WatchGroup> WatchGroups { get; set; }

        public DbSet<WatchLocation> WatchLocations { get; set; }

        public DbSet<LocationDetail> LocationDetails { get; set; }

        public DbSet<WatchTime> WatchTimes { get; set; }

        public DbSet<TimeGroup> TimeGroups { get; set; }

        public DbSet<WatchPointer> WatchPointers { get; set; }

        public DbSet<PointerGroup> PointerGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchTypes);
            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchTimes);
            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchPointers);

            modelBuilder.Entity<WatchType>().HasMany(b => b.WatchGroups).WithOne(w => w.WatchType);

            modelBuilder.Entity<WatchLocation>().HasMany(b => b.LocationDetails).WithOne(w => w.WatchLocation);

            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchLocations).WithOne(w => (WatchInfo_DIYJieLi)w.WatchInfo);

            modelBuilder.Entity<WatchTime>().HasMany(b => b.TimeGroups).WithOne(w => w.WatchTime);
            modelBuilder.Entity<WatchPointer>().HasMany(b => b.PointerGroups).WithOne(w => w.WatchPointer);

            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchTimes).WithOne(w => (WatchInfo_DIYJieLi)w.WatchInfo);
            modelBuilder.Entity<WatchInfo_DIYJieLi>().HasMany(b => b.WatchPointers).WithOne(w => w.WatchInfo);

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
