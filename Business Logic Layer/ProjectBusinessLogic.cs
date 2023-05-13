using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Runtime.InteropServices;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class ProjectBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;

        public ProjectBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;

        }

        public Project GetProjectDetails(int id)
        {
            Project? project = _projectRepository.Get(id);

            if (project == null)
            {
                throw new Exception("Project not found");
            }
            else
            {
                return project;
            }   
        }

        public Project DeleteProject(int id)
        {
            Project project = _projectRepository.Get(id);

            if (project == null)
            {
                throw new Exception("Project not found");
            }
            else
            {
                return project;
            }
        }

        public void ConfirmProjectDelete(int id)
        {
            Project project = _projectRepository.Get(id);

            if (project == null)
            {
                throw new Exception("Project not found");
            }
            else
            {
                List<Ticket> tickets = project.Tickets.ToList();

                tickets.ForEach(ticket =>
                {
                    _context.Tickets.Remove(ticket);
                });


                List<UserProject> userProjects = _userProjectRepository.GetProjects(id);


                userProjects.ForEach(userProj =>
                {
                    _userProjectRepository.RemoveUserProject(userProj);
                });

                _projectRepository.Delete(project);


            }

            
            
        }

        public void  RemoveAssignedUser(string userId, int projectId)
        {
            ApplicationUser user = _userRepository.Get(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                UserProject currentUserProject = _userProjectRepository.GetUserProject(projectId, userId);

                if (currentUserProject == null)
                {
                    throw new Exception("User Project not found");
                }
                else
                {
                    _userProjectRepository.RemoveUserProject(currentUserProject);
                }
            }
           
        }
    }
}
