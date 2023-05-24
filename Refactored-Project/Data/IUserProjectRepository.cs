using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IUserProjectRepository
    {
        UserProject GetUserProject(int? projectId, string userId);

        void RemoveUserProject(UserProject userProject);

        List<UserProject> GetProjects(int projectId);

        void CreateUserProject(UserProject userProject);

        void UpdateUserProject(UserProject userProject);

        ICollection<UserProject> GetAll();


		UserProject GetProject(int projectId);

        List<ApplicationUser> GetUsersAssignedToProject(Project project);

        void SaveChangesToDatabase();

	}
}
