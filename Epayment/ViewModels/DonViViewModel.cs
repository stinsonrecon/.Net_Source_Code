using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class DonViViewModel
    {
        public int Id { get; set; }
        public string TenDonVi { get; set; }
        public string MaDonVi { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MoTa { get; set; }
        public int? DonViChaId { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public int? LaPhongBan { get; set; }
        public int? TrangThai { get; set; }
        public string ERPMaCongTy { get; set; }
        public string ERPMaChiNhanh { get; set; }
        public string ERPIDDonVi { get; set; }
        public int DOfficeDonVi { get; set; }
    }

    public class DonViGetViewModel
    {
        public int Id { get; set; }
        public string TenDonVi { get; set; }
        public string MaDonVi { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MoTa { get; set; }
        public int? DonViChaId { get; set; }
        public string DonViChaTieuDe { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public int? LaPhongBan { get; set; }
        public int? TrangThai { get; set; }
        public string ERPMaCongTy { get; set; }
        public string ERPMaChiNhanh { get; set; }
        public string ERPIDDonVi { get; set; }
        public int DOfficeDonVi { get; set; }
    }

    public class GetDonViViewModel
    {
        public int DonViId { get; set; }
        public string TenDonVi { get; set; }
        public string MaDonVi { get; set; }
        public string DiaChi { get; set; }

        public string NguoiLienHe { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MoTa { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public int LoaiDonVi { get; set; }
        public int? TrangThai { get; set; }
        public string ERPMaCongTy { get; set; }
        public string ERPMaChiNhanh { get; set; }
        public string ERPIDDonVi { get; set; }
        public int DOfficeDonVi { get; set; }
    }

    public class ParamsGetDonViViewModel : PaginationViewModel
    {
        public int? TrangThai { get; set; }
        public string TenDV { get; set; }
    }

    public class ResponseDonViViewModel : ResponseWithPaginationViewModel
    {
        public List<DonViGetViewModel> Data { get; set; }
        public ResponseDonViViewModel(List<DonViGetViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}
