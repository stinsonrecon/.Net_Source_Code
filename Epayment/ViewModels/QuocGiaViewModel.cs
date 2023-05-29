using BCXN.ViewModels;
using System;
using System.Collections.Generic;

namespace Epayment.ViewModels
{
    public class QuocGiaViewModel
    {
        public Guid QuocGiaId { get; set; }
        public string MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }

    public class ResponseGetQuocGia
    {
        public Dictionary<string, object> Data { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public ResponseGetQuocGia(string message, string errorCode, bool success, int totalRecord, object items)
        {
            Data = new Dictionary<string, object>()
            {
                { "TotalRecord", totalRecord },
                { "Items", items },
            };
            Message = message;
            ErrorCode = errorCode;
            Success = success;
        }
    }

    public class QuocGiaParam : PaginationViewModel
    {
        public string MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }

    public class QuocGiaPagination : PaginationViewModel
    {
        public Guid? QuocGiaId { get; set; }
        public string MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
    }
}