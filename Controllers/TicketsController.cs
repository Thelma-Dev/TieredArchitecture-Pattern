using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly IRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TicketBusinessLogic _ticketBusinessLogic;
        private readonly IUserProjectRepository _userProjectRepository;

        public TicketsController(ApplicationDbContext context, IRepository<Project> projectRepository, IRepository<Ticket> ticketRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, IUserProjectRepository userProjectRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _userProjectRepository = userProjectRepository;
            _ticketBusinessLogic = new TicketBusinessLogic(userManager, projectRepository, userRepository, ticketRepository);
        }


        // GET: Tickets
        public async Task<IActionResult> Index()
        {
              return _context.Tickets != null ? 
                          View(await _context.Tickets
                          .Include(t => t.Project)
                          .Include(t => t.Owner)
                          .ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                .Include(u => u.Owner)
                .Include(t => t.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);


            List<SelectListItem> currUsers = new List<SelectListItem>();


            ticket.Project.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.User.UserName, t.User.Id.ToString()));
            });


            ViewBag.Users = currUsers;

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

       

        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projectId)
        {
            try
            {
                Project CurrentProject = _projectRepository.Get(projectId);

                UserProject userProject = _userProjectRepository.GetProject(projectId);

                List<ApplicationUser> DevelopersAssignedToProject = _userProjectRepository.GetUsersAssignedToProject(CurrentProject);


                CreateTicketVm vm = new CreateTicketVm(DevelopersAssignedToProject);

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
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority,OwnerId")] CreateTicketVm vm)
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

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == id);
      
            if (ticket == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = _context.Users.Where(u => u != ticket.Owner).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket currTicket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == ticketId);
            ApplicationUser currUser = await _context.Users.FirstAsync(u => u.Id == id);
            //To be fixed ASAP
            currTicket.Owner = currUser;
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Edit", new { id = ticketId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id,string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == userId);
                    ticket.Owner = currUser;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new {id = ticket.Id});
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == TaskId);

                    newComment.User = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    user.Comments.Add(newComment);
                    _context.Comments.Add(newComment);
                    ticket.Comments.Add(newComment);

                    int Id = TaskId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new {Id});

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.RequiredHours = hrs;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
            if (id != null)
            {
                try
                {
                    TicketWatcher newTickWatch = new TicketWatcher();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

                    newTickWatch.Ticket = ticket;
                    newTickWatch.TicketId = ticket.Id;
                    newTickWatch.Watcher = user;
                    newTickWatch.WatcherId = user.Id;
                    user.TicketWatching.Add(newTickWatch);
                    ticket.TicketWatchers.Add(newTickWatch);
                    _context.Add(newTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnWatch(int id)
        {
            if (id != null)
            {
                try
                {
                    
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    TicketWatcher currTickWatch = await _context.TicketWatchers.FirstAsync(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));
                    _context.TicketWatchers.Remove(currTickWatch);
                    ticket.TicketWatchers.Remove(currTickWatch);
                    user.TicketWatching.Remove(currTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
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
                return View(_ticketBusinessLogic.DeleteTicket(id););
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

        private bool TicketExists(int id)
        {
          return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

