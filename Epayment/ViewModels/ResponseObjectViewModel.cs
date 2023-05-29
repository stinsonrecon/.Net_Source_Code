using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class ResponseYeuCauBaoCaoViewModel : ResponseWithPaginationViewModel
    {
        public List<YeuCauBaoCaoViewModel> Data { get; set; }
        public ResponseYeuCauBaoCaoViewModel(List<YeuCauBaoCaoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
    public class ResponseBaoCaoViewModel : ResponseWithPaginationViewModel
    {
        public List<BaoCaoViewModel> Data { get; set; }
        public ResponseBaoCaoViewModel(List<BaoCaoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }

    public class Response
    {
        public string Message { get; set; }

        public object Data { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public Response(string message, object data, string errorcode, bool success)
        {
            Message = message;
            Data = data;
            ErrorCode = errorcode;
            Success = success;
        }
    }

    public class ResponsePostViewModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public ResponsePostViewModel(){

        }
        public ResponsePostViewModel(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }

    public class ResponseWithPaginationViewModel
    {
        // List<YeuCauBaoGiaViewModel> Data { get; set; }
        public int StatusCode { get; set; }
        public int TotalRecord { get; set; }
   
        public ResponseWithPaginationViewModel(int statusCode, int totalRecord)
        {
            StatusCode = statusCode;
            TotalRecord = totalRecord;
        }
    }
    public class ResponsePostHSTTViewModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string idHoSoTT {get;set;}
        public ResponsePostHSTTViewModel(){

        }
        public ResponsePostHSTTViewModel(string message, int statusCode, string _idHoSoTT )
        {
            Message = message;
            StatusCode = statusCode;
            idHoSoTT = _idHoSoTT;
        }
    }
    public class ResponsePaging
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public int TotalRecord { get; set; }

        public ResponsePaging(string message, object data, string errorcode, bool success, int totalRecord)
        {
            Message = message;
            Data = data;
            ErrorCode = errorcode;
            Success = success;
            TotalRecord = totalRecord;
        }
    }

     
}