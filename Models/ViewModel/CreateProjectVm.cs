using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class CreateProjectVm
    {
        public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

        [Required]
        [DisplayName("Project Name :")]
        public string ProjectName { get; set; }


        [DisplayName("Assign User :")]
        public string AssignedUserId { get; set; }

        public ApplicationUser? AssignedUser { get; set; }

        public List<string> ProjectDevelopersId { get; set; } = new List<string>();


        public void PopulateLists(List<ApplicationUser> users)
        {
            users.ForEach(u =>
            {
                AllDevelopers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
        }


        public CreateProjectVm() { }

        public CreateProjectVm(List<ApplicationUser> users)
        {
            PopulateLists(users);
        }
    }
}
