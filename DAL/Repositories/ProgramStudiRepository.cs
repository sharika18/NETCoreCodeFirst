using DAL.Model;
using Microsoft.EntityFrameworkCore;


namespace DAL.Repositories
{
    public class ProgramStudiRepository : BaseRepository<ProgramStudi>, IProgramStudiRepository
    {
        public ProgramStudiRepository(DbContext dbContext) : base(dbContext) { }
    }
}
