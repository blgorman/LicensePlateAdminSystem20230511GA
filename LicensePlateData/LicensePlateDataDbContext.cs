using LicensePlateDataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LicensePlateDataLibrary
{
    public class LicensePlateDataDbContext : DbContext
    {
        private static IConfigurationRoot _configuration;

        public DbSet<LicensePlate> LicensePlates { get; set; }

        public LicensePlateDataDbContext()
        {
            //purposefully empty: Necessary for Scaffold 
        }

        public LicensePlateDataDbContext(DbContextOptions options)
            : base(options)
        {
            //purposefully empty: sets options appropriatly
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("LicensePlateDataDbConnection");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var ts = new DateTime(2021, 12, 31);

            modelBuilder.Entity<LicensePlate>(x =>
            {
                x.HasData(
                    new LicensePlate() { Id = 1, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE1.jpg", IsProcessed = true, LicensePlateText = "FUNTIME", TimeStamp = ts },
                    new LicensePlate() { Id = 2, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE2.jpg", IsProcessed = true, LicensePlateText = "GVMESPD", TimeStamp = ts },
                    new LicensePlate() { Id = 3, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE3.jpg", IsProcessed = true, LicensePlateText = "PULTOYS", TimeStamp = ts },
                    new LicensePlate() { Id = 4, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE4.jpg", IsProcessed = true, LicensePlateText = "NCC1701", TimeStamp = ts },
                    new LicensePlate() { Id = 5, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE5.jpg", IsProcessed = true, LicensePlateText = "MYCAR01", TimeStamp = ts },
                    new LicensePlate() { Id = 6, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE6.jpg", IsProcessed = true, LicensePlateText = "FBIAGNT", TimeStamp = ts },
                    new LicensePlate() { Id = 7, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE7.jpg", IsProcessed = true, LicensePlateText = "FLYERS1", TimeStamp = ts },
                    new LicensePlate() { Id = 8, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE8.jpg", IsProcessed = true, LicensePlateText = "EMC2FST", TimeStamp = ts },
                    new LicensePlate() { Id = 9, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE9.jpg", IsProcessed = true, LicensePlateText = "FSTRNU", TimeStamp = ts },
                    new LicensePlate() { Id = 10, FileName = "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE10.jpg", IsProcessed = true, LicensePlateText = "BACKOFF", TimeStamp = ts }

                );
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}