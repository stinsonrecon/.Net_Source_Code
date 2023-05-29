using System;
using System.Collections.Generic;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class LuongPheDuyetViewModel
    {
        public Guid BuocThucHienId { get; set; }
        public Guid VaiTroId { get; set; }
        public string TenVaiTro { get; set; }
        public Guid LoaiHoSoId { get; set; }
        public string TenLoaiHoSo { get; set; }
        public Guid TrangThaiHoSoId { get; set; }
        public string TenTrangThaiHoSo { get; set; }
        public string TenBuoc { get; set; }
        public int ChuyenSangERP { get; set; }
        public int CoThamTra { get; set; }
        public int ThuTu { get; set; }
    }
    public class ResponseLuongPheDuyetViewModel : ResponseWithPaginationViewModel
    {
        List<LuongPheDuyetViewModel> Data { get; set; }
        public ResponseLuongPheDuyetViewModel(List<LuongPheDuyetViewModel> Data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {

        }
    }
}