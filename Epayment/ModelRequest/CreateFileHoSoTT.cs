using Microsoft.AspNetCore.Http;
using System;

namespace Epayment.ModelRequest
{
    public class CreateFileHoSoTT
    {
        public string HoSoId { get; set; }
        public IFormFile FileTaiLieu{ get; set; }
        public string UrlFile { get; set; }
        public int CapKy { get; set; }
        public int TrangThai { get; set;}
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
    }
}