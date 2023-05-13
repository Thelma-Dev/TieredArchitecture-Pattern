using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IUserRepository
    {
        ApplicationUser? Get(string? id);

        ICollection<ApplicationUser> GetAll();

        IdentityRole GetRole(string? id);

        void AddUserToRole(ApplicationUser user, string roleName);

        void UpdateUserRole(ApplicationUser user, string oldRole, string roleName);

        IdentityUserRole<string> GetUsersRole(string userId);

        bool IsInAnyRole(string userId);
    }
}
