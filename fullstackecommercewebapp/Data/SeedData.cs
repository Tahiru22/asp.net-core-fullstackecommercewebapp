using fullstackecommercewebapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace fullstackecommercewebapp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext _db, UserManager<User> _userManager, RoleManager<AspNetRoles> _roleManager)
        {
            _db.Database.Migrate();

            // Seed roles
            if (!_db.Roles.Any())
            {
                var administratorRole = new AspNetRoles() { Name = "Administrator" };
                var normalUserRole = new AspNetRoles() { Name = "Normal User" };

                await _roleManager.CreateAsync(administratorRole);
                await _roleManager.CreateAsync(normalUserRole);
            }


            if (!_db.Users.Any())
            {
                var admin = new User()
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    PhoneNumber = "233542345603"
                };

                // Create the user asynchronously
                var result = await _userManager.CreateAsync(admin, "Admin@123");

                // Check if the user was successfully created
                if (result.Succeeded)
                {
                    // Add the user to the Administrator role
                    await _userManager.AddToRoleAsync(admin, "Administrator");
                }
                else
                {
                    // Handle the failure scenario
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }



        }
    }

}
