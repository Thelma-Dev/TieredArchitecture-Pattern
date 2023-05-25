using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using Project = SD_340_W22SD_Final_Project_Group6.Models.Project;

namespace TieredArchitectureUnitTest
{
    [TestClass]
    public class TicketUnitTest
    {
        public TicketBusinessLogic TicketBusinessLogic { get; set; }
        List<Project> projectData { get; set; }
        List<Ticket> ticketData { get; set; }

        private List<Comment> commentData;
        List<UserProject> userProjectData { get; set; }
        List<ApplicationUser> applicationUserData { get; set; }
        List<CreateTicketVm> createTicketVmData { get; set; }
        List<EditTicketVm> editTicketVmData { get; set; }
        List<TicketWatcher> ticketWatcherData { get; set; }
        List<PaginationVM> paginationVMData { get; set; }
        List<IdentityRole> roleData { get; set; }
        List<IdentityUserRole<string>> identityUserRoleData { get; set; }

        int initialCount = 0;

        [TestInitialize]
        public void Initialize()
        {
            applicationUserData = new List<ApplicationUser>
            {
                new ApplicationUser{Id="1" ,UserName = "john34@gmail.com", PasswordHash="John@34"},
                new ApplicationUser{Id="2", UserName = "brenda21@gmail.com", PasswordHash="brenda@21"},
                new ApplicationUser{Id="3", UserName = "manager14@gmail.com", PasswordHash= "manager@14"},
                new ApplicationUser{Id="4", UserName = "amanda12@gmail.com", PasswordHash="amanda@12"},

            }.ToList();

            userProjectData = new List<UserProject>
            {
                new UserProject{Id = 1, ProjectId = 1, UserId = "1", User = applicationUserData.First()},
                new UserProject{Id = 2, ProjectId = 2, UserId = "2"},
                new UserProject{Id = 3, ProjectId = 3, UserId = "3"}
            }.ToList();

            projectData = new List<Project>
            {
                new Project{Id = 1,ProjectName = "Zion Project 1", AssignedTo = {userProjectData.First()} },
                new Project{Id = 2, ProjectName = "Alpha Project 2"},
                new Project{Id = 3, ProjectName = "Butter Project 3"}
            }.ToList();

            applicationUserData = new List<ApplicationUser>
            {
                new ApplicationUser{Id="1" ,UserName = "john34@gmail.com", PasswordHash="John@34"},
                new ApplicationUser{Id="2", UserName = "brenda21@gmail.com", PasswordHash="brenda@21"},
                new ApplicationUser{Id="3", UserName = "manager14@gmail.com", PasswordHash= "manager@14"},
                new ApplicationUser{Id="4", UserName = "amanda12@gmail.com", PasswordHash="amanda@12"},

            }.ToList();


            ticketData = new List<Ticket>
            {

                new Ticket{Id = 1, Owner=applicationUserData.First(),RequiredHours=8, TicketPriority=Ticket.Priority.High, Completed=true},
                new Ticket{Id = 1,RequiredHours=8, TicketPriority=Ticket.Priority.High, Completed=true, Project = projectData.First()},

                new Ticket{Id = 2, RequiredHours = 20, TicketPriority=Ticket.Priority.Medium, Completed= false},
                new Ticket{Id = 3, RequiredHours = 12, TicketPriority = Ticket.Priority.Low, Completed = false}

            }.ToList();


            commentData = new List<Comment>
            {
                new Comment {Id = 1,Description = "Good Comment",UserId = "1", TicketId = 1 },
                new Comment {Id = 2,Description = "Good Comment",UserId = "1", TicketId = 2 }

            }.ToList();

            userProjectData = new List<UserProject>
            {
                new UserProject{Id = 1, ProjectId = 2, UserId = "1"},
                new UserProject{Id = 2, ProjectId = 2, UserId = "2"},
                new UserProject{Id = 3, ProjectId = 3, UserId = "3"}
            }.ToList();
           
           

            createTicketVmData = new List<CreateTicketVm>
            {
                new CreateTicketVm{Title = "TicketTitle", Body = "Ticket1", OwnerId = "1", ProjectId = 1, RequiredHours = 24 },
                new CreateTicketVm{Title = "TicketTitle", Body = "Ticket2", ProjectId = 2, RequiredHours = 27, Owner = applicationUserData.First(), OwnerId = "1"},
                new CreateTicketVm{Title = "TicketTitle", Body = "Ticket3", ProjectId = 3, RequiredHours = 24 },
                new CreateTicketVm{Title = "TicketTitle", Body = "Ticket3", RequiredHours = 24 },


            }.ToList();

            editTicketVmData = new List<EditTicketVm>

            {
                //new EditProjectVm{ProjectName = "Edited Project Name", ProjectId = 3, ProjectDevelopersId= {"1","2"}}

                new EditTicketVm{TicketId = 1, Title = "UpdateTicket" ,Body = "UpdateTicket1", OwnerId = "1", RequiredHours = 55},
                new EditTicketVm{Title = "UpdateTicket2" ,Body = "UpdateTicket2", OwnerId = "1", RequiredHours = 55}


            }.ToList();

            ticketWatcherData = new List<TicketWatcher>
            {
                new TicketWatcher {TicketId = 1, WatcherId = "1"}

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


            // Creating tickets for projects
            projectData.First().Tickets.Add(ticketData.First(t => t.Id == 1));
            projectData.Last().Tickets.Add(ticketData.First(t => t.Id == 3));

            // Adding comments to tickets
            ticketData.First().Comments.Add(commentData.First(c => c.Id == 1));
            ticketData.First().Comments.Add(commentData.First(c => c.Id == 2));





            // Create a copy of the Project,Ticket,and UserProject, and ApplicationUser table
            Mock<DbSet<Project>> mockProjectSet = new Mock<DbSet<Project>>();
            Mock<DbSet<Ticket>> mockTicketSet = new Mock<DbSet<Ticket>>();
            Mock<DbSet<Comment>> mockCommentSet = new Mock<DbSet<Comment>>();
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

            mockCommentSet.As<IQueryable<Comment>>().Setup(c => c.Provider).Returns(commentData.AsQueryable().Provider);
            mockCommentSet.As<IQueryable<Comment>>().Setup(c => c.Expression).Returns(commentData.AsQueryable().Expression);
            mockCommentSet.As<IQueryable<Comment>>().Setup(c => c.ElementType).Returns(commentData.AsQueryable().ElementType);
            mockCommentSet.As<IQueryable<Comment>>().Setup(c => c.GetEnumerator()).Returns(commentData.GetEnumerator());

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
            mockContext.Setup(c => c.Comments).Returns(mockCommentSet.Object);
            mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockApplicationUserSet.Object);
            mockContext.Setup(c => c.TicketWatchers).Returns(mockTicketWatcherSet.Object);
            mockContext.Setup(c => c.DeleteProject(It.IsAny<Project>())).Callback<Project>(p => projectData.Remove(p));
            mockContext.Setup(c => c.DeleteTicket(It.IsAny<Ticket>())).Callback<Ticket>(t => ticketData.Remove(t));
            mockContext.Setup(c => c.DeleteTicketWatcher(It.IsAny<TicketWatcher>())).Callback<TicketWatcher>(tw => ticketWatcherData.Remove(tw));
            mockContext.Setup(c => c.RemoveUserProject(It.IsAny<UserProject>())).Callback<UserProject>(up => userProjectData.Remove(up));
            mockContext.Setup(c => c.CreateProject(It.IsAny<Project>())).Callback<Project>(p => projectData.Add(p));

            mockContext.Setup(c => c.CreateTicketWatcher(It.IsAny<TicketWatcher>())).Callback<TicketWatcher>(tw => ticketWatcherData.Add(tw));
            mockContext.Setup(c => c.CreateComment(It.IsAny<Comment>())).Callback<Comment>(c => commentData.Add(c));


            mockContext.Setup(c => c.CreateTicket(It.IsAny<Ticket>())).Callback<Ticket>(t => ticketData.Add(t));
            mockContext.Setup(c => c.CreateTicketWatcher(It.IsAny<TicketWatcher>())).Callback<TicketWatcher>(tw => ticketWatcherData.Add(tw));

            mockContext.Setup(c => c.Roles).Returns(mockIdentityRoleSet.Object);
            mockContext.Setup(c => c.UserRoles).Returns(mockIdentityUserRoleSet.Object);

            TicketBusinessLogic = new TicketBusinessLogic(new TicketRepository(mockContext.Object), new UserProjectRepository(mockContext.Object), new UserRepository(mockContext.Object, manager.Object), new TicketRepository(mockContext.Object), new ProjectRepository(mockContext.Object), new CommentRepository(mockContext.Object), new TicketWatchersRepository(mockContext.Object));




        }

      
        [TestMethod]
        public void GetTicket_WithArgumentAndFoundId_ReturnsTicketWithAnIdOfArgument()
        {
            Ticket actualTicket = ticketData.First();
            Ticket queriedTicket = TicketBusinessLogic.GetTicket(actualTicket.Id);
            Assert.AreEqual(actualTicket, queriedTicket);
        }

        [TestMethod]

        public void GetTicket_OnNoArgument_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TicketBusinessLogic.GetTicket(null));
        }

        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void GetTicket_OnNoFoundId_ThrowsAnInvalidOperationException(int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.GetTicket(ticketId));
        }
        

        [TestMethod]
        [DataRow(2)]
        public void GetTicketDetails_WithFoundIdArgument_ReturnsExpectedTicket(int ticketId)
        {
           
            Ticket actualTicket = ticketData.First(x => x.Id == ticketId);

            Assert.IsTrue(actualTicket.Equals(TicketBusinessLogic.GetTicketDetails(ticketId)));
        }

        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void GetTicketDetails_WithNoFoundIdArgument_ThrowsInvalidException(int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.GetTicketDetails(ticketId));
            
        }


        [TestMethod]
        [DataRow(1, 1)]
        public void RemoveAssignedUser_WithFoundUserIdAndTicketId_MakeTheTicketOwnerNull(int userId, int ticketId)
        {
            Ticket ticket = ticketData.First(x => x.Id == ticketId);

            TicketBusinessLogic.RemoveAssignedUser(userId.ToString(), ticketId);

            Assert.IsTrue(ticket.Owner == null);
        }

        [TestMethod]
        [DataRow(Int32.MaxValue, 1)]
        public void RemoveAssignedUser_WithNoUserIdFound_ThrowsNullException(int userId, int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.RemoveAssignedUser(userId.ToString(), ticketId));
        }

        [TestMethod]
        [DataRow(Int32.MinValue, "john34@gmail.com")]
        public void AddToWatch_WithNoFoundTicketId_ThrowsInvalidOperationException(int ticketId, string userName)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.AddToWatch(ticketId, userName));
        }

        [TestMethod]
        [DataRow(1, "john34@gmail.com")]

        public void AddToWatch_WithFoundTicketIdAndUserName_AddTicketToWatchedTicket(int ticketId, string userName)
        {
            initialCount = ticketWatcherData.Count();
            TicketBusinessLogic.AddToWatch(ticketId, userName);
            Assert.AreEqual(ticketWatcherData.Count(),initialCount + 1) ;
        }
        [TestMethod]
        [DataRow(1, 29, 29)]

        public void UpdateRequiredHours_WithArgumentAndFoundId_SetsTheTicketRequiredHours(int ticketId, int hours, int expectedHours)
        {
            Ticket acctualTicket = ticketData.First(t => t.Id == ticketId);

            TicketBusinessLogic.UpdateRequiredHours(ticketId, hours);

            Assert.AreEqual(expectedHours, acctualTicket.RequiredHours);
        }
        [TestMethod]
        [DataRow(1, 1001)]
        public void UpdateRequiredHours_ArgumentExceedsRequiredHoursLimits_ThrowsInvalidOperation(int ticketId, int hours)
        {
            Ticket acctualTicket = ticketData.First(t => t.Id == ticketId);

            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.UpdateRequiredHours(ticketId, hours));
        }


        [TestMethod]
        [DataRow(3)]
        public void DeleteTicket_WithArgumentAndFoundId_ReturnsExpectedTicketToBeDeleted(int ticketId)
        {
            Ticket ActualProject = ticketData.First(p => p.Id == ticketId);

            Assert.IsTrue(ActualProject.Equals(TicketBusinessLogic.DeleteTicket(ticketId)));
        }

        [TestMethod]
        [DataRow(5)]
        public void DeleteTicket_WithNoFoundId_ThrowsInvalidOperationException(int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.DeleteTicket(ticketId));
        }

        [TestMethod]
        [DataRow(1, 1)]
        public void DeleteTicketConfirmed_WithArgumentAndFoundId_DeletesTheTicket(int ticketId, int projectId)
        {
            initialCount = ticketData.Count;
            Ticket ActualTicket = ticketData.First(p => p.Id == ticketId);
            // Act
            TicketBusinessLogic.TicketDeleteConfirmed(ticketId, projectId);


            // Assert
            Assert.AreEqual(ticketData.Count(), initialCount - 1);

        }


        [TestMethod]
        [DataRow(4, 1)]
        public void DeleteTicketConfirmed_WithNoFoundId_ThrowsInvalidOperationException(int ticketId, int projectId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.TicketDeleteConfirmed(ticketId, projectId));

        }

        [TestMethod]
        public void CreateTicket_WithCreateTicketVmHavingTicketNameLoggedInUserNameAndListOfDevelopers_CreatesATicket()
        {
            initialCount = ticketData.Count;
            // Act
            TicketBusinessLogic.CreateTicket(createTicketVmData.FirstOrDefault(vm => vm.ProjectId == 2));

            // Assert
            Assert.AreEqual(ticketData.Count, initialCount + 1);
        }

        [TestMethod]
        [DataRow(3)]
        public void CreateTicket_WithNoFounfProject_ThrowsInvalidOperationException(int projectId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.CreateTicket(createTicketVmData.FirstOrDefault(vm => vm.ProjectId == projectId)));

        }


        [TestMethod]

        [DataRow(1)]
        public void MarkAsCompleted_WithFoundTicketId_ReturnsTrue(int ticketId)
        {
            Ticket ticket = ticketData.First( t => t.Id == ticketId);
            TicketBusinessLogic.MarkAsCompleted(ticket.Id);
            Assert.IsTrue(ticket.Completed);

        }

        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void MarkAsCompleted_OnNoArgument_ThrowsNullException(int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.MarkAsCompleted(ticketId));
        }

        [TestMethod]
        [DataRow(1)]
        public void UnMarkAsCompleted_WithFoundTicketId_ReturnsFalse(int ticketId)
        {
            Ticket ticket = ticketData.First(t => t.Id == ticketId);
            TicketBusinessLogic.UnMarkAsCompleted(ticket.Id);
            Assert.IsFalse(ticket.Completed);

        }

        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void UnMarkAsCompleted_WithNoFoundTicket_ThrowsInvalidException(int ticketId)
        {
  
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.UnMarkAsCompleted(ticketId));
        }

        [TestMethod]
        public void Read()
        {
            int count = ticketData.Count();
            TicketBusinessLogic.Read();
            Assert.AreEqual(count, ticketData.Count());
        }
      
        [TestMethod]
        [DataRow(Int32.MaxValue)]
        public void EditTicket_WithNoFoundId_EditsTheGivenTicket(int ticketId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.EditTicket(ticketId));
        }

        [TestMethod]
        [DataRow(1)]
        public void EditTicket_WithFoundId_ThrowsAnInvalidOperationException(int ticketId)
        {
            Assert.IsInstanceOfType(TicketBusinessLogic.EditTicket(ticketId), typeof(EditTicketVm));
        }


        [TestMethod]
        public void UpdateEditedTicket_WithEditTicketViewModelHavingTicketNameAndListOfDevelopers_UpdatesExistingTicket()
        {
            // Act
            TicketBusinessLogic.UpdateEditedTicket(editTicketVmData.First());
            Ticket ticket = ticketData.First(t => t.Id == editTicketVmData.First().TicketId);
            // Assert
            Assert.IsTrue(editTicketVmData.First().TicketId.Equals(ticket.Id));
        }
        [TestMethod]
        public void UpdateEditedTicket_WithNoFoundTicket_ThrowsInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.UpdateEditedTicket(editTicketVmData.Last()));
        }

        [TestMethod]
        public void RepopulateDevelopersInProjectList_WithHavingCreateTicketVM_ReturnsDevelopersOfProject()
        {
            int count = createTicketVmData.First().AllDevelopers.Count();
            TicketBusinessLogic.RepopulateDevelopersInProjectList(createTicketVmData.First());
            Assert.AreEqual(createTicketVmData.First().AllDevelopers.Count(), count + 1);
        }

        [TestMethod]
        public void RepopulateDevelopersInProjectList_WithNoFoundArgument_ThrowsInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.RepopulateDevelopersInProjectList(createTicketVmData.Last()));
        }

        [TestMethod]
        [DataRow(1, "john34@gmail.com")]
        public void UnWatch_WithFoundTicketIdAndUsername_UnWatchTheGivenTicket(int id, string userName)
        {
            initialCount = ticketWatcherData.Count();
            TicketBusinessLogic.Unwatch(id, userName);
            Assert.AreEqual(ticketWatcherData.Count, initialCount - 1);


        }

        [TestMethod]
        [DataRow(6, "john34@gmail.com")]
        public void UnWatch_WithNoFoundTicket_ThrowsInvalidOperationxception(int id, string userName)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.Unwatch(id, userName));
        }

        [TestMethod]

        [DataRow(1, "Comment", "john34@gmail.com")]
        public void CommentOnTask_WithFoundAllRequiredIds_AddCommentsOnTask(int taskId,string taskText, string userName)
        {
            initialCount = commentData.Count();
            TicketBusinessLogic.CommentOnTask(taskId, taskText, userName);
            Assert.AreEqual(commentData.Count(), initialCount + 1);

        }

        [TestMethod]
        [DataRow(null, null, "john34@gmail.com")]
        public void CommentOnTask_WithNoFoundTaskIdAndText_ThrowsInvalidOperationxception(int taskId, string taskText, string userName)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.CommentOnTask(taskId, taskText, userName));
        }
    
    
       

        [TestMethod]
        [DataRow(1)]
        public void InitializeCreateTicketMethod_WithFoundProjectId_ReturnsACreateTickettViewModelType(int projectId)
        {
            Assert.IsInstanceOfType(TicketBusinessLogic.InitializeCreateTicketMethod(projectId), typeof(CreateTicketVm));
        }

        [TestMethod]
        [DataRow(6)]
        public void InitializeCreateTicketMethod_WithNotFoundProjectId_ThrowsInvalidOperationxception(int projectId)
        {
            Assert.ThrowsException<InvalidOperationException>(() => TicketBusinessLogic.InitializeCreateTicketMethod(projectId));
        }

    }



}