using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Models;

namespace PatientRecoverySystem.GrpcService.Data
{
    public class PatientRecoveryContext : DbContext
    {
        public PatientRecoveryContext(DbContextOptions<PatientRecoveryContext> options)
            : base(options) { }

        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<RehabilitationSession> RehabilitationSessions { get; set; }
        public DbSet<Alert> Alerts { get; set; }

    }
}
