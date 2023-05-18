﻿namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectManagersAndDevelopersVm
    {
        public ICollection<ApplicationUser> ProjectManagers { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<ApplicationUser> devs { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<ApplicationUser> allUsers { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<string> roles { get; set; } = new HashSet<string>();

        public ProjectManagersAndDevelopersVm() { }
    }
}
