using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class SortProjectVm
    {
		List<SelectListItem> AllDevelopers = new List<SelectListItem>();

		public List<Project> Projects { get; set; } = new List<Project>();

		public SortOrder SortMethod { get; set; }

		public Project Project { get; set; }

		public enum SortOrder
		{
			Priority = 1,

			[Display(Name = "Required Hours")]
			RequiredHours,
			Completed
		}
		public SortProjectVm(List<ApplicationUser> developers, List<Project> projects, SortOrder sortMethod)
		{
			developers.ForEach(au =>
			{
				AllDevelopers.Add(new SelectListItem(au.UserName, au.Id.ToString()));
			});

			SortMethod = sortMethod;
			Projects = projects;
			
		}

		public SortProjectVm() { }
	}
}
