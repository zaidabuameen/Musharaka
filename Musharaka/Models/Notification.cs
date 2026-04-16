using System.ComponentModel.DataAnnotations;

namespace Musharaka.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; } // [cite: 451]

        public string UserId { get; set; } = string.Empty; // المواطن المستلم [cite: 464]
        public virtual ApplicationUser User { get; set; } = null!;

        public string Message { get; set; } = string.Empty; // نص الرسالة [cite: 462]

        public bool IsRead { get; set; } = false; // هل قرأها المستخدم؟ [cite: 459]

        public DateTime CreatedAt { get; set; } = DateTime.Now; // وقت الإرسال [cite: 456]
    }
}