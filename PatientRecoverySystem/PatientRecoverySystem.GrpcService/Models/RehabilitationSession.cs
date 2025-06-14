using System;

namespace PatientRecoverySystem.GrpcService.Models
{
    public class RehabilitationSession
    {
        public int      RehabilitationSessionId { get; set; } // PK
        public string?   PatientId               { get; set; }
        public string Exercises { get; set; } = null!;// "ex1||ex2"
        public int      PainLevel               { get; set; }
        public string?   Notes                   { get; set; }
        public DateTime Timestamp               { get; set; }
    }
}
