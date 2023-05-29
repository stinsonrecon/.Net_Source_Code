using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.Models;
using BCXN.Statics;
using BCXN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BCXN.ViewModels;

namespace BCXN.Controllers
{
    //[Authorize]
    [Route("api/user")]
    public class ApplicationUserController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext _context;

        public ApplicationUserController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;
        }
        [HttpGet("getAll")]
        public async Task<List<UserViewModel>> GetAllUser()
        {
            List<UserViewModel> model = new List<UserViewModel>();
            model = userManager.Users.Where(x => (x.IsDelete == false || x.IsDelete == null)).Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DonViId = u.DonViId,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                IsDelete = u.IsDelete,
                IsActive = u.IsActive,
                accountType = u.accountType,
                CAProvider = u.CAProvider,
                DinhDanhKy = u.DinhDanhKy

            }).OrderBy(x => x.UserName).ToList();

            foreach (var m in model)
            {
                var user = userManager.Users.FirstOrDefault(u => u.Id == m.Id);
                IList<string> listRole = await userManager.GetRolesAsync(user);
                if (listRole != null && listRole.Count > 0)
                {
                    m.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                    m.RoleName = roleManager.Roles.Single(r => r.Name == listRole.Single()).Name;
                }

                //    m.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                // m.ApplicationRoles = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Name;
            }

            return model;

        }

        public async Task<IActionResult> CreateOrEditAsync(string id)
        {
            UserViewModel model = new UserViewModel();
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();

            model.UserClaims = ClaimData.UserClaims.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList();

            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    model.EditMode = true;
                    model.IsActive = user.IsActive;
                    model.accountType = user.accountType;
                    model.CAProvider = user.CAProvider;                                        
                    model.DinhDanhKy = user.DinhDanhKy;
                    IList<string> listRole = await userManager.GetRolesAsync(user);
                    if (listRole != null && listRole.Count > 0)
                        model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;

                    //get claim
                    var claims = await userManager.GetClaimsAsync(user);
                    model.UserClaims = ClaimData.UserClaims.Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = claims.Any(x => x.Value == c)
                    }).ToList();
                }
            }
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost("create/{id}")]
        public async Task<HttpResponseMessage> CreateOrEditUser(string id, [FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (id == "-1")
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    DonViId = model.DonViId,
                    PhoneNumber = model.PhoneNumber,
                    ChangePassWordDate = DateTime.Now.AddDays(90),
                    accountType = model.accountType,
                    IsActive = model.IsActive,
                    CAProvider = model.CAProvider,
                    DinhDanhKy = model.DinhDanhKy
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                    if (applicationRole != null)
                    {
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name.ToString());
                        if (roleResult.Succeeded)
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        }
                    }

                    List<SelectListItem> userClaims = model.UserClaims.Where(c => c.Selected).ToList();
                    foreach (var claim in userClaims)
                    {
                        await userManager.AddClaimAsync(user, new Claim(claim.Value, claim.Value));
                    }
                }
            }
            else
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.DonViId = model.DonViId;
                    user.PhoneNumber = model.PhoneNumber;
                    // user.NhomQuyenId = model.NhomQuyenId;
                    user.ChangePassWordDate = DateTime.Now;
                    user.IsActive = model.IsActive;
                    user.accountType = model.accountType;
                    user.CAProvider = model.CAProvider;
                    user.DinhDanhKy = model.DinhDanhKy;
                    string existingRole = "";
                    IList<string> listRole = await userManager.GetRolesAsync(user);

                    string existingRoleId = "";
                    if (listRole != null && listRole.Count > 0)
                    {
                        existingRole = listRole.Single();
                        existingRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id;
                    }


                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (existingRoleId != model.ApplicationRoleId)
                        {
                            if (existingRole != "")
                            {
                                IdentityResult roleResult = await userManager.RemoveFromRoleAsync(user, existingRole);
                                if (roleResult.Succeeded)
                                {
                                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                    if (applicationRole != null)
                                    {
                                        IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                                        if (newRoleResult.Succeeded)
                                        {
                                            return new HttpResponseMessage(HttpStatusCode.OK);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                if (applicationRole != null)
                                {
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                                    if (newRoleResult.Succeeded)
                                    {
                                        return new HttpResponseMessage(HttpStatusCode.OK);
                                    }
                                }
                            }
                        }

                        // var claims = await userManager.GetClaimsAsync(user);
                        // await userManager.RemoveClaimsAsync(user, claims);

                        // List<SelectListItem> userClaims = model.UserClaims.Where(c => c.Selected).ToList();
                        // foreach (var claim in userClaims)
                        // {
                        //     await userManager.AddClaimAsync(user, new Claim(claim.Value, claim.Value));
                        // }
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public async Task<IActionResult> Delete(string id)
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
            return PartialView("_Delete", model);
        }

        [HttpDelete("delete/{id}")]
        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            if (id != null)
            {
                // ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.Id);
                // if (applicationRole != null)
                // {
                //     IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result;
                //     if (roleRuslt.Succeeded)
                //     {
                //         return RedirectToAction("Index");
                //     }
                // }
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.IsDelete = true;
                    user.IsActive = false;
                }
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("checkpassword")]
        public async Task<bool> CheckPassword(string Id, string Password)
        {
            var user = await userManager.FindByIdAsync(Id);
            var result = await userManager.CheckPasswordAsync(user, Password);
            return result;
        }

        [HttpGet("changepassword")]
        public async Task<HttpResponseMessage> ChangePassword(string id, string PasswordNew)
        {
            if (PasswordNew == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            else
            {
                var user = await userManager.FindByIdAsync(id);
                IdentityResult result = await userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    IdentityResult result2 = await userManager.AddPasswordAsync(user, PasswordNew);
                    var userFirst = _context.ApplicationUser.FirstOrDefault(x => x.Id == id);
                    userFirst.ChangePassWordDate = DateTime.Now;
                    _context.SaveChanges();
                    // user.ChangePassWordDate = DateTime.Now;

                    if (result2.Succeeded)
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

            }
        }
    }
}