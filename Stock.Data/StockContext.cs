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


        public StockContext(DbContextOptions<StockContext> options) : base(options) { }



    }
}
