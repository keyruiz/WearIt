using Microsoft.AspNetCore.Identity;

namespace MvcWearIt
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CrearRolesAsync(roleManager);

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await CrearAdminAsync(userManager);
        }

        private static async Task CrearRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string nombreRol = "Administrador";
            var yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));

            nombreRol = "Usuario";
            yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
        }

        private static async Task CrearAdminAsync(UserManager<IdentityUser> userManager)
        {
            var testAdmin = userManager.Users
                .Where(x => x.UserName == "admin@wearit.com")
                .SingleOrDefault();

            if (testAdmin != null) return;

            testAdmin = new IdentityUser
            {
                UserName = "admin@wearit.com",
                Email = "admin@wearit.com"
            };

            string admPasswd = "skibidi";

            IdentityResult userResult;
            userResult = await userManager.CreateAsync(testAdmin, admPasswd);

            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testAdmin, "Administrador");
                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to create admin user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
