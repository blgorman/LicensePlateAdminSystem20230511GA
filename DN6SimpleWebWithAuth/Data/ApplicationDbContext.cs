using LicensePlateDataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DN6SimpleWebWithAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //public DbSet<LicensePlateData> LicensePlates { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        

    }
}