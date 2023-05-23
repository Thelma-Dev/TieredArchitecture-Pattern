using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ProjectRepository : IRepository<Project>
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Project entity)
        {
            _context.CreateProject(entity);
            _context.SaveChanges();

        }

        public void Delete(Project entity)
        {
            _context.DeleteProject(entity);
            _context.SaveChanges();
        }

        public Project? Get(int? id)
        {
            return _context.Projects.First(p => p.Id == id);
        }


        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToHashSet();
        }

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public void Update(Project entity)
        {
            _context.Projects.Update(entity);
            _context.SaveChanges();
        }

    }
}
