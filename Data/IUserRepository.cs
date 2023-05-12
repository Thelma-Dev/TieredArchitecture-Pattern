using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IUserRepository
    {
        ApplicationUser? Get(string? id);

        ICollection<ApplicationUser> GetAll();

        IdentityRole GetRole(string? id);

        void SaveChangesAsync();
    }
}
