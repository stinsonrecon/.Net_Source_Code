using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class TinhThanhPho
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid TinhTpId { get; set; }
        public QuocGia QuocGia { get; set; }
        public string MaTinhTp { get; set; }
        public string TenTinhTp { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }
}