using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            var ROLE_ADMIN_ID = Guid.NewGuid().ToString();
            var USER_ADMIN_ID = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = ROLE_ADMIN_ID,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "HRM",
                    NormalizedName = "HRM"
                },
                new IdentityRole
                {
                    Name = "Tech Lead",
                    NormalizedName = "TECH LEAD"
                },
                new IdentityRole
                {
                    Name = "Office Manager",
                    NormalizedName = "OFFICE MANAGER"
                },
                new IdentityRole
                {
                    Name = "Trainee",
                    NormalizedName = "TRAINEE"
                }
                );

            var hasher = new PasswordHasher<ApplicationUser>();

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = USER_ADMIN_ID,
                    firstName = "internship",
                    lastName = "admin",
                    UserName = "admin@gmail.com",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1377*"),
                }
                );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = ROLE_ADMIN_ID,
                    UserId = USER_ADMIN_ID
                }
                );

        }

    }
}
