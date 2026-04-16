using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; } // [cite: 474]

        public int ElectionId { get; set; } // الانتخابات المرتبطة [cite: 477]
        public virtual Election Election { get; set; } = null!;

        public string UserId { get; set; } = string.Empty; // الشخص الذي صوت [cite: 476]
        public virtual ApplicationUser User { get; set; } = null!;

        public string VoteValue { get; set; } = string.Empty; // قيمة التصويت (مثلاً: نعم/لا) [cite: 478]

        public DateTime Timestamp { get; set; } = DateTime.Now; // وقت التصويت [cite: 479]
    }
}