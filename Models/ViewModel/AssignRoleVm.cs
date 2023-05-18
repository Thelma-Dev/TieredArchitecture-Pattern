using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class AssignRoleVm
    {
        public  List<SelectListItem> AllUsers = new List<SelectListItem>();

        public List<SelectListItem> Roles { get; } = new List<SelectListItem>();


        [Display(Name = "Roles")]
        public string RoleId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        public string? ViewMessage { get; set; }

        public void PopulateLists(HashSet<IdentityRole> role, List<ApplicationUser> users)
        {

            foreach (IdentityRole r in role)
            {
                Roles.Add(new SelectListItem(r.Name, r.Id));
            }

            users.ForEach(u =>
            {
                AllUsers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
        }


        public AssignRoleVm() { }

        public AssignRoleVm(HashSet<IdentityRole> roles, List<ApplicationUser> users)
        {
            PopulateLists(roles, users);
        }
    }
}
