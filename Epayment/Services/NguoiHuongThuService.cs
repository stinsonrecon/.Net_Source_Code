using System;
using System.Collections.Generic;
using AutoMapper;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class NguoiHuongThuService :INguoiHuongThuService
    {
        private readonly INguoiHuongThuRepository _repo;
        private readonly IMapper _mapper;

        public NguoiHuongThuService(INguoiHuongThuRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
         public List<NguoiHuongThuViewModel> GetListNguoiHuongThu()
        {
            var ret = _repo.GetListNguoiHuongThu();
            return ret;
        }

       
    }
}