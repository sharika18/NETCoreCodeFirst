using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = DAL.Model;

namespace API.GraphQL
{
    public class Query
    {
        public string Instruction => "This is yaya";

        private readonly Model.Context dbContext;
        public Query(Model.Context dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Model.Fakultas> Fakultas => dbContext.Fakultas;
        public IQueryable<Model.ProgramStudi> ProgramStudis => dbContext.ProgramStudi;
    }
}
