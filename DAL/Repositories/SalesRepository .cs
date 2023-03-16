using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class SalesRepository : BaseRepository<Sales>, ISalesRepository
    {
        public SalesRepository(DbContext dbContext) : base(dbContext) { }
    }
}
