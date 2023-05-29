using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BCXN.ViewModels;
using BCXN.Models;
namespace Epayment.ViewModels
{
    public class LichSuHoSoViewModel : DanhSachHoSoViewModel
    {
        public Guid lichSuId { get; set; }
        public DateTime thoiGianCapNhat { get; set; }
        public ApplicationUser nguoiCapNhat { get; set; }
        

    }
    public class ResponseLichSuViewModel : ResponseWithPaginationViewModel
    {
        public List<LichSuHoSoViewModel> Data { get; set; }
        public ResponseLichSuViewModel(List<LichSuHoSoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}