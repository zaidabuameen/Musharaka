using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(10)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string Governorate { get; set; } = string.Empty;

        public string LanguagePreference { get; set; } = "Arabic";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}