using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [StringLength(1000, ErrorMessage = "Comment should be from 1 upto 1000 characters only", MinimumLength =1)]
        [DisplayName("Comments :")]
        public string Description { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }

    }
}
