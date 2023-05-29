using BCXN.ViewModels;
using Epayment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epayment.ViewModels
{
    public class CreateQuyTrinhPheDuyetViewModelBase
    {
        public Guid LoaiHoSoId { get; set; }

        private string _tenQuyTrinh;
        public string TenQuyTrinh 
        { 
            get => _tenQuyTrinh; 
            set => _tenQuyTrinh = value?.Trim(); 
        }
        public DateTime NgayHieuLuc { get; set; }

        private string _moTa;
        public string MoTa 
        { 
            get => _moTa; 
            set => _moTa = value?.Trim(); 
        }
    }

    public class CreateQuyTrinhPheDuyetViewModel : CreateQuyTrinhPheDuyetViewModelBase
    {
    }

    public class UpdateQuyTrinhPheDuyetViewModel : CreateQuyTrinhPheDuyetViewModelBase
    {
        public Guid QuyTrinhPheDuyetId { get; set; }
        public bool TrangThai { get; set; }
    }

    public class QuyTrinhPheDuyetViewModel
    {
        public Guid QuyTrinhPheDuyetId { get; set; }
        public Guid LoaiHoSoId { get; set; }
        public string TenLoaiHoSo { get; set; }
        public string TenQuyTrinh { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
    }

    public class QuyTrinhPheDuyetSearchViewModel : PaginationViewModel
    {
        public string TuKhoa { get; set; }
    }

    public class ResponseQuyTrinhPheDuyetViewModel : ResponseWithPaginationViewModel
    {
        public List<QuyTrinhPheDuyetViewModel> Data { get; set; }
        public ResponseQuyTrinhPheDuyetViewModel(List<QuyTrinhPheDuyetViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
    public class QuyTrinhBuocHanhDongViewModel : QuyTrinhPheDuyetViewModel
    {
        public List<BuocViewModel> ListBuocThuHien { get; set; }
    }
    public class BuocViewModel
    {
        public Guid BuocPheDuyetId { get; set; }
        public Guid QuyTrinhPheDuyetId { get; set; }
        public Guid? BuocPheDuyetTruocId { get; set; }
        public Guid? BuocPheDuyetSauId { get; set; }
        public string TrangThaiHoSo { get; set; }
        public string TrangThaiChungTu { get; set; }
        public string TenBuoc { get; set; }
        public int ThuTu { get; set; }
        public List<Guid> DsNguoiThucHien { get; set; }
        public int ThoiGianXuLy { get; set; }
        public List<ThaoTacViewModel> ListThaoTac { get; set; }
    }
    public class ThaoTacViewModel
    {
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid? BuocPheDuyetId { get; set; }
        public string HanhDong { get; set; }
        public bool KySo { get; set; }
        public int LoaiKy { get; set; }
        public Guid? GiayToId { get; set; }
        public bool IsSendMail { get; set; }
    }
}
