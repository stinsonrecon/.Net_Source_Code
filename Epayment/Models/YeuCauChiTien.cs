using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class YeuCauChiTien
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid YeuCauChiTienId { get; set; }
        public ChungTu ChungTu { get; set; }
        public decimal SoTienTT { get; set; }
        public NganHang NganHangThuHuong { get; set; }
        public string TaiKhoanChi { get; set; }
        public string TaiKhoanThuHuong { get; set; }
        public string NguoiThuHuong { get; set; }
        public NganHang NganHangChi { get; set; }
        public int TrangThaiChi { get; set; }
        public DateTime? NgayYeuCauChi { get; set; }
        public ApplicationUser NguoiYeuCauChi { get; set; }
        public string ChuKy { get; set; }
        public string MaKetQuaChi { get; set; }
        public DateTime? ThoiGianChi { get; set; }
    }
}
