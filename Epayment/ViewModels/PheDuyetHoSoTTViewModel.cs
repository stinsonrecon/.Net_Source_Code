using System;
using System.Collections.Generic;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class PheDuyetHoSoTTViewModel
    {
        public string PheDuyetHoSoTTId { get;set;}
        public string HoSoThanhToanId { get;set;}
        public string NguoiThucHienId { get;set;}
        public string TenNguoiThucHien { get; set;}
        public DateTime NgayThucHien{get;set;}
        public int TrangThaiHoSo {get;set;}
        public string TrangThaiPheDuyet {get;set;}
        public string BuocThucHien {get;set;}
        public string NoiDung {get;set;}
    }
    public class ResponsePheDuyetHoSoTTViewModel : ResponseWithPaginationViewModel
    {
        public List<PheDuyetHoSoTTViewModel> Data { get; set; }
        public ResponsePheDuyetHoSoTTViewModel(List<PheDuyetHoSoTTViewModel> data, int statusCode, int totalRecord ) : base(statusCode, totalRecord )
        {
            Data = data;
        }
    }
}