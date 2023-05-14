using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using X.PagedList.Mvc;


namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly IUserProjectRepository _userProjectRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _projectBusinessLogic = new ProjectBusinessLogic(userManager, projectRepository, userProjectRepository, userRepository, ticketRepository, httpContextAccessor);
            
        }


        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<Project> SortedProjs = new List<Project>();

            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();


            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });


            ViewBag.Users = users;


            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.OrderByDescending(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.OrderBy(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.OrderByDescending(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.OrderBy(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "Completed":
                    SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.Where(t => t.Completed == true))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets.Where(t => t.Owner.Id.Equals(userId)))
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.User)
                        .Include(p => p.Tickets)
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }

                    break;
            }

            //check if User is PM or Develoer
            var LogedUserName = User.Identity.Name;  // logined user name

            var user = _context.Users.FirstOrDefault(u => u.UserName == LogedUserName);

            var rolenames = await _userManager.GetRolesAsync(user);

            var AssinedProject = new List<Project>();

            // geting assigned project

            if (rolenames.Contains("Developer"))
            {
                AssinedProject = SortedProjs
                    .Where(p => p.AssignedTo
                    .Select(projectUser => projectUser.UserId)
                    .Contains(user.Id))
                    .ToList();
            }
            else
            {
                AssinedProject = SortedProjs;
            }

            X.PagedList.IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);

            return View(projList);
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(_projectBusinessLogic.GetProjectDetails(id));
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }

            
        }

        public async Task<IActionResult> RemoveAssignedUser(string userid, int projectid)
        {
            try
            {
                _projectBusinessLogic.RemoveAssignedUser(userid, projectid);

                return RedirectToAction("Edit", new { id = projectid });
            }
            catch(Exception exe)
            {
                return Problem(exe.Message);
            }

            
        }

        
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create()
        {
            List<ApplicationUser> allDevelopers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            CreateProjectVm vm = new CreateProjectVm(allDevelopers);

            return View(vm);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName,AssignedUserId")] CreateProjectVm vm)
        {
            try
            {
                _projectBusinessLogic.CreateProject(vm);

                if(ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    vm.PopulateLists((List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer"));

                    return View(vm);
                }
                
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }

        

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
               return View(_projectBusinessLogic.EditProject(id));
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit([Bind("ProjectId,ProjectName,AssignedUserId")] EditProjectVm vm)
        {
            try
            {            
                _projectBusinessLogic.UpdateProject(vm);
                
                 return View("Index");
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }

            
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Delete(int id)
        {
            try
            {
                return View(_projectBusinessLogic.DeleteProject(id));
            }
            catch(Exception exe)
            {
                return Problem(exe.Message);
            }
        }

        

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _projectBusinessLogic.ConfirmProjectDelete(id);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception exe)
            {
                return Problem(exe.Message);
            }
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
