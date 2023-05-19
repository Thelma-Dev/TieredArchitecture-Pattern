using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<TicketWatcher> TicketWatchers { get; set; }

    }
}