using API.Falultas.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ProgramStudi.DTO
{
    public class ProgramStudiDTO
    {
        public Guid? ProgramStudiId { get; set; }
        public string NamaProgramStudi { get; set; }
        
    }

    public class ProgramStudiWithFakultasDTO : ProgramStudiDTO
    {
        public FakultasDTO Fakultas { get; set; }
    }
}
