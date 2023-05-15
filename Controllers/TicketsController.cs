using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using Project = SD_340_W22SD_Final_Project_Group6.Models.Project;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly IRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TicketBusinessLogic _ticketBusinessLogic;
        private readonly IUserProjectRepository _userProjectRepository;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;

        public TicketsController(IRepository<Project> projectRepository, IRepository<Ticket> ticketRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, IUserProjectRepository userProjectRepository, IHttpContextAccessor httpContextAccessor, IRepository<TicketWatcher> ticketWatcherRepository)
        {
            _projectRepository = projectRepository;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _userProjectRepository = userProjectRepository;
            _ticketBusinessLogic = new TicketBusinessLogic(userManager, projectRepository, userRepository, ticketRepository, httpContextAccessor, ticketWatcherRepository, userProjectRepository);
        }


        // GET: Tickets
        public IActionResult Index()
        {
            return View(_ticketBusinessLogic.Read());
        }



        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                return View(_ticketBusinessLogic.GetTicketDetails(id));
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
           
        }

       

        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projectId)
        {
            try
            {
                Project CurrentProject = _projectRepository.Get(projectId);

                UserProject userProject = _userProjectRepository.GetProject(projectId);

                List<ApplicationUser> DevelopersAssignedToProject = _userProjectRepository.GetUsersAssignedToProject(CurrentProject);

                ViewBag.Project = CurrentProject.ProjectName;
                ViewBag.ProjectId = CurrentProject.Id;

                CreateTicketVm vm = new CreateTicketVm(DevelopersAssignedToProject);
                vm.Project = CurrentProject;
                vm.ProjectId = CurrentProject.Id;

                return View(vm);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
            

        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority,OwnerId,ProjectId")] CreateTicketVm vm)
        {
            if (ModelState.IsValid)
            {
                _ticketBusinessLogic.CreateTicket(vm);

                return RedirectToAction("Index","Projects", new { area = ""});
            }
            else
            {
				
				List<ApplicationUser> DevelopersAssignedToProject = _userProjectRepository.GetUsersAssignedToProject(vm.Project);

				CreateTicketVm newVm = new CreateTicketVm(DevelopersAssignedToProject);

				return View(newVm);
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                return View(_ticketBusinessLogic.EditTicket(id));
            }
            catch(Exception exe)
            {
                return BadRequest();
            }
        }

                
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit([Bind("TicketId,Title,Body,RequiredHours,OwnerId")] EditTicketVm vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ticketBusinessLogic.UpdateTicket(vm);

                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    Ticket ticket = _ticketRepository.Get(vm.TicketId);

                    ApplicationUser owner = ticket.Owner;

                    List<ApplicationUser> DevelopersNotInTicket = _userRepository.GetAll().Where(u => u != ticket.Owner).ToList();

                    EditTicketVm newVm = new EditTicketVm(DevelopersNotInTicket);
                    newVm.Ticket = ticket;
                    newVm.TicketId = ticket.Id;

                    return View(newVm);
                }
            }
            catch(Exception exe)
            {
                return BadRequest();
            }
        }


        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            try
            {
                _ticketBusinessLogic.RemoveAssignedUser(id, ticketId);

                return RedirectToAction("Edit", new { id = ticketId });
            }
            catch(Exception exe)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            try
            {
                if (TaskId == null || TaskText == null)
                {
                    return RedirectToAction("Index");
                }
                    
                _ticketBusinessLogic.CommentOnTask(TaskId, TaskText);
                    
                return RedirectToAction("Details", new {id = TaskId});

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            
            
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
                        
            try
            {
                if (id != null || hrs != null)
                {
                    return RedirectToAction("Index");
                }
                
                _ticketBusinessLogic.UpdateRequiredHours(id, hrs);

                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            
            
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
                        
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                _ticketBusinessLogic.AddToWatch(id);
                
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            
            
        }

        public async Task<IActionResult> UnWatch(int id)
        {
                        
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                _ticketBusinessLogic.Unwatch(id);

                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            
            
        }

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            
            try
            {
                _ticketBusinessLogic.MarkASCompleted(id);

                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            try
            {
                _ticketBusinessLogic.MarkASCompleted(id);

                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }


        
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                return View(_ticketBusinessLogic.DeleteTicket(id));
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id, int projectId)
        {
            try
            {
                _ticketBusinessLogic.ConfirmDeleteTicket(id, projectId);

                return RedirectToAction("Index", "Projects");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

            
        }
    }
}

