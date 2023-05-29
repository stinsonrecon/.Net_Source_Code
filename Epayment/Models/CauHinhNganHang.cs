using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Models
{
    public class CauHinhNganHang
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid CauHinhNganHangId { get; set; }
        public NganHang NganHang { get; set; }
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
        [StringLength(50)]
        public string BankModel { get; set; }
        [StringLength(50)]
        public string Version { get; set; }
        [StringLength(50)]
        public string SoftwareProviderId { get; set; }
        [StringLength(50)]
        public string BankLanguage { get; set; }
        [StringLength(50)]
        public string BankAppointedApprover { get; set; }
        [StringLength(50)]
        public string BankChannel { get; set; }
        [StringLength(50)]
        public string BankMerchantId { get; set; }
        [StringLength(50)]
        public string BankClientIP { get; set; }
        public bool DaXoa { get; set; }
    }
}
