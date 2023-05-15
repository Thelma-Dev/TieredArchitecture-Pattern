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

        public IPagedList<Project> Read(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<Project> SortedProjects = new List<Project>();
            
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        List<Project> Projects = _projectRepository.GetAll().ToList();
                        List<ApplicationUser> ProjectCreatedBy = new List<ApplicationUser>();
                        List<UserProject> ProjectAssignedTo = new List<UserProject>();
                        List<ApplicationUser>ProjectUser = new List<ApplicationUser>();
                        List<ApplicationUser> ProjectOwner = new List<ApplicationUser>();
                        List<Ticket> ProjectTicket = new List<Ticket>();

                        foreach(Project p in Projects)
                        {
                            ProjectCreatedBy.Add(p.CreatedBy);
                            ProjectAssignedTo = p.AssignedTo.ToList();
                            ProjectUser = p.AssignedTo.Select(x => x.User).ToList();
                            ProjectOwner = p.Tickets.Select(x => x.Owner).ToList();
                            ProjectTicket = p.Tickets.OrderByDescending(t => t.TicketPriority).ToList();
                        }

                        SortedProjects = Projects.ToList();
                        
                    }
                    else
                    {
						List<Project> Projects = _projectRepository.GetAll().ToList();
						List<ApplicationUser> ProjectCreatedBy = new List<ApplicationUser>();
						List<UserProject> ProjectAssignedTo = new List<UserProject>();
						List<ApplicationUser> ProjectUser = new List<ApplicationUser>();
						List<ApplicationUser> ProjectOwner = new List<ApplicationUser>();
						List<Ticket> ProjectTicket = new List<Ticket>();

						foreach (Project p in Projects)
						{
							ProjectCreatedBy.Add(p.CreatedBy);
							ProjectAssignedTo = p.AssignedTo.ToList();
							ProjectUser = p.AssignedTo.Select(x => x.User).ToList();
							ProjectOwner = p.Tickets.Select(x => x.Owner).ToList();
							ProjectTicket = p.Tickets.OrderBy(t => t.TicketPriority).ToList();
						}

						SortedProjects = Projects.ToList();
					}

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
						List<Project> AllProject = _projectRepository.GetAll().ToList();
						List<ApplicationUser> ProjCreatedBy = new List<ApplicationUser>();
						List<UserProject> ProjAssignedTo = new List<UserProject>();
						List<ApplicationUser> ProjUser = new List<ApplicationUser>();
						List<ApplicationUser> ProjOwner = new List<ApplicationUser>();
						List<Ticket> ProjTicket = new List<Ticket>();

						foreach (Project p in AllProject)
						{
							ProjCreatedBy.Add(p.CreatedBy);
							ProjAssignedTo = p.AssignedTo.ToList();
							ProjUser = p.AssignedTo.Select(x => x.User).ToList();
							ProjOwner = p.Tickets.Select(x => x.Owner).ToList();
							ProjTicket = p.Tickets.OrderByDescending(t => t.RequiredHours).ToList();
						}

						SortedProjects = AllProject.ToList();
					}
                    else
                    {
						List<Project> AllProject = _projectRepository.GetAll().ToList();
						List<ApplicationUser> ProjCreatedBy = new List<ApplicationUser>();
						List<UserProject> ProjAssignedTo = new List<UserProject>();
						List<ApplicationUser> ProjUser = new List<ApplicationUser>();
						List<ApplicationUser> ProjOwner = new List<ApplicationUser>();
						List<Ticket> ProjTicket = new List<Ticket>();

						foreach (Project p in AllProject)
						{
							ProjCreatedBy.Add(p.CreatedBy);
							ProjAssignedTo = p.AssignedTo.ToList();
							ProjUser = p.AssignedTo.Select(x => x.User).ToList();
							ProjOwner = p.Tickets.Select(x => x.Owner).ToList();
							ProjTicket = p.Tickets.OrderBy(t => t.RequiredHours).ToList();
						}

						SortedProjects = AllProject.ToList();
					}

                    break;
                case "Completed":
					List<Project> AllProjects = _projectRepository.GetAll().ToList();
					List<ApplicationUser> CreatedBy = new List<ApplicationUser>();
					List<UserProject> AssignedTo = new List<UserProject>();
					List<ApplicationUser> User = new List<ApplicationUser>();
					List<ApplicationUser> Owner = new List<ApplicationUser>();
					List<Ticket> Ticket = new List<Ticket>();

					foreach (Project p in AllProjects)
					{
						CreatedBy.Add(p.CreatedBy);
						AssignedTo = p.AssignedTo.ToList();
						User = p.AssignedTo.Select(x => x.User).ToList();
						Owner = p.Tickets.Select(x => x.Owner).ToList();
						Ticket = p.Tickets.OrderByDescending(t => t.Completed == true).ToList();
					}

					SortedProjects = AllProjects.ToList();
					break;
                default:
                    if (userId != null)
                    {
						AllProjects = _projectRepository.GetAll().ToList();
						CreatedBy = new List<ApplicationUser>();
						AssignedTo = new List<UserProject>();
						User = new List<ApplicationUser>();
						Owner = new List<ApplicationUser>();
						Ticket = new List<Ticket>();
                        List<TicketWatcher> TicketWatchers = new List<TicketWatcher>();
                        List<ApplicationUser> Watcher = new List<ApplicationUser>();

						foreach (Project p in AllProjects)
						{
							CreatedBy.Add(p.CreatedBy);
							AssignedTo = p.AssignedTo.ToList();
							User = p.AssignedTo.Select(x => x.User).ToList();
							Owner = p.Tickets.Select(x => x.Owner).ToList();
							Ticket = p.Tickets.Where(t => t.Owner.Id == userId).ToList();
							//TicketWatchers = p.Tickets.Select(t => t.TicketWatchers).ToList();
							//Watcher = TicketWatchers.Select(tw => tw.Watcher).ToList();
						}

						SortedProjects = AllProjects.ToList();

						
                    }
                    else
                    {
						AllProjects = _projectRepository.GetAll().ToList();
						CreatedBy = new List<ApplicationUser>();
						AssignedTo = new List<UserProject>();
						User = new List<ApplicationUser>();
						Owner = new List<ApplicationUser>();
						Ticket = new List<Ticket>();
						List<TicketWatcher> TicketWatchers = new List<TicketWatcher>();
						List<ApplicationUser> Watcher = new List<ApplicationUser>();

						foreach (Project p in AllProjects)
						{
							CreatedBy.Add(p.CreatedBy);
							AssignedTo = p.AssignedTo.ToList();
							User = p.AssignedTo.Select(x => x.User).ToList();
							Owner = p.Tickets.Select(x => x.Owner).ToList();
							Ticket = p.Tickets.ToList();
							//TicketWatchers = p.Tickets.Select(t => t.TicketWatchers).ToList();
							//Watcher = TicketWatchers.Select(tw => tw.Watcher).ToList();
						}

						SortedProjects = AllProjects.ToList();
					}

                    break;
            }


			//check if User is PM or Develoer

			string LoggedUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

			ApplicationUser user = _userRepository.Get(userId);


            // Get the role the user is in
            string roleId = _userRepository.GetUserRole(LoggedUserId);

            IdentityRole role = _userRepository.GetRole(roleId);


            // Get assigned projects
            List<Project> AssinedProject = new List<Project>();


            // geting assigned project

            if (role.Name == "Developer")
            {
                AssinedProject = SortedProjects
                    .Where(p => p.AssignedTo
                    .Select(projectUser => projectUser.UserId)
                    .Contains(user.Id))
                    .ToList();
            }
            else
            {
                AssinedProject = SortedProjects;
            }

            X.PagedList.IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);

            return projList;
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
