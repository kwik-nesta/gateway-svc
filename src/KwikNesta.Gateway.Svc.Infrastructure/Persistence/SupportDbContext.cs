using KwikNesta.Contracts.Models.Location;
using KwikNesta.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace KwikNesta.Gateway.Svc.Infrastructure.Persistence
{
    public class SupportDbContext : DbContext
    {
        public DbSet<KwikNestaAuditLog> AuditLogs { get; set; }
        public DbSet<KwikNestaLocationCountry> Countries { get; set; }
        public DbSet<KwikNestaLocationState> States { get; set; }
        public DbSet<KwikNestaLocationCity> Cities { get; set; }
        public DbSet<KwikNestaLocationTimeZone> TimeZones { get; set; }

        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("sysyem-support-svc");

            base.OnModelCreating(builder);
        }
    }
}