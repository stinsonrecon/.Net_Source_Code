using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;

namespace Epayment.ViewModels
{
    public class TinhThanhPhoViewModel
    {
        public Guid TinhTpId { get; set; }
        public QuocGia QuocGia { get; set; }
        public string MaTinhTp { get; set; }
        public string TenTinhTp { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }

    public class TinhThanhPhoGetModel
    {
        public Guid TinhTpId { get; set; }
        public string TenQuocGia { get; set; }
        public string MaQuocGia { get; set; }
        public string MaTinhTp { get; set; }
        public string TenTinhTp { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }

    public class ParamGetTinhThanhPhoViewModel : PaginationViewModel
    {
        public int? TrangThai { get; set; }
        public string TenTinhTP { get; set; }
    }

    public class ResponseTinhThanhPhoViewModel : ResponseWithPaginationViewModel
    {
        public List<TinhThanhPhoGetModel> Data { get; set; }

        public ResponseTinhThanhPhoViewModel(List<TinhThanhPhoGetModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}