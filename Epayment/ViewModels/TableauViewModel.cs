using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class TableauViewModel
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
        public List<SelectListItem> UserClaims { get; set; }
    }
    public class Site
    {
        public string contentUrl { get; set; }
        public string id { get; set; }

    }
    public class User
    {
        public string id { get; set; }

    }
    public class View
    {
        public string id { get; set; }
    }
    public class OutputGetView
    {
        public View views { get; set; }
    }

    public class InputDataSignIn
    {
        public Credentials credentials { get; set; }
    }
    public class OutputDataSignIn
    {
        public Credentials credentials { get; set; }
    }
    public class Credentials
    {
        public string name { get; set; }
        public string password { get; set; }
        public Site site { get; set; }
        public string token { get; set; }
        public User user { get; set; }

    }
    
    public class SlideInfo
    {
        public string LinhVucBaoCao { get; set; }
        public string TenBaoCao { get; set; }
        public string PhongBan { get; set; }
        public int? OrderBaoCao { get; set; }
    }
   
}

