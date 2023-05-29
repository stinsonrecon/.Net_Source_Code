using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class KetQuaKySoModel
    {
        public string state { get; set; }
        public string message { get; set; }
        public string data { get; set; }
       
    }
}
