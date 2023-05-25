﻿using Microsoft.AspNetCore.Identity;
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
        public TicketBusinessLogic(IRepository<Ticket> ticketRepository, UserProjectRepository userProjectRepository, UserRepository userRepository, TicketRepository ticketRepository1, ProjectRepository ProjectRepository, CommentRepository CommentRepository, TicketWatchersRepository ticketWatchersRepository) : this(ticketRepository)
        {
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository1;
            _ticketWatcherRepository = ticketWatchersRepository;
            _projectRepository = ProjectRepository;
            _commentRepository = CommentRepository;
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
                throw new ArgumentNullException("TicketId is null");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(id);

                if(ticket == null)
                {

                    throw new InvalidOperationException("Ticket not found");

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
                throw new InvalidOperationException("Project is null");
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
                throw new InvalidOperationException("Project not found");
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
            Ticket ticket = GetTicket(vm.TicketId);

            if (ticket == null)
            {
                throw new InvalidOperationException("Ticket not found");
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
            Ticket ticket = GetTicket(vm.TicketId);

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

                
                throw new ArgumentNullException("UserId is null");

            }
            else
            {
                ApplicationUser currentTicketOwner = _userRepository.Get(id);

                if(currentTicketOwner == null)
                {
                    throw new InvalidOperationException("User not found");
                }
                else
                {
                    Ticket currentTicket = GetTicket(ticketId); 
                    
                    if(currentTicket == null)
                    {
                        throw new InvalidOperationException("Ticket not found");
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

            if(ticket != null)
            {
                ticket.Completed = true;
                _ticketRepository.Update(ticket);
                
            }
            else
            {
                throw new InvalidOperationException("Ticket not found");
            }
            

        }

        public void UnMarkAsCompleted(int id)
        {
            
            Ticket ticket = GetTicket(id);

            if(ticket != null)
            {
                ticket.Completed = false;

                _ticketRepository.Update(ticket);
            }
            else
            {
                throw new InvalidOperationException("Ticket not found");
            }

        }

        public void CommentOnTask(int TaskId, string? TaskText, string username)
        {
            if (TaskId != null || TaskText != null)
            {
                
                ApplicationUser user = _userRepository.GetUserByUserName(username);


                Comment newComment = new Comment();

                Ticket ticket = GetTicket(TaskId);

                newComment.User = user;
                newComment.UserId = user.Id;
                newComment.Description = TaskText;
                newComment.Ticket = ticket;
                newComment.TicketId = ticket.Id;

                _commentRepository.Create(newComment);

                user.Comments.Add(newComment);
                ticket.Comments.Add(newComment);


            }
            else
            {
                throw new InvalidOperationException("Task not found");
            }
            
        }

        public void UpdateRequiredHours(int id, int hrs)
        {
            Ticket ticket = GetTicket(id);

            if(ticket == null)
            {
                throw new InvalidOperationException("Ticket not found");

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
            Ticket ticket = GetTicket(id);

            if(ticket == null)
            {
                throw new InvalidOperationException("Ticket not found");
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
            Ticket ticket = GetTicket(id);

            if(ticket == null)
            {
                throw new InvalidOperationException("Ticket not found");
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
                throw new ArgumentNullException("Ticket not found");
            }
            else
            {
                Ticket? ticket = GetTicket(id);

                // To allow population of related table
				List<Project> AllProjects = _projectRepository.GetAll().ToList();

				if (ticket == null)
                {
                    throw new InvalidOperationException("Ticket not found");
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
                throw new ArgumentNullException("Ticket not found");
            }
            else
            {
                Ticket? currentTicket = GetTicket(ticketId);

                if (currentTicket == null)
                {
                    throw new InvalidOperationException("Ticket not found");
                }
                else
                {
                    Project? projectWithCurrentTicket = GetProject(ProjectId);

                    if(projectWithCurrentTicket == null)
                    {
                        throw new InvalidOperationException("Project not found");
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
