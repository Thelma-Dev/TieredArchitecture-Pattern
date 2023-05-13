﻿using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IUserProjectRepository
    {
        UserProject GetUserProject(int projectId, string userId);

        void RemoveUserProject(UserProject userProject);

        List<UserProject> GetProjects(int projectId);
    }
}