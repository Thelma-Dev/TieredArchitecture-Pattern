using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace TieredArchitectureUnitTest
{
    [TestClass]
    public class TicketUnitTest
    {
        public TicketBusinessLogic TicketBusinessLogic { get; set; }
        public IQueryable<Ticket> data { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Mock<DbSet<Ticket>> mockTicketDbSet = new Mock<DbSet<Ticket>>();

            data = new List<Ticket>
            {
                new Ticket{ Id = 1, Body = "Body of Ticket One", Completed = false, RequiredHours = 8, Title = "TicketOne"},

                new Ticket{ Id = 2, Body = "Body of Ticket Two", Completed = true, RequiredHours = 19, Title = "TicketTwo"}

            }.AsQueryable();

            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(data.Provider);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(data.Expression);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            mockContext.Setup(c => c.Tickets).Returns(mockTicketDbSet.Object);

            TicketBusinessLogic = new TicketBusinessLogic(new TicketRepository(mockContext.Object));

        }


    }   
   
}