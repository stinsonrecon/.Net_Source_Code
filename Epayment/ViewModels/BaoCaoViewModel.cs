using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class BaoCaoViewModel
    {
        public int Id { get; set; }
        public int LoaiBaoCao { get; set; }
        public int TypeId { get; set; }
        public int SoLieuId { get; set; }
        public string TenBaoCao { get; set; }
        public string UrlLink { get; set; }
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public int? DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int? LinhVucId { get; set; }
        public string TenLinhVuc { get; set; }
        public string ChuKy { get; set; }
        public string ViewName { get; set; }
        public string ViewId { get; set; }
        public string SiteName { get; set; }
        public string SiteId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public string NguoiCapNhatId { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThaiDuLieu { get; set; }
        public int? TrangThaiXacNhan { get; set; }
        public DateTime? ThoiGianXacNhan { get; set; }
        public string NguoiTaoXNId { get; set; }
        public DateTime? ThoiGianXNTao { get; set; }
        public int? SapXep { get; set; }
        public int? Order { get; set; }
        public string FileBaoCao { get; set; }
        public DateTime? ThoiGianCapNhatDuLieu { get; set; }
    }

    public class ThongKeBCViewModel
    {
        public int DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int DaXacNhan { get; set; }
        public int ChuaXacNhan { get; set; }
        public int SoLieuDung { get; set; }
        public int SoLieuChuaDung { get; set; }
    }

    public class BaoCaoXNViewModel : XacNhanBaoCaoViewModel
    {
        public int LoaiBaoCao { get; set; }
        public int TypeId { get; set; }
        public int SoLieuId { get; set; }
        public string TenBaoCao { get; set; }
        public string UrlLink { get; set; }
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public int? DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int? LinhVucId { get; set; }
        public string TenLinhVuc { get; set; }
        public string ChuKy { get; set; }
        public string ViewName { get; set; }
        public string ViewId { get; set; }
        public string SiteName { get; set; }
        public string SiteId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public string FileBaoCao { get; set; }
    }

    public class GetBaoCaoByLinhVuc {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public int LinhVucChaId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public List<BaoCaoViewModel> BaoCao { get; set; }
    }

    public class ParamsGetBaoCaoViewModel : PaginationViewModel
    {
        public int? LoaiBaoCao { get; set; }
        public int? TrangThai { get; set; }
        public int? LinhVucId { get; set; }
        public int? DonViId { get; set; }
        public int? TypeId { get; set; }
        public string TenBC { get; set; }
    }
}
