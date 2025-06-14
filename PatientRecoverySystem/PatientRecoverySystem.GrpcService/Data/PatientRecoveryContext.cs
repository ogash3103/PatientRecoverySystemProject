using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Models;

namespace PatientRecoverySystem.GrpcService.Data
{
    // DbContext o‘rniga IdentityDbContext<ApplicationUser> ni foydalaning
    public class PatientRecoveryContext 
        : IdentityDbContext<ApplicationUser>
    {
        public PatientRecoveryContext(DbContextOptions<PatientRecoveryContext> options)
            : base(options)
        {
        }

        public DbSet<Diagnosis>             Diagnoses               { get; set; }
        public DbSet<MedicalRecord>         MedicalRecords          { get; set; }
        public DbSet<RehabilitationSession> RehabilitationSessions  { get; set; }
        public DbSet<Alert>                 Alerts                  { get; set; }
    }
}
