using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Project = SD_340_W22SD_Final_Project_Group6.Models.Project;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class TicketBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;
        private IRepository<Ticket> _ticketRepository;
        private UserProjectRepository userProjectRepository;
        private UserRepository userRepository;
        private TicketRepository ticketRepository1;
        private TicketWatchersRepository ticketWatchersRepository;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;
        private readonly IRepository<Comment> _commentRepository;

        public TicketBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IRepository<TicketWatcher> ticketWatcherRepository, IUserProjectRepository userProjectRepository, IRepository<Comment> commentRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _ticketWatcherRepository = ticketWatcherRepository;
            _userProjectRepository = userProjectRepository;
            _commentRepository = commentRepository;
        }
        public TicketBusinessLogic( IRepository<Ticket> ticketRepository)
        {
            _ticketRepository= ticketRepository;
        }

        public TicketBusinessLogic(IRepository<Ticket> ticketRepository, UserProjectRepository userProjectRepository, UserRepository userRepository, TicketRepository ticketRepository1, TicketWatchersRepository ticketWatchersRepository, ProjectRepository ProjectRepository) : this(ticketRepository)
        {
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository1;
            _ticketWatcherRepository = ticketWatchersRepository;
            _projectRepository = ProjectRepository;
        }

        public Project GetProject(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("ProjectId is null");
            }
            else
            {
                Project project = _projectRepository.Get(id);

                if (project == null)
                {
                    throw new InvalidOperationException("Project not found");
                }
                else
                {
                    return project;
                }
            }
        }

        public Ticket GetTicket(int? id)
        {
            if(id == null)
            {
                throw new Exception("TicketId is null");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(id);

                if(ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    return ticket;
                }
            }
        }

        public List<Ticket> Read()
        {
            List<Ticket> tickets = _ticketRepository.GetAll().ToList();

            // To populate related tables
            List<Project> AllProject = _projectRepository.GetAll().ToList() ;

            List<ApplicationUser> TicketOwners = _userRepository.GetAll().ToList();

            
            return tickets;
        }

        public Ticket GetTicketDetails(int? id)
        {
            
            Ticket ticket = GetTicket(id);


            // Including related comment, project and Application User tables

            List<Comment> comments = _commentRepository.GetAll().ToList();            
            List<Project> allProjects = _projectRepository.GetAll().ToList();
            List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();
            List<TicketWatcher> AllTicketWatchers = _ticketWatcherRepository.GetAll().ToList();


            TicketWatcher ticketWatcher = ticket.TicketWatchers.FirstOrDefault(tw => tw.TicketId == ticket.Id);

            if (ticketWatcher != null)
            {
				ApplicationUser watcher = ticketWatcher.Watcher;
			}
            else
            {
				ApplicationUser TicketOwner = ticket.Owner;
				Comment TicketComments = ticket.Comments.FirstOrDefault(c => c.TicketId == ticket.Id);

                if (TicketComments != null)
                {
					ApplicationUser TicketCommenter = TicketComments.User;
				}
						
			}

            return ticket;
        }

        public CreateTicketVm InitializeCreateTicketMethod(int projectId)
        {
            Project CurrentProject = GetProject(projectId);

            if(CurrentProject == null)
            {
                throw new Exception("Project is null");
            }
            else
            {
                UserProject userProject = _userProjectRepository.GetProject(projectId);

                List<ApplicationUser> DevelopersAssignedToProject = _userProjectRepository.GetUsersAssignedToProject(CurrentProject);

                CreateTicketVm vm = new CreateTicketVm(DevelopersAssignedToProject);
                vm.Project = CurrentProject;
                vm.ProjectId = CurrentProject.Id;

                return vm;
            }           
        }

        public void CreateTicket(CreateTicketVm vm)
        {
            Project CurrentProject = GetProject(vm.ProjectId);

            if (CurrentProject == null)
            {
                throw new Exception("Project not found");
            }
            else
            {

                ApplicationUser owner = vm.Owner;

                if (owner == null)
                {
                    throw new InvalidOperationException();
                } else
                {
                    owner = _userRepository.Get(vm.OwnerId);
                }

                Ticket newTicket = new Ticket();

                newTicket.Title = vm.Title;
                newTicket.Body = vm.Body;
                newTicket.RequiredHours = vm.RequiredHours;
                newTicket.Project = CurrentProject;
                newTicket.TicketPriority = vm.TicketPriority;
                newTicket.Owner = owner;
                

                _ticketRepository.Create(newTicket);
                CurrentProject.Tickets.Add(newTicket);
                _ticketRepository.SaveChanges();

            }           
        }

        public CreateTicketVm RepopulateDevelopersInProjectList(CreateTicketVm vm)
        {
            Project currentProject = GetProject(vm.ProjectId);

            List<ApplicationUser> DevelopersAssignedToProject = _userProjectRepository.GetUsersAssignedToProject(currentProject);

            vm.PopulateLists(DevelopersAssignedToProject);
            vm.Project = currentProject;
            vm.ProjectId = currentProject.Id;

            return vm;
        }



        public EditTicketVm EditTicket(int? id)
        {
            
                Ticket ticket = GetTicket(id);

               List<ApplicationUser> user = _userRepository.GetAll().ToList();
                
                
                ApplicationUser owner = ticket.Owner;

                List<ApplicationUser> DevelopersNotInTicket = _userRepository.GetAll().Where(u => u != ticket.Owner).ToList();

                EditTicketVm vm = new EditTicketVm(DevelopersNotInTicket);
                vm.Ticket = ticket;
                vm.TicketId = ticket.Id;
                vm.Owner = owner;

                return vm;       
            
        }

        public void UpdateEditedTicket(EditTicketVm vm)
        {
            Ticket ticket = _ticketRepository.Get(vm.TicketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                ApplicationUser newOwner = _userRepository.Get(vm.OwnerId);

                ticket.Title= vm.Title;
                ticket.Body= vm.Body;
                ticket.RequiredHours= vm.RequiredHours;
                ticket.Owner = newOwner;

                _ticketRepository.Update(ticket);
            }

            vm.TicketId= ticket.Id;
            vm.Ticket = ticket;
        }

        public EditTicketVm RepopulateDevelopersNotInTicket(EditTicketVm vm)
        {
            Ticket ticket = _ticketRepository.Get(vm.TicketId);

            ApplicationUser owner = ticket.Owner;

            List<ApplicationUser> DevelopersNotInTicket = _userRepository.GetAll().Where(u => u != ticket.Owner).ToList();

            vm.PopulateLists(DevelopersNotInTicket);
            vm.Ticket = ticket;
            vm.TicketId = ticket.Id;

            return vm;
        }

        public void RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                throw new  Exception("UserId is null");
            }
            else
            {
                ApplicationUser currentTicketOwner = _userRepository.Get(id);

                if(currentTicketOwner == null)
                {
                    throw new Exception("User not found");
                }
                else
                {
                    Ticket currentTicket = _ticketRepository.Get(ticketId); 
                    
                    if(currentTicket == null)
                    {
                        throw new Exception("Ticket not found");
                    }
                    else
                    {
                        currentTicket.Owner = null;
                        _ticketRepository.Update(currentTicket);
                    }
                }
            }
            
        }

        public void MarkAsCompleted(int id)
        {
            
             Ticket ticket = GetTicket(id);

                
             ticket.Completed = true;

            _ticketRepository.Update(ticket);
                             
            
        }

        public void UnMarkAsCompleted(int id)
        {
            
            Ticket ticket = GetTicket(id);
                
            ticket.Completed = false;

            _ticketRepository.Update(ticket);
            
        }

        public void CommentOnTask(int TaskId, string? TaskText, string username)
        {
            if (TaskId != null || TaskText != null)
            {
                
                ApplicationUser user = _userRepository.GetUserByUserName(username);


                Comment newComment = new Comment();

                Ticket ticket = _ticketRepository.Get(TaskId);

                newComment.User = user;
                newComment.UserId = user.Id;
                newComment.Description = TaskText;
                newComment.Ticket = ticket;
                newComment.TicketId = ticket.Id;

                _commentRepository.Create(newComment);

                user.Comments.Add(newComment);
                ticket.Comments.Add(newComment);
                
                
            }
            
        }

        public void UpdateRequiredHours(int id, int hrs)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            } else if (hrs < 1 || hrs > 999)
            {
                throw new InvalidOperationException();
            }
            else
            {
                ticket.RequiredHours = hrs;
                _ticketRepository.Update(ticket);
            }
            
        }

        public void AddToWatch(int id, string username)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                
                ApplicationUser user = _userRepository.GetUserByUserName(username);

                TicketWatcher newTickWatch = new TicketWatcher();

                newTickWatch.Ticket = ticket;
                newTickWatch.TicketId = ticket.Id;
                newTickWatch.Watcher = user;
                newTickWatch.WatcherId = user.Id;

                user.TicketWatching.Add(newTickWatch);
                ticket.TicketWatchers.Add(newTickWatch);

                _ticketWatcherRepository.Create(newTickWatch);
            }
        }

        public void Unwatch(int id, string username)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                
                ApplicationUser userLoggedIn = _userRepository.GetUserByUserName(username);

                TicketWatcher currentTicketWatcher = _ticketWatcherRepository.GetAll().First(tw => tw.TicketId == ticket.Id && tw.WatcherId == userLoggedIn.Id);

                if(currentTicketWatcher != null)
                {
                    _ticketWatcherRepository.Delete(currentTicketWatcher);
                    ticket.TicketWatchers.Remove(currentTicketWatcher);
                    userLoggedIn.TicketWatching.Remove(currentTicketWatcher);
                    _ticketWatcherRepository.SaveChanges();

                    
                }
                else
                {
                    throw new Exception("You are not currently watching this ticket");
                }
               
            }
            
        }

        public Ticket DeleteTicket(int? id)
        {
            if (id == null )
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(id);

                // To allow population of related table
				List<Project> AllProjects = _projectRepository.GetAll().ToList();

				if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    return ticket;
                }

                
            }            
        }

        public void TicketDeleteConfirmed(int ticketId, int ProjectId)
        {
            if(ticketId == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                Ticket? currentTicket = _ticketRepository.Get(ticketId);

                if (currentTicket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    Project? projectWithCurrentTicket = _projectRepository.Get(ProjectId);

                    if(projectWithCurrentTicket == null)
                    {
                        throw new Exception("Project not found");
                    }
                    else
                    {
                        projectWithCurrentTicket.Tickets.Remove(currentTicket);

                        _ticketRepository.Delete(currentTicket);
                    }
                }

            }
            
        }

        
    }
}
