 namespace PatientRecoverySystem.GrpcService.Models
 {
     public class MedicalRecord
     {
       // public int RecordId { get; set; }    // bu nom qoladi, lekin EF uni PK deb bilmadi
        public int MedicalRecordId { get; set; }  // konvensiyaga mos: <ClassName>Id → EF avtomatik PK qiladi

         public string?   PatientId     { get; set; }
         public double   Temperature   { get; set; }
         public string?   BloodPressure { get; set; }
         public string?   Ecg           { get; set; }
         public string?   Notes         { get; set; }
         public DateTime Timestamp     { get; set; }
     }
 }
