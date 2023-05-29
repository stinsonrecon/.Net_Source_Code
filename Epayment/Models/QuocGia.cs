using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class QuocGia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid QuocGiaId { get; set; }
        public string MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }
}