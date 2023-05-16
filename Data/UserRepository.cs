using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser? Get(string? id)
        {
            return _context.Users.First(u => u.Id == id);

        }

        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToHashSet<ApplicationUser>();
        }

        public IdentityRole GetRole(string? id)
        {
            return _context.Roles.Find(id);
        }


        public async Task AddUserToRole(ApplicationUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
            _context.SaveChanges();
        }


        public async Task UpdateUserRole(ApplicationUser user, string oldRole, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, oldRole);

            await AddUserToRole(user, roleName);
            _context.SaveChanges();
        }
        
        public IdentityUserRole<string> GetUsersRole(string userId)
        {
            return _context.UserRoles.First(ur => ur.UserId == userId);
        }

        public string GetUserRole(string userId)
        {
            return _context.UserRoles.First(ur => ur.UserId == userId).RoleId;
        }

        public bool IsInAnyRole(string userId)
        {
            return _context.UserRoles.Any(ur => ur.UserId == userId);
        }

        public ApplicationUser GetUserByUserName(string userName)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == userName);
        }
    }
}
