using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epayment.ViewModels
{
    //Bước phê duyệt
    public class CreateBuocPheDuyetViewModelBase
    {
        public Guid QuyTrinhPheDuyetId { get; set; }

        public Guid? BuocPheDuyetTruocId { get; set; }
        public Guid? BuocPheDuyetSauId { get; set; }

        [Required]
        public string TrangThaiHoSo { get; set; }

        [Required]
        public string TrangThaiChungTu { get; set; }

        [Required]
        public string TenBuoc { get; set; }

        public int ThuTu { get; set; }

        [Required]
        public List<Guid> DsNguoiThucHien { get; set; }

        public int ThoiGianXuLy { get; set; }
        public string DinhDangKy { get; set; }
        public string ViTriKy { get; set; }
    }

    public class CreateBuocPheDuyetViewModel : CreateBuocPheDuyetViewModelBase
    {
        /// <summary>
        /// Danh sách hành động trong bước phê duyệt
        /// </summary>
        [Required]
        public List<CreateThaoTacBuocPheDuyetViewModel> DsThaoTacBuocPheDuyet { get; set; }
    }

    public class UpdateBuocPheDuyetViewModel : CreateBuocPheDuyetViewModelBase
    {
        public Guid BuocPheDuyetId { get; set; }

        /// <summary>
        /// Danh sách hành động trong bước phê duyệt
        /// </summary>
        [Required]
        public List<UpdateThaoTacBuocPheDuyetViewModel> DsThaoTacBuocPheDuyet { get; set; }
    }

    /// <summary>
    /// Chi tiết bước phê duyệt bao gồm danh sách thao tác
    /// </summary>
    public class BuocPheDuyetChiTietViewModel : BuocPheDuyetViewModel
    {
        public List<ThaoTacBuocPheDuyetViewModel> DsThaoTacBuocPheDuyet { get; set; }
    }

    /// <summary>
    /// Chi tiết bước phê duyệt không có danh sách thao tác
    /// </summary>
    public class BuocPheDuyetViewModel
    {
        public Guid BuocPheDuyetId { get; set; }
        public Guid QuyTrinhPheDuyetId { get; set; }
        public Guid? BuocPheDuyetTruocId { get; set; }
        public IEnumerable<string> TenBuocPheDuyetTruoc { get; set; }
        public Guid? BuocPheDuyetSauId { get; set; }
        public IEnumerable<string> TenBuocPheDuyetSau { get; set; }
        public string TrangThaiHoSo { get; set; }
        public string TrangThaiChungTu { get; set; }
        public string TenBuoc { get; set; }
        public int ThuTu { get; set; }
        public List<Guid> DsNguoiThucHien { get; set; }
        public List<string> TenNguoiThucHien { get; set; }
        public List<string> TaiKhoan { get; set; }
        public int ThoiGianXuLy { get; set; }
        public string DinhDangKy { get; set; }
        public string ViTriKy { get; set; }
    }

    //thao tác bước phê duyệt
    public class CreateThaoTacBuocPheDuyetViewModel
    {
        public Guid? BuocPheDuyetId { get; set; }
        [Required]
        public string HanhDong { get; set; }
        public bool KySo { get; set; }
        public int LoaiKy { get; set; }
        public Guid? GiayToId { get; set; }
        public string TrangThaiHoSo { get; set; }
        public string TrangThaiChungTu { get; set; }
        public bool IsSendMail { get; set; }
        public Guid? DiDenBuocPheDuyetId { get; set; }
    }

    public class UpdateThaoTacBuocPheDuyetViewModel : CreateThaoTacBuocPheDuyetViewModel
    {
        /// <summary>
        /// Id thao tác bước phê duyệt, nếu là null thì create khác null là update
        /// </summary>
        public Guid? ThaoTacBuocPheDuyetId { get; set; }
    }

    public class ThaoTacBuocPheDuyetViewModel
    {
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
        public string HanhDong { get; set; }
        public bool KySo { get; set; }
        public int LoaiKy { get; set; }
        public Guid? GiayToId { get; set; }
        public string TrangThaiHoSo { get; set; }
        public string TrangThaiChungTu { get; set; }
        public bool IsSendMail { get; set; }
        public Guid? DiDenBuocPheDuyetId { get; set; }
    }
}
