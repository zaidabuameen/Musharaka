using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class Election
    {
        [Key]
        public int ElectionId { get; set; } // [cite: 507]

        public int PartyId { get; set; } // الحزب الذي ينظم الانتخابات [cite: 508]
        public virtual PoliticalParty Party { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty; // عنوان الانتخابات [cite: 514]

        public string Description { get; set; } = string.Empty; // التفاصيل [cite: 512]

        public DateTime StartDate { get; set; } // تاريخ البدء [cite: 509]

        public DateTime EndDate { get; set; } // تاريخ الانتهاء [cite: 510]

        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>(); // قائمة الأصوات [cite: 569]
    }
}