using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;

namespace BCXN.ViewModels
{
    public class YeuCauBaoCaoViewModel
    {
        public string TieuDe { get; set; }
        public int Id { get; set; }
        public int? Nam { get; set; }
        public int? ChuKy { get; set; }
        public int? LoaiChuKy { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
        public int? DaXoa { get; set; }
        public string BaoCaoDaXN { get; set; }
        public string FileBaoCaoHieuChinh { get; set; }
    }

    public class YeuCauBaoCaoCreateViewModel
    {

        public string TieuDe { get; set; }
        public int? Nam { get; set; }
        public int? ChuKy { get; set; }
        public int? LoaiChuKy { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public string NguoiTaoId { get; set; }
        public List<BaoCaoViewModel> BaoCaoDinhKy { get; set; }
        public List<BaoCaoViewModel> BaoCaoPhatSinh { get; set; }

    }

    public class ParamsGetYeuCauBaoCaoViewModel : PaginationViewModel {
        public int? LoaiBaoCao { get; set; }
        public int? Nam { get; set; }
        public int? TrangThai { get; set; }
        public int? DonViId { get; set; }
    }
}