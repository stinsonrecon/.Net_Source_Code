using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public List<SelectListItem> ApplicationRoles { get; set; }
        public string ApplicationRoleId { get; set; }
        public bool EditMode { get; set; }
        public int? DonViId { get; set; }
        public string NhomQuyenId { get; set; }
        public int? LoaiDonVi { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsActive { get; set; }
        public int? accountType { get; set; }
        public List<SelectListItem> UserClaims { get; set; }
        public string CAProvider { get; set; }
        public string DinhDanhKy { get; set; }
    }
}
