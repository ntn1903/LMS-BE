using LMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure
{
    public class LmsDbContext : IdentityDbContext<User>
    {
        public LmsDbContext() { }
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            var path = Directory.GetParent(Directory.GetCurrentDirectory());
            string appSettingsPath = Path.Combine(path.ToString(), "LMS.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("defaultConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            //optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var role = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            };
            builder.Entity<IdentityRole>().HasData(role);

            var user = new User
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@lms.com",
                NormalizedEmail = "admin@lms.com".ToUpper(),
                PasswordHash = "AQAAAAIAAYagAAAAEGddRu4wFFcMDqtOybnlHuCSfK7le73nz7POKLZ1hJUEE9kUGw/+YTmnWubJGuBnpQ==" // Passw0rd!
            };
            builder.Entity<User>().HasData(user);

            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id,
            };
            builder.Entity<IdentityUserRole<string>>().HasData(userRole);

            //// Unit Data
            //List<Unit> units = new List<Unit>
            //    {
            //        new Unit{ Id = 1, Name = "KG" },
            //        new Unit{ Id = 2, Name = "PKG" },
            //    };
            //builder.Entity<Unit>().HasData(units);
        }
    }
}
