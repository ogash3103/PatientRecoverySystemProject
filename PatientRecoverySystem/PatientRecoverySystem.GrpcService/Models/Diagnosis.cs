using System;

namespace PatientRecoverySystem.GrpcService.Models
{
    public class Diagnosis
    {
        public int      DiagnosisId   { get; set; }   // PK
        public string?   PatientId     { get; set; }   // FK – bemor identifikatori
        public string?   DiagnosisText { get; set; }   // Aniqlangan tashxis matni
        public string?   Actions       { get; set; }   // Tavsiyalar "TestA||TestB" ko‘rinishda
        public DateTime Timestamp     { get; set; }   // UTC vaqti
    }
}
