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
using X.PagedList;
using System.Collections.Immutable;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{

    public class ProjectBusinessLogic
    {
       
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;
        private IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;


        public ProjectBusinessLogic(IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IRepository<TicketWatcher> ticketWatcherRepository)
        {
            
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _ticketWatcherRepository = ticketWatcherRepository;
        }

                
        public Project GetProject(int? id)
        {
            if(id == null)
            {
                throw new ArgumentNullException("ProjectId is null");
            }
            else
            {
                Project project = _projectRepository.Get(id);

                if(project == null)
                {
                    throw new InvalidOperationException("Project not found");
                }
                else
                {
                    return project;
                }
            }
        }

        public PaginationVM Read(string? sortOrder, int? page, bool? sort, string? userId, string loggedInUserName)
        {

            List<ApplicationUser> AllDevelopers = GetAllDevelopers();

            List<SelectListItem> users = new List<SelectListItem>();


            AllDevelopers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });




            List<Project> SortedProjects = new List<Project>();
            List<Project> Projects = _projectRepository.GetAll().ToList();

            List<ApplicationUser> ProjectCreatedBy = _userRepository.GetAll().ToList();
            List<UserProject> ProjectAssignedTo = _userProjectRepository.GetAll().ToList();
            List<Ticket> ProjectTicket = _ticketRepository.GetAll().ToList();
            List<TicketWatcher> ProjectTicketWatcher = _ticketWatcherRepository.GetAll().ToList();

            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        Projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.OrderByDescending(t => t.TicketPriority).ToList();
                        });

                    }
                    else
                    {
                        Projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.OrderBy(t => t.TicketPriority).ToList();
                        });
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        Projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.OrderByDescending(t => t.RequiredHours).ToList();
                        });


                    }
                    else
                    {
                        Projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.OrderBy(t => t.RequiredHours).ToList();
                        });
                    }

                    break;
                case "Completed":

                    Projects.ForEach(p =>
                    {
                        p.Tickets = p.Tickets.Where(t => t.Completed == true).ToList();
                    });
                    break;
                default:
                    if (userId != null)
                    {

                        Projects = Projects.OrderBy(p => p.ProjectName).ToList();

                        Projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.Where(t => t.Owner.Id == userId).ToList();
                        });


                    }
                    else
                    {
                        Projects = Projects.OrderBy(p => p.ProjectName).ToList();
                    }

                    break;
            }


            //check if User is PM or Develoer

            ApplicationUser user = _userRepository.GetUserByUserName(loggedInUserName);


            // Get the role the user is in
            string roleId = _userRepository.GetUserRole(user.Id);

            IdentityRole role = _userRepository.GetRole(roleId);


            // Get assigned projects
            List<Project> AssinedProject = new List<Project>();

            PaginationVM vm = new PaginationVM();

            vm.AllDevelopers = users;


            // geting assigned project

            if (role.Name == "Developer")
            {
                AssinedProject = Projects
                    .Where(p => p.AssignedTo
                    .Select(projectUser => projectUser.UserId)
                    .Contains(user.Id))
                    .ToList();
                vm.Projects = AssinedProject.ToPagedList(page ?? 1, 3);
            }
            else
            {
                vm.Projects = Projects.ToPagedList(page ?? 1, 3);
            }

            return vm;
        }

        public Project GetProjectDetails(int id)
        {
            Project project = GetProject(id);

            return project;
        }

        public Project DeleteProject(int id)
        {
            Project project = GetProject(id);

            return project;
        }

        public void DeleteProjectConfirmed(int projectId)
        {
            Project project = GetProject(projectId);

            HashSet<Ticket> ticketList = _ticketRepository.GetAll().ToHashSet();

            
            List<Ticket> tickets = GetTicketsInProject(projectId);

            List<UserProject> userProjects = new List<UserProject>();

            if (tickets.Count != 0)
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



        public CreateProjectVm ReturnCreateProjectVm()
        {
            List<ApplicationUser> AllDevelopers = GetAllDevelopers().ToList();

            CreateProjectVm vm = new CreateProjectVm(AllDevelopers);

            return vm;
        }

        public async Task CreateProject(CreateProjectVm vm)
        {

            ApplicationUser user = _userRepository.GetUserByUserName(vm.LoggedInUsername);

            List<string> ProjectDevelopersId = vm.ProjectDevelopersId.ToList();

            List<ApplicationUser> ProjectDevelopers = new List<ApplicationUser>();

            foreach (string pd in ProjectDevelopersId)
            {
                ApplicationUser developer = _userRepository.Get(pd);

                if (developer != null)
                {
                    ProjectDevelopers.Add(developer);
                }
            }

            if (ProjectDevelopers.Count > 0)
            {
                Project newProject = new Project();

                newProject.ProjectName = vm.ProjectName;
                newProject.CreatedBy = user;
                newProject.CreatedById = user.Id;

                if (newProject != null)
                {
                    _projectRepository.Create(newProject);

                    AddUserToProject(newProject, ProjectDevelopers);

                }
                else
                {
                    throw new ArgumentException("Error Creating Project");
                }
            }
            else
            {
                List<ApplicationUser> AllDevelopers = GetAllDevelopers();
                vm.PopulateLists(AllDevelopers);
            }            
        }

        public void AddUserToProject(Project project, List<ApplicationUser> ProjectAssignees)
        {
            foreach (ApplicationUser dev in ProjectAssignees)
            {
                UserProject newUserProject = new UserProject();

                newUserProject.User = dev;
                newUserProject.UserId = dev.Id;
                newUserProject.Project = project;
                newUserProject.ProjectId = project.Id;

                project.AssignedTo.Add(newUserProject);

                _userProjectRepository.CreateUserProject(newUserProject);
            }
        }

        public EditProjectVm EditProject(int? id)
        {
            
            Project project = GetProject(id);
                
            List<UserProject> usersProjects = _userProjectRepository.GetProjects(project.Id);

            List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();

            EditProjectVm vm = new EditProjectVm(allUsers);
            vm.Project = project;
            vm.ProjectId = project.Id;

            return vm;
        }

        public void UpdateEditedProject(EditProjectVm vm)
        {

            Project project = GetProject(vm.ProjectId);
            
            List<string> ProjectDevelopersId = vm.ProjectDevelopersId.ToList();

            if (ProjectDevelopersId.Count != 0)
            {
                List<ApplicationUser> ProjectDevelopers = new List<ApplicationUser>();

                foreach (string pd in ProjectDevelopersId)
                {

                    ApplicationUser developer = _userRepository.Get(pd);

                    if (developer != null)
                    {
                        ProjectDevelopers.Add(developer);
                    }
                }

                AddUserToProject(project, ProjectDevelopers);

                project.ProjectName = vm.ProjectName;
                _projectRepository.Update(project);
            }

            // Repopulate the vm list incase of error
            List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();
            vm.PopulateLists(allUsers);
            vm.Project = project;
            vm.ProjectId = project.Id;
            
        }

        public ApplicationUser GetUserToBeRemovedFromProject(string userId)
        {
            if(userId == null)
            {
                throw new ArgumentNullException("userId not found");
            }
            else
            {
                ApplicationUser userToBeRemoved = _userRepository.Get(userId);

                if (userToBeRemoved != null)
                {
                    return userToBeRemoved;
                }
                else
                {
                    throw new InvalidOperationException("User not found");
                }
            }
        }

        public void RemoveAssignedUser(string userId, int projectId)
        {
            
            ApplicationUser user = GetUserToBeRemovedFromProject(userId);

            UserProject currentUserProject = _userProjectRepository.GetUserProject(projectId, userId);

            if (currentUserProject == null)
            {
                throw new InvalidOperationException("User Project not found");
            }
            else
            {
                _userProjectRepository.RemoveUserProject(currentUserProject);
            }
        }

        private List<Ticket> GetTicketsInProject(int projectId)
        {
            Project project = _projectRepository.Get(projectId);

            List<Ticket> tickets = new List<Ticket>();

            foreach (Ticket t in project.Tickets)
            {
                tickets.Add(t);
            }

            return tickets;
        }

        private List<string> GetUsersInProject(int projectId)
        {
            Project project = _projectRepository.Get(projectId);

            List<string> DevelopersInProject = _userProjectRepository.GetAll().Where(up => up.ProjectId == project.Id).Select(up => up.UserId).ToList();

            return DevelopersInProject.ToList();
        }

        private List<ApplicationUser> GetAllDevelopers()
        {
            List<string> DevelopersId = _userRepository.GetUserIdsInRole("Developer");


            List<ApplicationUser> AllDevelopers = new List<ApplicationUser>();


            DevelopersId.ForEach(id =>
            {
                ApplicationUser developer = _userRepository.Get(id);

                AllDevelopers.Add(developer);
            });

            return AllDevelopers;

        }

    }
}
