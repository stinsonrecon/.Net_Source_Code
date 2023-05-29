using BCXN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.ViewModels
{
    public class ChiTietHachToanParams : PaginationViewModel
    {
        public Guid ChiTietHachToanId { get; set; }
        public string TKNo { get; set; }
        public string TKCo { get; set; }
        public decimal SoTien { get; set; }
        public string DienGiai { get; set; }
        public Guid ChungTuId { get; set; }
    }

    public class ResponseGetChiTietHachToan
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetChiTietHachToan(string message, string errorcode, bool success, int totalRecord, object items)
        {
            Data = new Dictionary<string, object>()
            {
                { "TotalRecord", totalRecord},
                { "Items", items }
            };
            Message = message;
            ErrorCode = errorcode;
            Success = success;
        }
    }
}
