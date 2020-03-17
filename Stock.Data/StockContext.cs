using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Stock.Model;
using System.Threading.Tasks;

namespace Stock.Data
{
    public class StockContext : DbContext
    {

        public DbSet<Securities> SecuritiesSet { get; set; }
        public DbSet<Price> PriceSet { get; set; }
        public DbSet<TempPrice> TempPrice { get; set; }
        public DbSet<User> Users { get; set; }


        public DbSet<Message> Messages { get; set; }
        public DbSet<StockEvent> StockEvents { get; set; }
        public DbSet<SearchResult> SearchResultSet { get; set; }



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

            modelBuilder.Entity<TempPrice>().HasKey(t => new
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
            modelBuilder.Entity<Securities>().HasIndex(t => new
            {
                t.Code,
            });

            modelBuilder.Entity<SearchResult>().HasKey(t => new
            {
                t.ActionName,
                t.ActionParams,
                t.ActionDate,
            });


            ConfigureModelBuilderForPrice(modelBuilder);
            ConfigureModelBuilderForUser(modelBuilder);
        }

        void ConfigureModelBuilderForPrice(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().Property(s => s.Unit).HasConversion<byte>();
            modelBuilder.Entity<Securities>().Property(s => s.Type).HasConversion<byte>();
        }

        void ConfigureModelBuilderForUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>()
                .Property(user => user.Username)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(user => user.Email)
                .HasMaxLength(60)
                .IsRequired();

            modelBuilder.Entity<User>()
        .Property(user => user.RoleName)
        .HasMaxLength(10)
        .IsRequired().HasDefaultValue("user");


            modelBuilder.Entity<User>()
            .Property(user => user.ExpiredDate)
            .IsRequired().HasDefaultValue(DateTime.MinValue);




            modelBuilder.Entity<User>()
               .Property(user => user.Id)
               .HasMaxLength(40)
               .IsRequired();


            modelBuilder.Entity<User>()
               .Property(user => user.Password)
               .HasMaxLength(90)
               .IsRequired();

            modelBuilder.Entity<StockEvent>()
                .Property(s => s.LastAriseEndDate)
                .IsRequired(false)
                .HasDefaultValue(null);
        }

        public async Task TruncateRealTimeAndCacheTable()
        {
            //await this.Database.ExecuteSqlCommandAsync("truncate table RealTimeDataSet");
            await this.Database.ExecuteSqlRawAsync("truncate table SearchResultSet");
            await this.Database.ExecuteSqlRawAsync("truncate table TempPrice");

        }

    }
}
