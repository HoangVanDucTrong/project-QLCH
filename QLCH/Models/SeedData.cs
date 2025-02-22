using Microsoft.AspNetCore.Identity;

namespace QLCH.Models
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Employee" };

            try
            {
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (!roleResult.Succeeded)
                        {
                         
                            Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                        else
                        {
                            Console.WriteLine($"Role {roleName} created successfully.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing roles: {ex.Message}");
                // Optionally, log the stack trace or use a logging framework here
            }
        }
    }

}
