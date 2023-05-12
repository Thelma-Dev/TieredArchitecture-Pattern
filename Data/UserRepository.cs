using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepository : IRepository<ApplicationUser>
    {
        private ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser? Get(int? id)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser? Get(string? id)
        {
            return _context.Users.First( u => u.Id == id);

        }
       
        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToHashSet<ApplicationUser>();
        }

        public void Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
