﻿using System;
using System.Linq;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FPT_Book_Khôi_Phi.DbInitializer
{
    public class DbInitializer: IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            {
            }

            if (_db.Roles.Any(r => r.Name == SD.Role_Admin)) return;
            if (_db.Roles.Any(r => r.Name == SD.Role_Customer)) return;
            if (_db.Roles.Any(r => r.Name == SD.Role_StoreOwner)) return;
            

            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_StoreOwner)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin",
            }, "Admin123@").GetAwaiter().GetResult();

            var userAdmin = _db.ApplicationUsers.Where(u => u.Email == "admin@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(userAdmin, "Admin").GetAwaiter().GetResult();
        }
    }
}