using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            return _context.Users.First( u => u.Id == id);

        }
       
        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToHashSet<ApplicationUser>();
        }

        public IdentityRole GetRole(string? id)
        {
            return _context.Roles.Find(id);
        }

        public void AddUserToRole(ApplicationUser user, string roleName)
        {
            _userManager.AddToRoleAsync(user, roleName);
            _context.SaveChanges();
            
        }

        public void UpdateUserRole(ApplicationUser user, string oldRole, string roleName)
        {
           _userManager.RemoveFromRoleAsync(user, oldRole);
            
            AddUserToRole(user, roleName);
            _context.SaveChanges();
        }

        public IdentityUserRole<string> GetUsersRole(string userId)
        {
            return _context.UserRoles.First(ur => ur.UserId == userId);
        }

        public bool IsInAnyRole(string userId)
        {
            return _context.UserRoles.Any(ur => ur.UserId == userId);
        }

    }
}
