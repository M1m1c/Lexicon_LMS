using System;
using System.Collections.Generic;
using System.Text;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<User,IdentityRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<CourseActivity> Activities { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
    }
}
