using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class Membership
    {
        [Key]
        public int MembershipId { get; set; } // [cite: 487]

        public string UserId { get; set; } = string.Empty; // معرف المستخدم [cite: 490]
        public virtual ApplicationUser User { get; set; } = null!;

        public int PartyId { get; set; } // معرف الحزب [cite: 498]
        public virtual PoliticalParty Party { get; set; } = null!;

        public DateTime JoinDate { get; set; } = DateTime.Now; // تاريخ الطلب [cite: 495]

        public string MembershipStatus { get; set; } = "Pending"; // حالة الطلب (قيد الانتظار/مقبول) [cite: 500]
    }
}