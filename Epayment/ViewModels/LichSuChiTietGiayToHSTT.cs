using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BCXN.ViewModels;
using BCXN.Models;
namespace Epayment.ViewModels
{
    public class LichSuChiTietGiayToHSTT :ChiTietGiayToHSTTViewModel
    {
        public Guid LichSuChiTietId {get;set;}
    }
    public class ResponseLichSuChiTietGiayToHSTTViewModel : ResponseWithPaginationViewModel
    {
        public List<LichSuChiTietGiayToHSTT> Data { get; set; }
        public ResponseLichSuChiTietGiayToHSTTViewModel(List<LichSuChiTietGiayToHSTT> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}