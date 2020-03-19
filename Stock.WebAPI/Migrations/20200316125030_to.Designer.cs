﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stock.Data;

namespace Stock.WebAPI.Migrations
{
    [DbContext(typeof(StockContext))]
    [Migration("20200316125030_to")]
    partial class to
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Stock.Model.Message", b =>
                {
                    b.Property<DateTime>("MesTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.HasKey("MesTime");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Stock.Model.Price", b =>
                {
                    b.Property<byte>("Unit")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("Code")
                        .HasColumnType("varchar(15) CHARACTER SET utf8mb4")
                        .HasMaxLength(15);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Avg")
                        .HasColumnType("double");

                    b.Property<double>("Close")
                        .HasColumnType("double");

                    b.Property<double>("High")
                        .HasColumnType("double");

                    b.Property<double>("Highlimit")
                        .HasColumnType("double");

                    b.Property<double>("Low")
                        .HasColumnType("double");

                    b.Property<double>("Lowlimit")
                        .HasColumnType("double");

                    b.Property<double>("Money")
                        .HasColumnType("double");

                    b.Property<double>("Open")
                        .HasColumnType("double");

                    b.Property<bool>("Paused")
                        .HasColumnType("tinyint(1)");

                    b.Property<double>("Preclose")
                        .HasColumnType("double");

                    b.Property<double>("Volume")
                        .HasColumnType("double");

                    b.HasKey("Unit", "Code", "Date");

                    b.ToTable("PriceSet");
                });

            modelBuilder.Entity("Stock.Model.SearchResult", b =>
                {
                    b.Property<string>("ActionName")
                        .HasColumnType("varchar(32) CHARACTER SET utf8mb4")
                        .HasMaxLength(32);

                    b.Property<string>("ActionParams")
                        .HasColumnType("varchar(512) CHARACTER SET utf8mb4")
                        .HasMaxLength(512);

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ActionReslut")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(4096);

                    b.HasKey("ActionName", "ActionParams", "ActionDate");

                    b.ToTable("SearchResultSet");
                });

            modelBuilder.Entity("Stock.Model.Securities", b =>
                {
                    b.Property<byte>("Type")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("Code")
                        .HasColumnType("varchar(15) CHARACTER SET utf8mb4")
                        .HasMaxLength(15);

                    b.Property<string>("Displayname")
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Type", "Code");

                    b.HasIndex("Code");

                    b.ToTable("SecuritiesSet");
                });

            modelBuilder.Entity("Stock.Model.StockEvent", b =>
                {
                    b.Property<string>("EventName")
                        .HasColumnType("varchar(30) CHARACTER SET utf8mb4")
                        .HasMaxLength(30);

                    b.Property<DateTime?>("LastAriseEndDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(null);

                    b.Property<DateTime>("LastAriseStartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("EventName");

                    b.ToTable("StockEvents");
                });

            modelBuilder.Entity("Stock.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(40) CHARACTER SET utf8mb4")
                        .HasMaxLength(40);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(60) CHARACTER SET utf8mb4")
                        .HasMaxLength(60);

                    b.Property<DateTime>("ExpiredDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(90) CHARACTER SET utf8mb4")
                        .HasMaxLength(90);

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10)
                        .HasDefaultValue("user");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}