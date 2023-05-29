using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BCXN.ViewModels;
using BCXN.Models;
namespace Epayment.ViewModels
{
    public class NguoiPheDuyetViewModel 
    {
        public Guid nguoiPheDuyet { get; set; }
        public string tenTaiKhoan { get; set; }
        public string hoVaTen { get; set; }
        public DateTime ngayTao { get; set; }
        

    }
}