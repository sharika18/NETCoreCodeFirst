﻿// <auto-generated />
using System;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20230303083144_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.Model.Fakultas", b =>
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

            modelBuilder.Entity("DAL.Model.ProgramStudi", b =>
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

            modelBuilder.Entity("DAL.Model.ProgramStudi", b =>
                {
                    b.HasOne("DAL.Model.Fakultas", "Fakultas")
                        .WithMany("ProgramStudis")
                        .HasForeignKey("FakultasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fakultas");
                });

            modelBuilder.Entity("DAL.Model.Fakultas", b =>
                {
                    b.Navigation("ProgramStudis");
                });
#pragma warning restore 612, 618
        }
    }
}
