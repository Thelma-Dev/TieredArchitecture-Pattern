using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public async void SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
