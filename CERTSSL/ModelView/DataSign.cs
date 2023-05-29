using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CERTSSL.ModelView
{
    public class DataSign
    {
        public string hash { get; set; }
        public string signature { get; set; }
        public string bankcode { get; set; }

    }
    public class ResponseResult
    {
        public string Message { get; set; }

        public object Data { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseResult(string message, object data, string errorcode, bool success)
        {
            Message = message;
            Data = data;
            ErrorCode = errorcode;
            Success = success;
        }
    }

}