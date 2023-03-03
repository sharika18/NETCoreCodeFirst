using API.ProgramStudi.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Falultas.DTO
{
    public class FakultasDTO
    {
        public Guid FakultasId { get; set; }

        [Required]
        public string NamaFakultas { get; set; }
    }

    public class FakultasWithProgramStudiDTO : FakultasDTO
    {
        public List<ProgramStudiDTO> ProgramStudis { get; set; }
    }
}
