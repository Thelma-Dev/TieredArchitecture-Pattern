using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.Areas.Identity.Pages.Account;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
	public class FilterVM
	{
		public Project Project { get; set; }

		public List<Project> Projects { get; set; }

		public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

		public IPagedList<Project> Logs { get; set; }


		//public void PopulateLists(List<ApplicationUser> users)
		//{
		//	users.ForEach(u =>
		//	{
		//		AllDevelopers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
		//	});
		//}

		public string userId { get; set; }

		public FilterVM() { }

		//public FilterVM(List<ApplicationUser> users)
		//{
		//	PopulateLists(users);
		//}
	}
}
