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
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
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
        }

    }
}
