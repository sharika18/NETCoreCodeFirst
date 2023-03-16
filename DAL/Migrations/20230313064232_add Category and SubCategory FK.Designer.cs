﻿// <auto-generated />
using System;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20230313064232_add Category and SubCategory FK")]
    partial class addCategoryandSubCategoryFK
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.Models.Category", b =>
                {
                    b.Property<Guid>("CategoryKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CategoryName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CategoryKey");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DAL.Models.Fakultas", b =>
                {
                    b.Property<Guid>("FakultasId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NamaFakultas")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("FakultasId");

                    b.HasIndex("NamaFakultas")
                        .IsUnique()
                        .HasFilter("[NamaFakultas] IS NOT NULL");

                    b.ToTable("Fakultas");
                });

            modelBuilder.Entity("DAL.Models.ProgramStudi", b =>
                {
                    b.Property<Guid>("ProgramStudiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FakultasId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NamaProgramStudi")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ProgramStudiId");

                    b.HasIndex("FakultasId");

                    b.ToTable("ProgramStudi");
                });

            modelBuilder.Entity("DAL.Models.SubCategory", b =>
                {
                    b.Property<Guid>("SubCategoryKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CategoryKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Categorykey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SubCategoryName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SubCategoryKey");

                    b.HasIndex("CategoryKey");

                    b.ToTable("SubCategory");
                });

            modelBuilder.Entity("DAL.Models.ProgramStudi", b =>
                {
                    b.HasOne("DAL.Models.Fakultas", "Fakultas")
                        .WithMany("ProgramStudis")
                        .HasForeignKey("FakultasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fakultas");
                });

            modelBuilder.Entity("DAL.Models.SubCategory", b =>
                {
                    b.HasOne("DAL.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryKey");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("DAL.Models.Fakultas", b =>
                {
                    b.Navigation("ProgramStudis");
                });
#pragma warning restore 612, 618
        }
    }
}
