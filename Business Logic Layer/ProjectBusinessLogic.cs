using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.CodeAnalysis.Differencing;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class ProjectBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;
        private IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _httpContextAccessor = httpContextAccessor;
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

        public void ConfirmProjectDelete(int projectId)
        {
            Project project = _projectRepository.Get(projectId);

            HashSet<Ticket> ticketList = _ticketRepository.GetAll().ToHashSet();

            if (project == null)
            {
                throw new Exception("Project not found");
            }
            else
            {
                List<Ticket> tickets = GetTicketsInProject(projectId);

                List<UserProject> userProjects = new List<UserProject>();

                if(tickets.Count != 0)
                {
                    tickets.ForEach(ticket =>
                    {
                        _ticketRepository.Delete(ticket);
                    });

                    userProjects = _userProjectRepository.GetProjects(projectId);

                    userProjects.ForEach(userProj =>
                    {
                        _userProjectRepository.RemoveUserProject(userProj);
                    });

                }
                else
                {
                    userProjects = _userProjectRepository.GetProjects(projectId);

                    userProjects.ForEach(userProj =>
                    {
                        _userProjectRepository.RemoveUserProject(userProj);
                    });
                }

                _projectRepository.Delete(project);

            }

            
            
        }

        public void CreateProject(CreateProjectVm vm)
        {
            string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser user = _userRepository.Get(userId);


            Project newProject = new Project();

            newProject.ProjectName = vm.ProjectName;
            newProject.CreatedBy = user;
            newProject.CreatedById = user.Id;

            if (newProject != null)
            {
                _projectRepository.Create(newProject);

                ApplicationUser developer = _userRepository.Get(vm.AssignedUserId);

                UserProject newUserProject = new UserProject();

                newUserProject.User = developer;
                newUserProject.UserId = developer.Id;
                newUserProject.Project = newProject;
                newUserProject.ProjectId = newProject.Id;

                newProject.AssignedTo.Add(newUserProject);
                    
                _userProjectRepository.CreateUserProject(newUserProject);
                
            }
            else
            {
                throw new Exception("Error Creating Project");
            }
        }

        public EditProjectVm EditProject(int? id)
        {
            if (id == null)
            {
                throw new Exception("Project not found");
            }
            else
            {
                Project project = _projectRepository.Get(id);

                if(project == null)
                {
                    throw new Exception("Project not found");
                }
                else
                {
                    List<UserProject> usersProjects = _userProjectRepository.GetProjects(project.Id);

                    List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();

                    EditProjectVm vm = new EditProjectVm(allUsers);
                    vm.Project = project;
                    vm.ProjectId = project.Id;

                    return vm;
                }
            }
            
        }

        public void UpdateProject(EditProjectVm vm)
        {

            Project project = _projectRepository.Get(vm.ProjectId);

            if(project == null )
            {
                throw new Exception("Project not found");
            }
            else
            {
                ApplicationUser developer = _userRepository.Get(vm.AssignedUserId);

                // The code below throws an error
                UserProject? OldUserProject = _userProjectRepository.GetProject(project.Id);

                if (OldUserProject == null)
                {
                    project.ProjectName = vm.ProjectName;
                    _projectRepository.Update(project);

                    UserProject newUserProject = new UserProject();
                    newUserProject.ProjectId = project.Id;
                    newUserProject.Project = project;
                    newUserProject.User = developer;
                    newUserProject.UserId = developer.Id;

                    project.AssignedTo.Add(newUserProject);
                    _userProjectRepository.CreateUserProject(newUserProject);
                }
                else
                {
                    project.ProjectName = vm.ProjectName;
                    _projectRepository.Update(project);


                    OldUserProject.User = developer;
                    OldUserProject.UserId = developer.Id;
                    OldUserProject.Project = project;
                    OldUserProject.ProjectId = project.Id;

                    _userProjectRepository.UpdateUserProject(OldUserProject);
                }                
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

        private List<Ticket> GetTicketsInProject(int projectId)
        {
            Project project = _projectRepository.Get(projectId);

            List<Ticket> tickets = new List<Ticket>();

            foreach(Ticket t in project.Tickets)
            {
                tickets.Add(t);
            }

            return tickets;
        }
    }
}
