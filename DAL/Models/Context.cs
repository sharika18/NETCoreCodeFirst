using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Fakultas>().HasIndex(a => a.NamaFakultas).IsUnique(true);
        }

        public DbSet<Fakultas> Fakultas { get; set; }
        public DbSet<ProgramStudi> ProgramStudi { get; set; }
    }
}
