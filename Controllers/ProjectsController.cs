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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly IUserProjectRepository _userProjectRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepository;

        public ProjectsController(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserProjectRepository userProjectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IHttpContextAccessor httpContextAccessor, IRepository<TicketWatcher> ticketWatcherRepository)
        {
            _userManager = userManager;
            _projectBusinessLogic = new ProjectBusinessLogic(userManager, projectRepository, userProjectRepository, userRepository, ticketRepository, httpContextAccessor, ticketWatcherRepository);
            
        }
        
        [Authorize]
		public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            try
            {
				List<ApplicationUser> AllUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

				List<SelectListItem> users = new List<SelectListItem>();


				AllUsers.ForEach(au =>
				{
					users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
				});


				ViewBag.Users = users;

                return View(_projectBusinessLogic.Read(sortOrder, page, sort, userId));
			}
            catch(Exception ex)
            {
                return BadRequest();
            }
			


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
    }
}
