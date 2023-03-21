using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class TerritoriesRepository : BaseRepository<Territories>, ITerritoriesRepository
    {
        public TerritoriesRepository(DbContext dbContext) : base(dbContext) { }
    }
}
