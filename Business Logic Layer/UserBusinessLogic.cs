using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Data;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class UserBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _users;
        private IRepository<ApplicationUser> _userRepository;

        public UserBusinessLogic(UserManager<ApplicationUser> users, IRepository<ApplicationUser> userRepository)
        {
            _users = users;
            _userRepository = userRepository;
        }

        public async void GetUserInRole(string roleName)
        {
            List<ApplicationUser> UsersInRole = (List<ApplicationUser>)await _users.GetUsersInRoleAsync(roleName);
        }
        public async List<string> GetRoleOfUser(ApplicationUser user)
        {
          return  await _users.GetRolesAsync(user).to;
        }
        public async void AssignRole(string roleName, string userId)
        {
            ApplicationUser user = _userRepository.Get(userId);


            ICollection<string> roleUser = await _users.GetRolesAsync(user);


            if (roleUser.Count == 0)
            {
                await _users.AddToRoleAsync(user, role);

                return RedirectToAction("Index", "Admin", new { area = "" });

            }
            else
            {

                await _users.RemoveFromRoleAsync(user, roleUser.First());
                await _users.AddToRoleAsync(user, role);


                return RedirectToAction("Index", "Admin", new { area = "" });
            }
        }
    }
}
