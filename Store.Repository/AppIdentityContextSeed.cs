using Microsoft.AspNetCore.Identity;
using Store.Data.Entities.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class AppIdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Mostafa Helal",
                    Email = "Mostafa@gmail.com",
                    UserName = "MostafaHelal",
                    Address = new Address
                    {
                        FirstName = "Mostafa",
                        LastName = "Helal",
                        City = "Tala",
                        State = "Menofia",
                        Street = "22",
                        ZipCode = "12345"
                    }
                };

                await userManager.CreateAsync(user, "Password123!");
            }
        }
    }
}
