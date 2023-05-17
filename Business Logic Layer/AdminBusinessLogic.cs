using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Collections;
using System.Data;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class AdminBusinessLogic
    {
        private IUserRepository _userRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminBusinessLogic(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public ICollection<ApplicationUser> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public ApplicationUser GetUser(string userId)
        {
            return _userRepository.Get(userId);
        }

        public ProjectManagersAndDevelopersVm GetAllProjectManagersAndDevelopers()
        {
            ProjectManagersAndDevelopersVm vm = new ProjectManagersAndDevelopersVm();

            
            List<string> DevelopersId = _userRepository.GetUserIdsInRole("Developer");

            List<string> ProjectManagersId = _userRepository.GetUserIdsInRole("ProjectManager");

            // Get the objects
            List<ApplicationUser> AllDevelopers = new List<ApplicationUser>();

            List<ApplicationUser> AllProjectManagers = new List<ApplicationUser>();

            List<ApplicationUser> AllUsers = GetAllUsers().ToList();

            DevelopersId.ForEach(id =>
            {
                ApplicationUser developer = _userRepository.Get(id);

                AllDevelopers.Add(developer);
            });

            ProjectManagersId.ForEach(id =>
            {
                ApplicationUser projectManager = _userRepository.Get(id);

                AllProjectManagers.Add(projectManager);

            });

            
            vm.ProjectManagers = AllProjectManagers;
            vm.devs = AllDevelopers;
            vm.allUsers = AllUsers;

            return vm;
        }

        public async Task<IdentityRole> GetRoleByRoleNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public AssignRoleVm InitializeAssignRoleVm()
        {
            List<ApplicationUser> allUsers = GetAllUsers().ToList();
            HashSet<IdentityRole> AllRoles = _roleManager.Roles.ToHashSet();

            AssignRoleVm vm = new AssignRoleVm(AllRoles, allUsers);

            return vm;
        }

        public async Task AssignRole(string roleId, string userId)
        {
            ApplicationUser user = _userRepository.Get(userId);

            IdentityRole role = _userRepository.GetRole(roleId);
            string CurrentRoleName;

            if (role == null)
            {
                throw new Exception("No roles selected");
            }
            else
            {
                CurrentRoleName = role.Name;

                if (_userRepository.IsInAnyRole(user.Id) == false)
                {
                    await _userRepository.AddUserToRole(user, CurrentRoleName);
                }
                else
                {
                    IdentityUserRole<string> roleUser = _userRepository.GetUsersRole(user.Id);

                    IdentityRole roleToChange = _userRepository.GetRole(roleUser.RoleId);

                    string roleNameToChange = roleToChange.Name;

                    await _userRepository.UpdateUserRole(user, roleNameToChange, CurrentRoleName);
                }
            }
        }

    }
}
