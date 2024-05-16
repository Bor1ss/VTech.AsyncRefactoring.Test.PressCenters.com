namespace PressCenters.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using PressCenters.Common;
    using PressCenters.Data.Models;

    public class RolesSeeder : ISeeder
    {
        public async Task Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRole(roleManager, GlobalConstants.AdministratorRoleName);
            await SeedRole(roleManager, GlobalConstants.ProUserRoleName);
        }

        private static async Task SeedRole(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
