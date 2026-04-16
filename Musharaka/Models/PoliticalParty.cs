using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class PoliticalParty
    {
        [Key]
        public int PartyId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Ideology { get; set; } = string.Empty;

        public DateTime FoundDate { get; set; }

        public string LogoUrl { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public virtual ICollection<Membership> Members { get; set; } = new List<Membership>();
        public virtual ICollection<Election> Elections { get; set; } = new List<Election>();
    }
}