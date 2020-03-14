using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Stock.Model;

namespace Stock.Data
{
    public class StockContext : DbContext
    {

        public DbSet<Securities> SecuritiesSet { get; set; }
        public DbSet<Price> PriceSet { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=StockQuantization;user=root;password=dragon00");
            }

        }

        public StockContext(DbContextOptions<StockContext> options) : base(options) { }

        public StockContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //获取历史数据时，以单个股票为单位。
            modelBuilder.Entity<Price>().HasKey(t => new
            {
                t.Unit,
                t.Code,
                t.Date
            });

            modelBuilder.Entity<Securities>().HasKey(t => new
            {
                t.Type,
                t.Code,
            });


            ConfigureModelBuilderForPrice(modelBuilder);
        }

        void ConfigureModelBuilderForPrice(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().Property(s => s.Unit).HasConversion<byte>();
            modelBuilder.Entity<Securities>().Property(s => s.Type).HasConversion<byte>();
        }
          

    }
}
