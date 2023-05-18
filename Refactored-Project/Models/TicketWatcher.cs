namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class TicketWatcher
    {
        public int Id { get; set; }

        public string WatcherId { get; set; }

        public ApplicationUser Watcher { get; set; }


        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }
    }
}
