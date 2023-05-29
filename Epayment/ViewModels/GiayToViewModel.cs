using System;
using System.Collections.Generic;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class GiayToViewModel
    {
        public Guid Id {get;set;}
        public string TenGiayTo{get;set;}
        public string MaGiayTo { get; set; }
        public int KySo { get; set; }
        public string Nguon { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTaoId { get; set;}
        public string TenNguoiTao {get;set;}
        public int BatBuoc { get; set; }
        public int ThuTu { get; set; }
    }
    public class ResponseGiayToViewModel : ResponseWithPaginationViewModel
    {
        public List<GiayToViewModel> Data { get; set; }
        public ResponseGiayToViewModel(List<GiayToViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}