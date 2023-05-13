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
            _context.Add(entity);
            _context.SaveChanges();

        }

        public void Delete(Project entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
        }

        public Project? Get(int? id)
        {
            return _context.Projects.Find(id);
        }


        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToHashSet();
        }

        public void Update(Project entity)
        {
            _context.Projects.Update(entity);
            _context.SaveChanges();
        }

    }
}
