using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mini_Project.Models
{
    public class SeedData
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<AppDbContext>();

            string[] roles = new string[]
            {
                "Admin",
                "HRM",
                "Trainee",
                "Tech Lead",
                "Office Manager"
            };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    roleStore.CreateAsync(new IdentityRole(role));
                }
            }

            context.SaveChangesAsync();
        }
    }
}
