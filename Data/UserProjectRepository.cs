using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserProjectRepository : IUserProjectRepository
    {
        private ApplicationDbContext _context;

        public UserProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserProject GetUserProject(int projectId, string userId)
        {
            return _context.UserProjects.First(up => up.ProjectId == projectId && up.UserId == userId);
        }

        public List<UserProject> GetProjects(int projectId)
        {
            return _context.UserProjects.Where(up => up.ProjectId == projectId).ToList();
        }

        public void RemoveUserProject(UserProject userProject)
        {
            _context.UserProjects.Remove(userProject);
            _context.SaveChanges();
        }
    }
}
