using AutoMapper;
using Epayment.Models;
using BCXN.Models;
using BCXN.Repositories;
using BCXN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epayment.ViewModels;

namespace BCXN.MapperProfile
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<NganHangViewModel, NganHang>();
            CreateMap<ChiNhanhViewModel, ChiNhanhNganHang>();
            CreateMap<TaiKhoanNganHangViewModel, TaiKhoanNganHang>();
        }
    }
}
