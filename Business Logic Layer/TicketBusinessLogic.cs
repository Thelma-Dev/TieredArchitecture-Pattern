using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Business_Logic_Layer
{
    public class TicketBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Project> _projectRepository;
        private IUserProjectRepository _userProjectRepository;
        private IUserRepository _userRepository;
        private IRepository<Ticket> _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketBusinessLogic(UserManager<ApplicationUser> userManager, IRepository<Project> projectRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
        }

        public void MarkASCompleted(int id)
        {
            if (id == null)
            {
                throw new Exception("Ticket not found");
                
            }
            else
            {

                Ticket ticket = _ticketRepository.Get(id);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    ticket.Completed = true;

                    _ticketRepository.Update(ticket);
                }                
            }
        }

        public void UnMarkAsCompleted(int id)
        {
            if (id == null)
            {
                throw new Exception("Ticket not found");

            }
            else
            {

                Ticket ticket = _ticketRepository.Get(id);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    ticket.Completed = false;

                    _ticketRepository.Update(ticket);
                }
            }
        }

        public Ticket DeleteTicket(int? id)
        {
            if (id == null )
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(id);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    return ticket;
                }

                
            }            
        }

        public void ConfirmDeleteTicket(int ticketId, int ProjectId)
        {
            if(ticketId == null)
            {
                throw new Exception("Ticket not found");
            }
            else
            {
                Ticket? ticket = _ticketRepository.Get(ticketId);

                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    Project? projectWithTicket = _projectRepository.Get(ProjectId);

                    if(projectWithTicket == null)
                    {
                        throw new Exception("Project not found");
                    }
                    else
                    {
                        projectWithTicket.Tickets.Remove(ticket);

                        _ticketRepository.Delete(ticket);
                    }
                }

            }
            
        }
    }
}
