using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TieredArchitectureUnitTest
{
    [TestClass]
    public class ProjectUnitTest
    {
        public ProjectBusinessLogic ProjectBusinessLogic { get; set; }

        IQueryable<Project> data { get; set; }

        [TestInitialize]
        public void Initialize()
        {

            data = new List<Project>
            {
                new Project{},
                new Project{},
                new Project{}
            }.AsQueryable();


            // Create a copy of the Project table

            Mock<DbSet<Project>> mockProjectSet = new Mock<DbSet<Project>>();


            // providing data to  the mock DbSet of project
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockProjectSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            // create a mock of the database context
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();


            // Mocked context should return an object of the mocked Project set
            
            mockContext.Setup(c => c.Projects).Returns(mockProjectSet.Object);


            // mocked objects must have a constructor without parameters
            ProjectBusinessLogic = new ProjectBusinessLogic(new ProjectRepository(mockContext.Object));

        }

        [TestMethod]
        [DataRow(1, 25, 25)]
        
        public void Withdrawal_ArgumentGreaterThanZeroLessThanBalance_FromBalance(int accountId, decimal withdrawalAmount, decimal expectedResult)
        {

            

        }
    }
}
