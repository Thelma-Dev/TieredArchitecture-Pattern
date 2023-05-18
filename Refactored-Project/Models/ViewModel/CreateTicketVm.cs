using Microsoft.AspNetCore.Mvc.Rendering;
using static SD_340_W22SD_Final_Project_Group6.Models.Ticket;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class CreateTicketVm
    {
       public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

        
        [StringLength(200, ErrorMessage = "Title should be 5 to 200 characters only", MinimumLength = 5)]
        [Required]
        [DisplayName("Ticket Name :")]
        public string Title { get; set; }

        [DisplayName("Body :")]
        public string Body { get; set; }


        [Range(1, 999)]
        [DisplayName("Required Hours :")]
        public int RequiredHours { get; set; }


		[DisplayName("Owner :")]
		public string OwnerId { get; set; }


        [ForeignKey("ApplicationUser")]
        [DisplayName("Owner :")]
        public ApplicationUser? Owner { get; set; }


        public int ProjectId { get; set; }

        [DisplayName("Project :")]
        public Project? Project { get; set; }


        [DisplayName("Ticket Priority :")]
        public Priority? TicketPriority { get; set; }

        public void PopulateLists(List<ApplicationUser> users)
        {
            users.ForEach(u =>
            {
                AllDevelopers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
        }


        public CreateTicketVm() { }

        public CreateTicketVm(List<ApplicationUser> users)
        {
            PopulateLists(users);
        }

    }
}
