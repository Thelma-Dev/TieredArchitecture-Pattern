using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketRepository : IRepository<Ticket>
    {
        private ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Ticket entity)
        {
            _context.CreateTicket(entity);
            _context.SaveChanges();
        }

        public void Delete(Ticket entity)
        {
            _context.DeleteTicket(entity);
            _context.SaveChanges();
        }

        public Ticket? Get(int? id)
        {
            return _context.Tickets.First(t => t.Id ==  id);
        }

        public ICollection<Ticket> GetAll()
        {
            return _context.Tickets.ToHashSet();
        }

		public void SaveChanges()
		{
            _context.SaveChanges();
		}

		public void Update(Ticket entity)
        {
           _context.Tickets.Update(entity);
            _context.SaveChanges();
        }
       
    }
}
