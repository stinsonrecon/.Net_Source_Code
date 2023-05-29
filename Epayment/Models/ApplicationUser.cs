using Epayment.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? DonViId { get; set; }
        public string NhomQuyenId { get; set; }
        public DateTime? ChangePassWordDate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsActive { get; set; }
        public string CAProvider { get; set; }
        public int? accountType { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
        public string DinhDanhKy { get; set; }
    }
}
