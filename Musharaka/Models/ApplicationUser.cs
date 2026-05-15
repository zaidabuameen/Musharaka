using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(10)]
        public string NationalId { get; set; } = string.Empty; // الرقم الوطني الأردني

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = "/images/default-user.png"; // صورة الشخص

        [Required]
        public override string PhoneNumber { get; set; } = string.Empty; // رقم الهاتف

        public string Governorate { get; set; } = string.Empty; // المحافظة

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}