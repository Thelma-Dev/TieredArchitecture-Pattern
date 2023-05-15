using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Runtime.CompilerServices;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IUserRepository
    {
        ApplicationUser? Get(string? id);

        ICollection<ApplicationUser> GetAll();

        IdentityRole GetRole(string? id);

        Task AddUserToRole(ApplicationUser user, string roleName);

        Task UpdateUserRole(ApplicationUser user, string oldRole, string roleName);

        IdentityUserRole<string> GetUsersRole(string userId);

        string GetUserRole(string userId);


		bool IsInAnyRole(string userId);
    }
}
