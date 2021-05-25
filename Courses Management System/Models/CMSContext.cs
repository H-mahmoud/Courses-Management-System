using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Courses_Management_System.Models
{
    public class CMSContext : IdentityDbContext
    {
        public CMSContext() : base("DBconnection")
        {
                
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Courses> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasMany<Courses>(s => s.Courses)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("CourseId");
                    cs.ToTable("UserCourse");
                });

            base.OnModelCreating(modelBuilder);
        }

    }
}