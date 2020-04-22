using Microsoft.EntityFrameworkCore;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyStock.Data
{
    public class StockContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Stock> StockSet { get; set; }

        public DbSet<DayData> DayDataSet { get; set; }


        public DbSet<Sharing> SharingSet { get; set; }
        public DbSet<RealTimeData> RealTimeDataSet { get; set; }

        public DbSet<StockNum> StockNumSet { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<StockEvent> StockEvents { get; set; }
        public DbSet<SearchResult> SearchResultSet { get; set; }
        public DbSet<MarginTotal> MarginTotal { get; set; }
        public DbSet<MarketDeal> MarketDeal { get; set; }
        public DbSet<StaPrice> StaPrice { get; set; }





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
            modelBuilder.Entity<DayData>().HasKey(t => new
            {
                t.StockId,
                t.Date
            });

            //获取实时数据时，以最新地时间为重点
            modelBuilder.Entity<RealTimeData>().HasKey(t => new
            {

                t.StockId,
                t.Date,
            });
            modelBuilder.Entity<Sharing>().HasKey(t => new
            {
                t.StockId,
                t.DateGongGao
            });
            modelBuilder.Entity<StockNum>().HasKey(t => new
            {
                t.StockId,
                t.Date
            });

            modelBuilder.Entity<SearchResult>().HasKey(t => new
            {
                t.ActionName,
                t.ActionParams,
                t.ActionDate,
            });

            modelBuilder.Entity<MarketDeal>().HasKey(t => new
            {
                t.MarketType,
                t.Date,
            });


            ConfigureModelBuilderForUser(modelBuilder);

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
            await this.Database.ExecuteSqlRawAsync("truncate table RealTimeDataSet");
            await this.Database.ExecuteSqlRawAsync("truncate table SearchResultSet");

        }

    }
}
