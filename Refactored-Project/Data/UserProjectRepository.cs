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

        public UserProject GetUserProject(int? projectId, string userId)
        {
            return _context.UserProjects.First(up => up.ProjectId == projectId && up.UserId == userId);
        }

        public UserProject? GetProject(int projectId)
        {
            return _context.UserProjects.FirstOrDefault(up => up.ProjectId == projectId);
        }

        public List<UserProject> GetProjects(int? projectId)
        {
            return _context.UserProjects.Where(up => up.ProjectId == projectId).ToList();
        }

        public List<ApplicationUser> GetUsersAssignedToProject(Project project)
        {
            return _context.UserProjects.Where(up => up.ProjectId == project.Id).Select(up => up.User).ToList();
        }

		public ICollection<UserProject> GetAll()
		{
			return _context.UserProjects.ToHashSet();
		}

		public void RemoveUserProject(UserProject userProject)
        {
            _context.RemoveUserProject(userProject);
            _context.SaveChanges();
        }

        public void CreateUserProject(UserProject userProject)
        {
            _context.UserProjects.Add(userProject);
            _context.SaveChanges();
        }

        public void UpdateUserProject(UserProject userProject)
        {
            _context.UserProjects.Update(userProject);
            _context.SaveChanges();
        }

        public void SaveChangesToDatabase()
        {
            _context.SaveChanges();
        }
    }
}
