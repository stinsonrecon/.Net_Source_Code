using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;

namespace BCXN.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
        public string ChucNangDefault { get; set; }
        public List<int> DsChucNang { get; set; }
        public List<ApplicationUser> DsDonVi { get; set; }
    }
}
