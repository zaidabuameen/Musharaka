using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Musharaka.Models; // تأكد أن هذا يطابق اسم مشروعك

namespace Musharaka.Data
{
    // نرث من IdentityDbContext لأننا نستخدم نظام المستخدمين الجاهز (ApplicationUser)
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // تحويل الكلاسات إلى جداول (DbSet) في قاعدة البيانات
        public DbSet<PoliticalParty> PoliticalParties { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // إعدادات إضافية للعلاقات (للتأكد من عدم حذف البيانات بالخطأ)
            builder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade); // إذا حُذف المستخدم، تُحذف عضوياته

            builder.Entity<Membership>()
                .HasOne(m => m.Party)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.PartyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}