using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Epayment.Models;
using Microsoft.AspNetCore.Http;

namespace Epayment.ViewModels
{
    public class ParmChiTietGiayToHSTTViewModel
    {
        public string ChiTietHoSoId { get; set; }
        public string GiayToId { get; set; }
        public string HoSoThanhToanId { get; set; }
        // public string  FileDinhKem {get;set;}
        public int TrangThaiGiayTo { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayCapNhat { get; set; }
        public string NguoiCapNhatId { get; set; }
        public List<IFormFile> listFile { get; set; }
        public IEnumerable<string> urlFile { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
    }
    public class ParmPhieuThamTraHSTTViewModel{
        public IFormFile File {get;set;}
        public string GiayToId { get; set; }
        public string NguoiCapNhatId { get; set; }
        public string HoSoThanhToanId { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
    }
}