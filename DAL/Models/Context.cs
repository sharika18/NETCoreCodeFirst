using Microsoft.EntityFrameworkCore;

namespace DAL.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Fakultas>().HasIndex(a => a.NamaFakultas).IsUnique(true);
            builder.Entity<Sales>().HasKey(x => x.SalesId);
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Territories> Territories { get; set; }
        public DbSet<Sales> Sales { get; set; }

        //public DbSet<SalesStatus> SalesStatus { get; set; }

    }
}
