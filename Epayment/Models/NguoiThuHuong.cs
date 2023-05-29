using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class NguoiThuHuong
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid NguoiThuHuongId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
        public string SoTKThuHuong { get; set; }
        public string TenNguoiThuHuong { get; set; }
        public decimal SoTienThanhToan { get; set; }
        public int HinhThucTT { get; set; }
        public NganHang NganHangThuHuong { get; set; }
    }
}
