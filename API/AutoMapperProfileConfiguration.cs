using AutoMapper;
using API.DTO;
using Model = DAL.Models;
namespace API
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            //CATEGORY
            CreateMap<CategoryDTO, Model.Category>();
            CreateMap<Model.Category, CategoryDTO>();

            //CUSTOMER
            CreateMap<CustomerDTO, Model.Customer>();
            CreateMap<Model.Customer, CustomerDTO>();

            //PRODUCT
            CreateMap<ProductDTO, Model.Product>();
            CreateMap<Model.Product, ProductDTO>();

            CreateMap<ProductWithSubCategoryDTO, Model.Product>()
                .ForMember(s => s.SubCategoryId, d => d.MapFrom(t => t.SubCategory.SubCategoryId))
                .ForMember(s => s.SubCategory, opt => opt.Ignore()); ;
            CreateMap<Model.Product, ProductWithSubCategoryDTO>();


            //SALES
            CreateMap<SalesDTO, Model.Sales>();
            CreateMap<Model.Sales, SalesDTO>();

            CreateMap<SalesCreateDTO, Model.Sales>();
            CreateMap<Model.Sales, SalesCreateDTO>();

            CreateMap<SalesWithDependencyDTO, Model.Sales>();
            CreateMap<Model.Sales, SalesWithDependencyDTO>();

            //SUBCATEGORY
            CreateMap<SubCategoryDTO, Model.SubCategory>();
            CreateMap<Model.SubCategory, SubCategoryDTO>();

            

            CreateMap<SubCategoryWithCategoryDTO, Model.SubCategory>()
                .ForMember(s => s.CategoryId, d => d.MapFrom(t => t.CategoryDTO.CategoryId))
                .ForMember(s => s.Category, opt => opt.Ignore()); ; ;
            CreateMap<Model.SubCategory, SubCategoryWithCategoryDTO>();

            //TERRITORIES
            CreateMap<TerritoriesDTO, Model.Territories>();
            CreateMap<Model.Territories, TerritoriesDTO>();

            CreateMap<TerritoriesCreateDTO, Model.Territories>();
            CreateMap<Model.Territories, TerritoriesCreateDTO>();
        }
    }
}
