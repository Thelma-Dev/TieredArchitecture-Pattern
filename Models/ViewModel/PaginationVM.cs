using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.Areas.Identity.Pages.Account;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
	public class PaginationVM
	{
		public Project Project { get; set; }


		public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

		public IPagedList<Project> Projects { get; set; }

		public string userId { get; set; }

		public PaginationVM() { }

	}
}
