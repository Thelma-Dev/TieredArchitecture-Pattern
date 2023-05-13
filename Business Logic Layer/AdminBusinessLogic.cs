using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Collections;
using System.Data;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class AdminBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IUserRepository _userRepository;
        private ApplicationDbContext _context;
        
        public AdminBusinessLogic(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public ICollection<ApplicationUser> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public ApplicationUser GetUser(string userId)
        {
            return _userRepository.Get(userId);
        }

        public void AssignRole(string roleId, string userId)
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
                    _userManager.AddToRoleAsync(user, CurrentRoleName);
                }
                else
                {
                    IdentityUserRole<string> roleUser = _userRepository.GetUsersRole(user.Id);

                    IdentityRole roleToChange = _userRepository.GetRole(roleUser.RoleId);

                    string roleNameToChange = roleToChange.Name;

                    _userRepository.UpdateUserRole(user, roleNameToChange, CurrentRoleName);
                }
            }
        }

    }
}
