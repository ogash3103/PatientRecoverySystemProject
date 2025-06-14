using System;

namespace PatientRecoverySystem.GrpcService.Models
{
    public class Alert
    {
        public int      AlertId       { get; set; } // PK
        public string?   PatientId     { get; set; }
        public string?   AlertType     { get; set; } // e.g. "HighTemperature", "HighPain"
        public string?   Message       { get; set; } // ogohlantirish matni
        public string?   RecipientRole { get; set; } // "Physician"|"Nurse"|"Patient"
        public bool     IsSent        { get; set; } // jo‘natildi-mi?
        public bool     IsAcknowledged { get; set; } // qabul qilindi-mi?
        public DateTime CreatedAt     { get; set; } // UTC
        public DateTime? SentAt       { get; set; }
        public DateTime? AckAt        { get; set; }
    }
}
