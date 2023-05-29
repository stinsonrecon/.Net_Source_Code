using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.Models;
using BCXN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace BCXN.Controllers
{
    //[Authorize]
    [Route("api/role")]
    public class ApplicationRoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        public ApplicationRoleController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;
        }
        [HttpGet("getAll")]
        public IActionResult GetRole()
        {
            List<ApplicationRoleViewModel> model = new List<ApplicationRoleViewModel>();
            model = roleManager.Roles.Select(r => new ApplicationRoleViewModel
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                ChucNangDefault = r.ChucNangDefault
            }).ToList();
            return Ok(model);
        }

        public IActionResult CreateOrEdit(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = roleManager.Roles.FirstOrDefault(r => r.Id == id);
                if (applicationRole != null)
                {
                    model.Id = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return Ok(id);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> CreateOrEditRole(string id, [FromBody] ApplicationRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationRole applicationRole = (!String.IsNullOrEmpty(id) && id != "0") ? roleManager.Roles.FirstOrDefault(r => r.Id == id) :
                  new ApplicationRole
                  {
                      CreatedDate = DateTime.UtcNow
                  };
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.ChucNangDefault = model.ChucNangDefault;
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                IdentityResult roleRuslt = !String.IsNullOrEmpty(id) && id != "0" ? await roleManager.UpdateAsync(applicationRole)
                                                    : await roleManager.CreateAsync(applicationRole);


                _context.ChucNangNhom.RemoveRange(_context.ChucNangNhom.Where(x => x.NhomQuyenId == id));
                _context.SaveChanges();


                if (model.DsChucNang.Count() > 0)
                {
                    foreach (var item in model.DsChucNang)
                    {
                        _context.ChucNangNhom.Add(
                            new ChucNangNhom
                            {
                                ChucNangId = item,
                                NhomQuyenId = !String.IsNullOrEmpty(id) && id != "0" ? id : applicationRole.Id
                            }
                        ) ;
                        _context.SaveChanges();
                    }
                }

                if (applicationRole != null)
                {
                    var listUserInRole = await userManager.GetUsersInRoleAsync(applicationRole.Name.ToString());
                    if(listUserInRole.Count() > 0)
                    {
                        foreach (var item in listUserInRole)
                        {
                            IdentityResult roleResultRemove = await userManager.RemoveFromRoleAsync(item, applicationRole.Name.ToString());
                        }
                    }
                    

                    foreach (var item in model.DsDonVi)
                    {
                        ApplicationUser user = await userManager.FindByIdAsync(item.Id);
                        if (user != null)
                        {
                            user.FirstName = item.FirstName;
                            user.LastName = item.LastName;
                            user.Email = item.Email;
                            user.UserName = item.UserName;
                            user.DonViId = item.DonViId;
                            user.PhoneNumber = item.PhoneNumber;
                            // user.NhomQuyenId = model.NhomQuyenId;
                            user.ChangePassWordDate = DateTime.Now;
                        }
                        
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name.ToString());
 
                    }

                }

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    model.Id = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return PartialView("_DeleteRole", model);
        }

        public async Task<IActionResult> DeleteApplicationRole(ApplicationRoleViewModel model)
        {
            if (model != null)
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.Id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return PartialView("_DeleteRole", model);
        }
    }
}