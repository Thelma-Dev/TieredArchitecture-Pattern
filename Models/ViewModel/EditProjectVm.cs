using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class EditProjectVm
    {
        public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

        [Required]
        [DisplayName("Project Name :")]
        public string ProjectName { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        
        [DisplayName("Assign User :")]
        public string AssignedUserId { get; set; }

        public ApplicationUser? AssignedUser { get; set; }


        public void PopulateLists(List<ApplicationUser> users)
        {
            users.ForEach(u =>
            {
                AllDevelopers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
        }


        public EditProjectVm() { }

        public EditProjectVm(List<ApplicationUser> users)
        {
            PopulateLists(users);
        }
    }
}
