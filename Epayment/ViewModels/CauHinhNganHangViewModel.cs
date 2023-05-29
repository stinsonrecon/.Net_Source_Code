using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;

namespace Epayment.ViewModels
{
    public class CauHinhNganHangViewModel
    {
        public Guid CauHinhNganHangId { get; set; }
        public Guid NganHangId { get; set; }
        public string  MaNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string LinkTransferUrl { get; set; }
        public string LinkQueryBaseUrl { get; set; }
        public string FTPPath { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string EVNDSAPath { get; set; }
        public string PassPhraseDSA { get; set; }
        public string EVNRSAPath { get; set; }
        public string PassPhraseRSA { get; set; }
        public string BankDSAPath { get; set; }
        public string BankRSAPath { get; set; }
        public string BankApprover { get; set; }
        public string BankSenderAddr { get; set; }
        public string BankModel { get; set; }
        public string Version { get; set; }
        public string SoftwareProviderId { get; set; }
        public string BankLanguage { get; set; }
        public string BankAppointedApprover { get; set; }
        public string BankChannel { get; set; }
        public string BankMerchantId { get; set; }
        public string BankClientIP { get; set; }
        public bool DaXoa { get; set; }
    }

    public class ResponseGetCauHinhNganHang
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }

        public ResponseGetCauHinhNganHang(string message, string errorcode, bool success, int totalRecord, object items)
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

    public class CauHinhNganHangParams: PaginationViewModel
    {
        public Guid NganHangId { get; set; }
        public string LinkTransferUrl { get; set; }
        public string LinkQueryBaseUrl { get; set; }
        public string FTPPath { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string EVNDSAPath { get; set; }
        public string PassPhraseDSA { get; set; }
        public string EVNRSAPath { get; set; }
        public string PassPhraseRSA { get; set; }
        public string BankDSAPath { get; set; }
        public string BankRSAPath { get; set; }
        public string BankApprover { get; set; }
        public string BankSenderAddr { get; set; }
        public string BankModel { get; set; }
        public string Version { get; set; }
        public string SoftwareProviderId { get; set; }
        public string BankLanguage { get; set; }
        public string BankAppointedApprover { get; set; }
        public string BankChannel { get; set; }
        public string BankMerchantId { get; set; }
        public string BankClientIP { get; set; }
        public bool DaXoa { get; set; }
    }

    public class CauHinhNganHangPagination : PaginationViewModel
    {
        public Guid? CauHinhNganHangId { get; set; }
        public Guid? NganHangId { get; set; }
        public string  MaNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string LinkTransferUrl { get; set; }
        public string LinkQueryBaseUrl { get; set; }
        public string FTPPath { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string EVNDSAPath { get; set; }
        public string PassPharseDSA { get; set; }
        public string EVNRSAPath { get; set; }
        public string PassPharseRSA { get; set; }
        public string BankDSAPath { get; set; }
        public string BankRSAPath { get; set; }
        public string BankApprover { get; set; }
        public string BankSenderAddr { get; set; }
        public string BankModel { get; set; }
        public string Version { get; set; }
        public string SoftwareProviderId { get; set; }
        public string BankLanguage { get; set; }
        public string BankAppointedApprover { get; set; }
        public string BankChannel { get; set; }
        public string BankMerchantId { get; set; }
        public string BankClientIP { get; set; }
        public bool? DaXoa { get; set; }
    }
}