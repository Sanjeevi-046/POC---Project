using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Poc.CommonModel.Models;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;

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
