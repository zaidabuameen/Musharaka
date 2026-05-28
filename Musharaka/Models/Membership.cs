using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // تذكر إضافة هذا السطر للـ ForeignKey

namespace Musharaka.Models
{
    public class Membership
    {
        [Key]
        public int MembershipId { get; set; }

        // الربط مع المستخدم
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")] // تحديد صريح لعلاقة المستخدم
        public virtual ApplicationUser User { get; set; } = null!;

        // الربط مع الحزب
        public int PartyId { get; set; }

        [ForeignKey("PartyId")] // تحديد صريح لعلاقة الحزب (هذا السطر اللي بيحل مشكلة الـ Invalid Column Name)
        public virtual PoliticalParty Party { get; set; } = null!;

        [Required]
        public string NationalIdCardUrl { get; set; } = string.Empty; // رابط صورة الهوية الشخصية

        public string JobTitle { get; set; } = string.Empty; // المهنة (مهمة للأحزاب)

        public string EducationLevel { get; set; } = string.Empty; // المستوى التعليمي

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public string MembershipStatus { get; set; } = "Pending"; // (Pending, Approved, Rejected)

        public string? AdminNotes { get; set; } // ملاحظات الأدمن (في حال الرفض مثلاً)
    }
}