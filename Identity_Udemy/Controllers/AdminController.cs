﻿using Identity_Udemy.Models;
using Identity_Udemy.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Identity_Udemy.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager = null) : base(userManager, null, roleManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Claims()
        {
            return View(User.Claims.ToList());
        }
        public IActionResult RoleCreate()
        {
            return View();
        } 

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            AppRole role=new AppRole();
            role.Name = roleViewModel.Name;
            IdentityResult result=roleManager.CreateAsync(role).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(result);
            }
            return View(roleViewModel);
        }

        public IActionResult Roles()
        {
            return View(roleManager.Roles.ToList());
        }
        public IActionResult Users()
        {
            return View(userManager.Users.ToList());
        }
        public IActionResult RoleDelete(string id)
        {
            AppRole role = roleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                IdentityResult result = roleManager.DeleteAsync(role).Result;
            }
            return RedirectToAction("Roles");
        }
        public IActionResult RoleUpdate(string id)
        {
            AppRole role = roleManager.FindByIdAsync(id).Result;


            if (role!=null)
            { 
                return View(role.Adapt<RoleViewModel>());
                
            }
           return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel)
        {
            AppRole role = roleManager.FindByIdAsync(roleViewModel.Id).Result;
            if (role!=null)
            {
                role.Name = roleViewModel.Name;
                IdentityResult result =roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("","Güncelleme işlemi başarısız oldu.");
            }
            return View(roleViewModel);
        }
    
    
        public IActionResult RoleAssign(string id)
        {
            TempData["userId"] = id;
            AppUser user =userManager.FindByIdAsync(id).Result;
            ViewBag.userName = user.UserName;

            IQueryable<AppRole> roles = roleManager.Roles;

            List<string> userRoles = userManager.GetRolesAsync(user).Result as List<string>;

            List<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>();
            foreach (var role in roles)
            {
                RoleAssignViewModel r = new RoleAssignViewModel();
                r.RoleId = role.Id;
                r.RoleName = role.Name;
                if (userRoles.Contains(role.Name))
                {
                    r.Exist = true;
                }
                else
                {
                    r.Exist = false;
                }
                roleAssignViewModels.Add(r);
            }
            return View(roleAssignViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels)
        {
            AppUser user = userManager.FindByIdAsync(TempData["userId"].ToString()).Result;
            foreach (var item in roleAssignViewModels)
            {
                if (item.Exist)
                {
                   await userManager.AddToRoleAsync(user, item.RoleName);

                }
                else
                {
                   await userManager.RemoveFromRoleAsync(user,item.RoleName);
                }
            }
            return RedirectToAction("Users");
        }
    }
}
