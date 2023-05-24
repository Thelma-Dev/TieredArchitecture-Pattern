﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace TieredArchitectureUnitTest
{
    [TestClass]
    public class ProjectUnitTest
    {
        public ProjectBusinessLogic ProjectBusinessLogic { get; set; }

        List<Project> projectData { get; set; }
        List<Ticket> ticketData { get; set;}
        List<UserProject> userProjectData { get; set;}
        List<ApplicationUser> applicationUserData { get; set; }
        List<CreateProjectVm> createProjectVmData { get; set; }
        List<EditProjectVm> editProjectVmData { get; set;}
        List<TicketWatcher> ticketWatcherData { get; set; }

        List<PaginationVM> paginationVMData { get; set; }

        List<IdentityRole> roleData { get; set; }

        List<IdentityUserRole<string>> identityUserRoleData { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            projectData = new List<Project>
            {
                new Project{Id = 1,ProjectName = "Zion Project 1"},
                new Project{Id = 2, ProjectName = "Alpha Project 2"},
                new Project{Id = 3, ProjectName = "Butter Project 3"}
            }.ToList();

            ticketData = new List<Ticket>
            {
                new Ticket{Id = 1, Project =projectData.First(), RequiredHours=8, TicketPriority=Ticket.Priority.High, Completed=true, Owner=applicationUserData.Last()},
                new Ticket{Id = 2, Project = projectData.First(), RequiredHours = 20, TicketPriority=Ticket.Priority.Medium, Completed= false, Owner=applicationUserData.First()},
                new Ticket{Id = 3,Project = projectData.Last(), RequiredHours = 12, TicketPriority = Ticket.Priority.Low, Completed = false, Owner = applicationUserData.First()}

            }.ToList();

            userProjectData = new List<UserProject>
            {
                new UserProject{Id = 1, ProjectId = 2, UserId = "1"},
                new UserProject{Id = 2, ProjectId = 2, UserId = "2"},
                new UserProject{Id = 3, ProjectId = 3, UserId = "3"}
            }.ToList();

            applicationUserData = new List<ApplicationUser>
            {
                new ApplicationUser{Id="1" ,UserName = "john34@gmail.com", PasswordHash="John@34"},
                new ApplicationUser{Id="2", UserName = "brenda21@gmail.com", PasswordHash="brenda@21"},
                new ApplicationUser{Id="3", UserName = "manager14@gmail.com", PasswordHash= "manager@14"},
                new ApplicationUser{Id="4", UserName = "amanda12@gmail.com", PasswordHash="amanda@12"},

            }.ToList();

            createProjectVmData = new List<CreateProjectVm>
            {
                new CreateProjectVm{ProjectName = "ProjectName", ProjectDevelopersId = {"1","2"}, LoggedInUsername = "manager14@gmail.com"}

            }.ToList();

            editProjectVmData = new List<EditProjectVm>
            {
                new EditProjectVm{ProjectName = "Edited Project Name", ProjectId = 3, ProjectDevelopersId= {"1","2"}}

            }.ToList();

            ticketWatcherData = new List<TicketWatcher>
            {
                new TicketWatcher {TicketId = 1, WatcherId = "2"}

            }.ToList();

            paginationVMData = new List<PaginationVM>
            {
                new PaginationVM{Projects= projectData.ToPagedList(pageNumber: 1, pageSize: 3)}
            }.ToList();


            roleData = new List<IdentityRole>()
            {
                new IdentityRole{Id="1", Name = "ProjectManager" },
                new IdentityRole{Id ="2", Name = "Developer" },
                new IdentityRole{Id = "3", Name = "Admin" }

            }.ToList();

            identityUserRoleData = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>{RoleId = "1", UserId= "3"},
                new IdentityUserRole<string>{RoleId = "2", UserId="2"},
                new IdentityUserRole<string>{RoleId = "2", UserId = "1"}
            }.ToList();


            Mock<RoleManager<IdentityRole>> mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);
            


            // Create a copy of the Project,Ticket,and UserProject, and ApplicationUser table
            Mock<DbSet<Project>> mockProjectSet = new Mock<DbSet<Project>>();
            Mock<DbSet<Ticket>> mockTicketSet = new Mock<DbSet<Ticket>>();
            Mock<DbSet<UserProject>> mockUserProjectSet = new Mock<DbSet<UserProject>>();
            Mock<DbSet<ApplicationUser>> mockApplicationUserSet = new Mock<DbSet<ApplicationUser>>();
            Mock<DbSet<TicketWatcher>> mockTicketWatcherSet = new Mock<DbSet<TicketWatcher>>();
            Mock<DbSet<IdentityRole>> mockIdentityRoleSet = new Mock<DbSet<IdentityRole>>();
            Mock<DbSet<IdentityUserRole<string>>> mockIdentityUserRoleSet = new Mock<DbSet<IdentityUserRole<string>>>();


            //Mock user manager
            Mock store = new Mock<IUserStore<ApplicationUser>>();
            Mock<UserManager<ApplicationUser>> manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            manager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
            

            manager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            manager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => applicationUserData.Add(x));
            manager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);


            
            // providing data to the mock DbSets
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectData.AsQueryable().Provider);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectData.AsQueryable().Expression);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectData.AsQueryable().ElementType);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectData.GetEnumerator());

            mockApplicationUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(applicationUserData.AsQueryable().Provider);
            mockApplicationUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(applicationUserData.AsQueryable().Expression);
            mockApplicationUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(applicationUserData.AsQueryable().ElementType);
            mockApplicationUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(applicationUserData.GetEnumerator());


            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(ticketData.AsQueryable().Provider);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(ticketData.AsQueryable().Expression);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(ticketData.AsQueryable().ElementType);
            mockTicketSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(ticketData.GetEnumerator());


            mockUserProjectSet.As<IQueryable<UserProject>>().Setup(m => m.Provider).Returns(userProjectData.AsQueryable().Provider);
            mockUserProjectSet.As<IQueryable<UserProject>>().Setup(m => m.Expression).Returns(userProjectData.AsQueryable().Expression);
            mockUserProjectSet.As<IQueryable<UserProject>>().Setup(m => m.ElementType).Returns(userProjectData.AsQueryable().ElementType);
            mockUserProjectSet.As<IQueryable<UserProject>>().Setup(m => m.GetEnumerator()).Returns(userProjectData.GetEnumerator());


            mockTicketWatcherSet.As<IQueryable<TicketWatcher>>().Setup(m => m.Provider).Returns(ticketWatcherData.AsQueryable().Provider);
            mockTicketWatcherSet.As<IQueryable<TicketWatcher>>().Setup(m => m.Expression).Returns(ticketWatcherData.AsQueryable().Expression);
            mockTicketWatcherSet.As<IQueryable<TicketWatcher>>().Setup(m => m.ElementType).Returns(ticketWatcherData.AsQueryable().ElementType);
            mockTicketWatcherSet.As<IQueryable<TicketWatcher>>().Setup(m => m.GetEnumerator()).Returns(ticketWatcherData.GetEnumerator());


            mockIdentityRoleSet.As<IQueryable<IdentityRole>>().Setup(m => m.Provider).Returns(roleData.AsQueryable().Provider);
            mockIdentityRoleSet.As<IQueryable<IdentityRole>>().Setup(m => m.Expression).Returns(roleData.AsQueryable().Expression);
            mockIdentityRoleSet.As<IQueryable<IdentityRole>>().Setup(m => m.ElementType).Returns(roleData.AsQueryable().ElementType);
            mockIdentityRoleSet.As<IQueryable<IdentityRole>>().Setup(r => r.GetEnumerator()).Returns(roleData.GetEnumerator());

            mockIdentityUserRoleSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Provider).Returns(identityUserRoleData.AsQueryable().Provider);
            mockIdentityUserRoleSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Expression).Returns(identityUserRoleData.AsQueryable().Expression);
            mockIdentityUserRoleSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.ElementType).Returns(identityUserRoleData.AsQueryable().ElementType);
            mockIdentityUserRoleSet.As<IQueryable<IdentityUserRole<string>>>().Setup(r => r.GetEnumerator()).Returns(identityUserRoleData.GetEnumerator());



            // create a mock of the database context
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();


            // Mocked context should return an object of the mocked sets
            mockContext.Setup(c => c.Projects).Returns(mockProjectSet.Object);
            mockContext.Setup(c => c.Tickets).Returns(mockTicketSet.Object);
            mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockApplicationUserSet.Object);
            mockContext.Setup(c => c.TicketWatchers).Returns(mockTicketWatcherSet.Object);
            mockContext.Setup(c => c.DeleteProject(It.IsAny<Project>())).Callback<Project>(p => projectData.Remove(p));
            mockContext.Setup(c => c.RemoveUserProject(It.IsAny<UserProject>())).Callback<UserProject>(up => userProjectData.Remove(up));
            mockContext.Setup(c => c.CreateProject(It.IsAny<Project>())).Callback<Project> (p => projectData.Add(p));
            mockContext.Setup(c => c.Roles).Returns(mockIdentityRoleSet.Object);
            mockContext.Setup(c => c.UserRoles).Returns(mockIdentityUserRoleSet.Object);


            
            ProjectBusinessLogic = new ProjectBusinessLogic(new ProjectRepository(mockContext.Object), new TicketRepository(mockContext.Object), new UserProjectRepository(mockContext.Object), new UserRepository(mockContext.Object, manager.Object),new TicketWatchersRepository(mockContext.Object));

        }


        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void GetProject_OnNoFoundId_ThrowsAnIvalidOperationException(int projectId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => ProjectBusinessLogic.GetProject(projectId));
        }

        [TestMethod]
        public void GetProject_OnNoArgument_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ProjectBusinessLogic.GetProject(null));
        }

        [TestMethod]
        [DataRow(1)]
        public void GetProjectDetails_WithArgumentAndFoundId_ReturnsExpectedProjectObject(int projectId)
        {
            Project ActualProject = projectData.First(p => p.Id == projectId);

            Assert.IsTrue(ActualProject.Equals(ProjectBusinessLogic.GetProjectDetails(projectId)));
        }

        [TestMethod]
        [DataRow(3)]
        public void DeleteProject_WithArgumentAndFoundId_ReturnsExpectedProjectToBeDeleted(int projectId)
        {
            Project ActualProject = projectData.First(p => p.Id == projectId);

            Assert.IsTrue(ActualProject.Equals(ProjectBusinessLogic.DeleteProject(projectId)));
        }

        [TestMethod]
        public async Task CreateProject_WithCreateProjectVmHavingProjectNameLoggedInUserNameAndListOfDevelopers_CreatesAProject()
        {
            // Act
            await ProjectBusinessLogic.CreateProject(createProjectVmData.First());

            // Assert
            Assert.IsTrue(projectData.Count == 4);
        }


        [TestMethod]
        [DataRow(1)]
        public void AddUserToProject_WithAProjectAndListOfDevelopers_CreatesAUserProjectForEachDveloper(int ProjectId)
        {
            Project project = projectData.First(p => p.Id == ProjectId);

            ProjectBusinessLogic.AddUserToProject(project, applicationUserData.ToList());

            Assert.IsTrue(project.AssignedTo.Count == applicationUserData.Count);
        }


        [TestMethod]
        public void GetUserToBeRemovedFromProject_OnNoArgument_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ProjectBusinessLogic.GetUserToBeRemovedFromProject(null));
        }


        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void GetUserToBeRemovedFromProject_OnNoFoundUserId_ThrowsAnInvalidOperationException(int userId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => ProjectBusinessLogic.GetUserToBeRemovedFromProject(userId.ToString()));
        }


        [TestMethod]
        [DataRow(1, 2)]
        public void RemoveAssignedUser_WithArgumentFoundUserIdAndProjectId_RemovesTheUserFromTheUserProjectTable(int userId, int projectId)
        {
            ProjectBusinessLogic.RemoveAssignedUser(userId.ToString(), projectId);

            Assert.IsTrue(userProjectData.Count == 2);
        }


        [TestMethod]
        public void UpdateEditedProject_WithEditProjectVmHavingProjectNameAndListOfDevelopers_UpdatesExistingProject()
        {
            // Act
            ProjectBusinessLogic.UpdateEditedProject(editProjectVmData.First());

            Project project = projectData.First(p => p.Id == editProjectVmData.First().ProjectId);

            // Assert
            Assert.IsTrue(editProjectVmData.First().Project.Equals(project));
        }


        
        [TestMethod]
        [DataRow(3)]
        public void DeleteProjectConfirmed_WithArgumentAndFoundId_DeletesTheProject(int projectId)
        {
            Project ActualProject = projectData.First(p => p.Id == projectId);

            // Act
            ProjectBusinessLogic.DeleteProjectConfirmed(projectId);
           
            
            // Assert
            Assert.AreEqual(projectData.Count(), 2);
            
            Assert.ThrowsException<InvalidOperationException>(() => ProjectBusinessLogic.GetProject(ActualProject.Id));
        }


        [TestMethod]
        [DataRow(null,1,null,null,3 )]
        public void Read_WithArgument_ReturnsAPaginationVmWithListOfDevelopersAndProjects(string? sortOrder, int? page, bool? sort, string? userId, int loggedInUserId)
        {
            List<Project> ActualVmProjects = paginationVMData.First().Projects.OrderBy(p => p.ProjectName).ToList();


            // Act
            ApplicationUser user = applicationUserData.First(u => u.Id == loggedInUserId.ToString());
                    
            PaginationVM vm = ProjectBusinessLogic.Read(sortOrder, page, sort, userId, user.UserName);


            // Assert
            Assert.IsTrue(vm.Projects.SequenceEqual(ActualVmProjects));
        }


        /*
        [TestMethod]
        [DataRow(null, 1, null, null, 3)]
        public void Read_WithArgument_ReturnsAPaginationVmWithListOfDevelopersAndProjects(string? sortOrder, int? page, bool? sort, string? userId, int loggedInUserId)
        {
            PaginationVM vm = paginationVMData.First();

            ApplicationUser user = applicationUserData.First(u => u.Id == loggedInUserId.ToString());


            ProjectBusinessLogic.Read(sortOrder, page, sort, userId, user.UserName);

            //List<Ticket> ticket = new List<Ticket>();

            //foreach(Project p in vm.Projects)
            {
                //ticket = p.Tickets.OrderByDescending(p => p.TicketPriority).ToList();
            }

            Assert.IsTrue(vm.Projects.Any());
        }
        */

    }
}
