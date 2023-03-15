using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppIdentityContextSeed
{
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new AppUser
            {
                DisplayName = "John",
                Email = "jd@test.com",
                UserName = "jd@test.com",
                Address = new Address
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Street = "5555 Manhattan",
                    City = "New York",
                    State = "NY",
                    ZipCode = "90909"
                }
            };

            await userManager.CreateAsync(user, "Pa$$w0rd");
        }
    }
}