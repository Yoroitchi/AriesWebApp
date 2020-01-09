using System.ComponentModel.DataAnnotations;

namespace AriesWebApp.Models
{
    public class AcceptConnectionViewModel
    {
        [Required]
        public string InvitationDetails { get; set; }
    }
}
