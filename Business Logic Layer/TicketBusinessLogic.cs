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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;

        public TicketBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IHttpContextAccessor httpContextAccessor, IRepository<TicketWatcher> ticketWatcherRepository, IUserProjectRepository userProjectRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _httpContextAccessor = httpContextAccessor;
            _ticketWatcherRepository = ticketWatcherRepository;
            _userProjectRepository = userProjectRepository;
        }

        public List<Ticket> Read()
        {
            List<Ticket> tickets = _ticketRepository.GetAll().ToList();

            List<Project> TicketProject = new List<Project>();

            List<ApplicationUser> TicketOwners = new List<ApplicationUser>();

            foreach(Ticket ticket in tickets)
            {
                TicketOwners.Add(ticket.Owner);
                TicketProject.Add(ticket.Project);
            }

            return tickets;
        }

        public Ticket GetTicketDetails(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Ticket not found");
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
                    // Including related project and Application User tables
                    List<Project> allProjects = _projectRepository.GetAll().ToList();

                    List<ApplicationUser> allUsers = _userRepository.GetAll().ToList();


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
            }  
        }

        public void CreateTicket(CreateTicketVm vm)
        {
            Project CurrentProject = _projectRepository.Get(vm.ProjectId);

            if (CurrentProject == null)
            {
                throw new Exception("Project not found");
            }
            else
            {

                ApplicationUser owner = _userRepository.Get(vm.OwnerId);

                Ticket newTicket = new Ticket();

                newTicket.Title = vm.Title;
                newTicket.Body = vm.Body;
                newTicket.RequiredHours = vm.RequiredHours;
                newTicket.Project = CurrentProject;
                newTicket.TicketPriority = vm.TicketPriority;
                newTicket.Owner = owner;
                

                _ticketRepository.Create(newTicket);
                CurrentProject.Tickets.Add(newTicket);
                _userProjectRepository.SaveChangesToDatabase();
            }           
        }

        public EditTicketVm EditTicket(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Ticket not found");
            }
            else
            {
                Ticket ticket = _ticketRepository.Get(id);

               List<ApplicationUser> user = _userRepository.GetAll().ToList();
                

                if(ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    ApplicationUser owner = ticket.Owner;

                    List<ApplicationUser> DevelopersNotInTicket = _userRepository.GetAll().Where(u => u != ticket.Owner).ToList();

                    EditTicketVm vm = new EditTicketVm(DevelopersNotInTicket);
                    vm.Ticket = ticket;
                    vm.TicketId = ticket.Id;
                    vm.Owner = owner;

                    return vm;

                }              
            }
        }

        public void UpdateTicket(EditTicketVm vm)
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
        }

        public void RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                throw new  ArgumentNullException("User not found");
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

        public void MarkASCompleted(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Ticket not found");
                
            }
            else
            {

                Ticket ticket = _ticketRepository.Get(id);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    ticket.Completed = true;

                    _ticketRepository.Update(ticket);
                }                
            }
        }

        public void UnMarkAsCompleted(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Ticket not found");

            }
            else
            {

                Ticket ticket = _ticketRepository.Get(id);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    ticket.Completed = false;

                    _ticketRepository.Update(ticket);
                }
            }
        }

        public void CommentOnTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser user = _userRepository.Get(userId);


                Comment newComment = new Comment();

                Ticket ticket = _ticketRepository.Get(TaskId);

                newComment.User = user;
                newComment.Description = TaskText;
                newComment.Ticket = ticket;

                user.Comments.Add(newComment);
                ticket.Comments.Add(newComment);
                
                //_context.comment.Add(newComment);
            }
            
        }

        public void UpdateRequiredHours(int id, int hrs)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                ticket.RequiredHours = hrs;
                _ticketRepository.Update(ticket);
            }
            
        }

        public void AddToWatch(int id)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser user = _userRepository.Get(userId);

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

        public void Unwatch(int id)
        {
            Ticket ticket = _ticketRepository.Get(id);

            if(ticket == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser userLoggedIn = _userRepository.Get(userId);

                TicketWatcher currentTicketWatcher = _ticketWatcherRepository.GetAll().First(tw => tw.TicketId == ticket.Id && tw.WatcherId == userLoggedIn.Id);

                if(currentTicketWatcher != null)
                {
                    _ticketWatcherRepository.Delete(currentTicketWatcher);
                    ticket.TicketWatchers.Remove(currentTicketWatcher);
                    userLoggedIn.TicketWatching.Remove(currentTicketWatcher);
                    _ticketWatcherRepository.SaveChanges();

                    // Save changes to the database
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
                throw new ArgumentNullException("Ticket not found");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(id);

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

        public void ConfirmDeleteTicket(int ticketId, int ProjectId)
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
