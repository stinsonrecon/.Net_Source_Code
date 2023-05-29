using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class MailViewModel
    {
      
        public string[] toMail { get; set; }
        public string subject { get; set; }       
        public string noiDung { get; set; }
        public string PathAttachFile { get; set; }
    }

}
