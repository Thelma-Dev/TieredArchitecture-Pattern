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
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;
        private IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;

        public object ViewBag { get; private set; }

        public ProjectBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IHttpContextAccessor httpContextAccessor, IRepository<TicketWatcher> ticketWatcherRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _httpContextAccessor = httpContextAccessor;
            _ticketWatcherRepository = ticketWatcherRepository;
        }

        public IPagedList<Project> Read(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<ApplicationUser> AllDevelopers = GetAllDevelopers();
            
            List<SelectListItem> users = new List<SelectListItem>();


            AllDevelopers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });


            //ViewBag.Users = users;

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
						p.Tickets = p.Tickets.OrderByDescending(t => t.Completed).ToList();
					});
					break;
                default:
                    if (userId != null)
                    {
						Projects.OrderBy(p => p.ProjectName).ToList();

						Projects.ForEach(p =>
						{
							p.Tickets = p.Tickets.Where(t => t.Owner.Id == userId).ToList();
						});

											
                    }
                    else
                    {
                        Projects.OrderBy(p => p.ProjectName).ToList();

						SortedProjects = Projects.ToList();
					}

                    break;
            }


			//check if User is PM or Develoer

			string LoggedUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

			ApplicationUser user = _userRepository.Get(LoggedUserId);


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

            X.PagedList.IPagedList<Project> projList = Projects.ToPagedList(page ?? 1, 3);

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
            if(id == null)
            {
                throw new Exception("Project not found");
            }
            else
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

            Project newProject = new Project();

            newProject.ProjectName = vm.ProjectName;
            newProject.CreatedBy = user;
            newProject.CreatedById = user.Id;

            if (newProject != null)
            {
                _projectRepository.Create(newProject);

                foreach (ApplicationUser dev in ProjectDevelopers)
                {
                    UserProject newUserProject = new UserProject();

                    newUserProject.User = dev;
                    newUserProject.UserId = dev.Id;
                    newUserProject.Project = newProject;
                    newUserProject.ProjectId = newProject.Id;

                    newProject.AssignedTo.Add(newUserProject);

                    _userProjectRepository.CreateUserProject(newUserProject);
                }
                
            }
            else
            {
                throw new Exception("Error Creating Project");
            }

            List<ApplicationUser> AllDevelopers = GetAllDevelopers();
            vm.PopulateLists(AllDevelopers);
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
                List<string> ProjectDevelopersId = vm.ProjectDevelopersId.ToList();

                if (ProjectDevelopersId.Count == 0)
                {
                    List<string> DevelopersAlreadyInProject = GetUsersInProject(project.Id).ToList();

                    if (DevelopersAlreadyInProject.Count > 0)
                    {
                        project.ProjectName = vm.ProjectName;
                        _projectRepository.Update(project);
                    }
                }
                else
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

                    foreach (ApplicationUser dev in ProjectDevelopers)
                    {
                        UserProject newUserProject = new UserProject();

                        newUserProject.User = dev;
                        newUserProject.UserId = dev.Id;
                        newUserProject.Project = project;
                        newUserProject.ProjectId = project.Id;

                        project.AssignedTo.Add(newUserProject);
                        _userProjectRepository.CreateUserProject(newUserProject);
                    }

                    project.ProjectName = vm.ProjectName;
                    _projectRepository.Update(project);


                    // Repopulate the vm list incase of error
                    List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();
                    vm.PopulateLists(allUsers);
                    vm.Project = project;
                    vm.ProjectId = project.Id;
                }

            }    
        }

        public void  RemoveAssignedUser(string userId, int projectId)
        {
            if(userId == null)
            {
                throw new Exception("No user found");
            }
            else
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
