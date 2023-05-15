using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketWatchersRepository : IRepository<TicketWatcher>
    {
        private ApplicationDbContext _context;

        public TicketWatchersRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(TicketWatcher entity)
        {
            _context.TicketWatchers.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(TicketWatcher entity)
        {
            _context.TicketWatchers.Remove(entity);
            _context.SaveChanges();
        }

        public TicketWatcher? Get(int? id)
        {
            throw new NotImplementedException();
        }

        public ICollection<TicketWatcher> GetAll()
        {
            return _context.TicketWatchers.ToList();
        }

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public void Update(TicketWatcher entity)
        {
            _context.TicketWatchers.Update(entity);
            _context.SaveChanges();
        }
    }
}
