using System;
using System.Collections.Generic;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class LoaiHoSoViewModel
    {
        public Guid LoaiHoSoId{get;set;}
        public string TenLoaiHoSo{get;set;} 
        public DateTime NgayTao {get;set;}
        public string MaLoaiHoSo {get;set;}
        public string MoTa { get; set; }
        public int TrangThai { get; set; } 
        public List<GiayToLoaiHoSoResponse> GiayToLoaiHoSoResponse { get; set; }
        public List<LuongPheDuyetResponse> LuongPheDuyetResponse { get; set; }
        public Guid QuyTrinhPheDuyetId { get; set; }

    }
    public class GiayToLoaiHoSoResponse
    {
        public string GiayToLoaiHoSoId { get; set; }
        public string GiayToId { get; set; }
        public string TenGiayTo { get; set; }
        public string MoTa { get; set; }
        public int BatBuoc { get; set; }
        public string Nguon { get; set; }
        public int PheDuyetKySo { get; set; }
        public int ThuTu { get; set; }
    }
    public class LuongPheDuyetResponse
    {
        public string BuocThucHienId { get; set; }
        public string VaiTroId { get; set; }
        // public string LoaiHoSoId { get; set; }
        public string TrangThaiHoSoId { get; set; }
        public string TenTrangThaiHoSo { get; set; }
        public string TenBuoc { get; set; }
        public int ChuyenSangERP { get; set; }
        public int CoThamTra { get; set; }
        public int ThuTu { get; set; }
    }
    public class ResponseLoaiHoSoViewModel : ResponseWithPaginationViewModel
    {
        public List<LoaiHoSoViewModel> Data { get; set; }
        public ResponseLoaiHoSoViewModel(List<LoaiHoSoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}