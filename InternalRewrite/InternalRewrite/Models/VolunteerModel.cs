namespace InternalRewrite.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class VolunteerModel : DbContext
    {
        public VolunteerModel()
            : base("name=masterEntities")
        {
        }

        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestion { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<VolunteerPosition> VolunteerPosition { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasMany(e => e.VolunteerPosition)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityQuestion>()
                .Property(e => e.Question)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityQuestion>()
                .Property(e => e.Answer)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notification)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.RecieverID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notification1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.SenderID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecurityQuestion)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.VolunteerPosition)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VolunteerPosition>()
                .Property(e => e.Position)
                .IsUnicode(false);

            modelBuilder.Entity<VolunteerPosition>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }
    }
}
