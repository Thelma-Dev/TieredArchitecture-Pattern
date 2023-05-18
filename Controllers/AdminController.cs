using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminBusinessLogic _adminBusinessLogic;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roles)
        {
            _userManager = userManager;
            _adminBusinessLogic = new AdminBusinessLogic(userRepository, roles, userManager);
           
        }


        public async Task<IActionResult> Index()
        {
            return View(_adminBusinessLogic.GetAllProjectManagersAndDevelopers());
        }

        public IActionResult AssignRole()
        {
            try
            {
                return View(_adminBusinessLogic.InitializeAssignRoleVm());
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleVm vm)
        {

            try
            {
               await _adminBusinessLogic.AssignRole(vm.RoleId, vm.UserId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}

