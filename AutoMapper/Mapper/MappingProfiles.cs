using AutoMapper;
using POC.CommonModel.Models;
using Poc.CommonModel.Models;
using POC.DataLayer.TempModel;
using POC.DataLayer.Models;


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
            CreateMap<Login,CommonUserModel>().ReverseMap();
            CreateMap<CommonLoginModel, CommonUserModel>().ReverseMap();
            CreateMap<CommonAddressModel,AddressDetail>().ReverseMap();
            CreateMap<CommonPaymentModel,Payment>().ReverseMap();
        }
    }
}
