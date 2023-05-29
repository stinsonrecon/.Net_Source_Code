using System;
using System.Collections.Generic;
using System.Linq;

namespace Epayment.ViewModels
{
    public class ChiTietGiayToHSTTViewModel
    {
        public Guid? ChiTietHoSoId { get; set; }
        public Guid IdGiayTo{get;set;}
        public string TenGiayTo { get; set; }
        public Guid IdHoSoTT{get;set;}
        public string TenHoSoTT { get; set; }
        public string  FileDinhKem {get;set;}
        public int TrangThaiGiayTo { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public string Nguon { get; set; }
        public string MaGiayTo { get ;set; }
        public int ThuTu { get; set; }
        public Guid LoaiHoSoId { get ;set; }
        public int BatBuoc { get; set; }
        public string[] file =>setFile();
        public string[] setFile(){
            if(!String.IsNullOrEmpty(FileDinhKem)){
                return FileDinhKem.Split(",").Select(x => (!String.IsNullOrEmpty(x) ? x :null)).Where(m => m != null).ToArray();
            }
            return new string [0];
        }
    }
}