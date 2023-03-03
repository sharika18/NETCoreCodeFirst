using DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class FakultasRepository : BaseRepository<Fakultas>, IFakultasRepository
    {
        public FakultasRepository(DbContext dbContext) : base(dbContext) { }
    }
}
