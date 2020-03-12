using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Stock.Model;

namespace Stock.Data
{
    public class StockContext: DbContext
    {

        public DbSet<Securities> Securities { get; set; }
        public DbSet<Price1d> Price1d { get; set; }


        public StockContext(DbContextOptions<StockContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //获取历史数据时，以单个股票为单位。
            modelBuilder.Entity<Price1d>().HasKey(t => new
            {
                t.Code,
                t.Date
            });

        }


        }
    }
