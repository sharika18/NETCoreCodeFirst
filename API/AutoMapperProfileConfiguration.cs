using AutoMapper;
using API.Falultas.DTO;
using API.ProgramStudi.DTO;
using Model = DAL.Model;
namespace API
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<FakultasDTO, Model.Fakultas>();
            CreateMap<Model.Fakultas, FakultasDTO>();

            CreateMap<FakultasWithProgramStudiDTO, Model.Fakultas>();
            CreateMap<Model.Fakultas, FakultasWithProgramStudiDTO>();


            CreateMap<ProgramStudiWithFakultasDTO, Model.ProgramStudi>()
                .ForMember(s => s.FakultasId, d => d.MapFrom(t => t.Fakultas.FakultasId))
                .ForMember(s => s.Fakultas, opt => opt.Ignore());
            CreateMap<Model.ProgramStudi, ProgramStudiWithFakultasDTO>();


            CreateMap<ProgramStudiDTO, Model.ProgramStudi>();
            CreateMap<Model.ProgramStudi, ProgramStudiDTO>();
        }
    }
}
