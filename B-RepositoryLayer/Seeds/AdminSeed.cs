using A_DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace B_RepositoryLayer.Seeds;

public static class AdminSeed
{
    public static async Task SeedAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (await roleManager.FindByNameAsync("Admin") is null)
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        
        await roleManager.CreateAsync(new IdentityRole("User"));

        if (await userManager.FindByEmailAsync("admin@gmail.com") is null)
        {
            var user = new User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                PhoneNumber = "+201143432637",
            };
            
            var result = await userManager.CreateAsync(user, "admin");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "Admin");
            
        }
    }
}