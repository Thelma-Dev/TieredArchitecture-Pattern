using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
	public class FilterVM
	{
		public Project Project { get; set; }

		public List<Project> Projects { get; set; }

		public List<SelectListItem> AllDevelopers = new List<SelectListItem>();

		public void PopulateLists(List<ApplicationUser> users)
		{
			users.ForEach(u =>
			{
				AllDevelopers.Add(new SelectListItem(u.UserName, u.Id.ToString()));
			});
		}

		public string userId { get; set; }

		public FilterVM() { }

		public FilterVM(List<ApplicationUser> users)
		{
			PopulateLists(users);
		}
	}
}
