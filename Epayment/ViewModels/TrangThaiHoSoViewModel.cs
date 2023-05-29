using System;
using System.Collections.Generic;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class TrangThaiHoSoViewModel
    {
        public Guid TrangThaiHoSoId { get; set; }
        public string TenTrangThaiHoSo { get; set; }
        public int GiaTri { get; set; }
        public int TrangThai { get; set; }
    }
    public class ResponseTrangThaiHoSoViewModel : ResponseWithPaginationViewModel
    {
        public List<TrangThaiHoSoViewModel> Data { get; set; }
        public ResponseTrangThaiHoSoViewModel(List<TrangThaiHoSoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }


    }
}