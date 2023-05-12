using Microsoft.AspNetCore.Identity;
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

        public AdminBusinessLogic(UserManager<ApplicationUser> userManager, IUserRepository userRepository)
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

        public async void AssignRole(string roleId, string userId)
        {
            ApplicationUser user = _userRepository.Get(userId);

            IdentityRole role = _userRepository.GetRole(roleId);

            string roleName = role.Name;

            ICollection<string> roleUser = await _userManager.GetRolesAsync(user);

            

            if (roleUser.Count == 0)
            {
                await _userManager.AddToRoleAsync(user, roleName);

                _userRepository.SaveChangesAsync();

            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, roleUser.First());

                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

    }
}
