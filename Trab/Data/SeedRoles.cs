using Microsoft.AspNetCore.Identity;

namespace Trab.Data
{
    public class SeedRoles
    {
        public static void Seed(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.Roles.Any() == false)
            {
                roleManager.CreateAsync(new IdentityRole("Professor")).Wait();
                roleManager.CreateAsync(new IdentityRole("Aluno")).Wait();
            }
        }
    }
}
