using AutoMapper;
using POC.DomainModel.Models;
using POC.CommonModel.Models;
using Poc.CommonModel.Models;
using POC.DataLayer.TempModel;


namespace POC.AutoMapper.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Login, CommonLoginModel>().ReverseMap();
            CreateMap<Product, CommonProductModel>().ReverseMap();
            CreateMap<Order, CommonOrderModel>().ReverseMap();
            CreateMap<CartTable, CommonCartModel>().ReverseMap();
            CreateMap<Refreshtoken, CommonRefereshToken>().ReverseMap();
            CreateMap<PaginationModel, CommonPaginationModel>().ReverseMap();

        }
    }
}
