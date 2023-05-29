using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class LinhVucBaoCaoViewModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public int LinhVucChaId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public int DaXoa { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
    }

    public class LinhVucBaoCaoGetViewModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public int LinhVucChaId { get; set; }
        public string LinhVucChaTieuDe { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public int DaXoa { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
    }

    public class ParamGetLinhVucBaoCaoViewModel : PaginationViewModel
    {
        public int? TrangThai { get; set; }
        public string TenLV { get; set; }
    }

    public class ResponseLinhVucBaoCaoViewModel : ResponseWithPaginationViewModel
    {
        public List<LinhVucBaoCaoGetViewModel> Data { get; set; }
        public ResponseLinhVucBaoCaoViewModel(List<LinhVucBaoCaoGetViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}
