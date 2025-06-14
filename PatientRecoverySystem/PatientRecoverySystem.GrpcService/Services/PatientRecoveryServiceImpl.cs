using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Data;
using PatientRecoverySystem.GrpcService.Models;
using PatientRecoverySystem.GrpcService;       // generated kod uchun

namespace PatientRecoverySystem.GrpcService.Services
{
    public class PatientRecoveryServiceImpl
        : PatientRecoveryService.PatientRecoveryServiceBase
    {
        private readonly PatientRecoveryContext _context;
        public PatientRecoveryServiceImpl(PatientRecoveryContext context)
            => _context = context;

        // task 1 
        public override async Task<DiagnoseResponse> DiagnosePatient(
            DiagnoseRequest request, ServerCallContext context)
        {
            // Bu yerga avval yozgan tashxis logikangizni joylang.
            var diagText = $"Symptoms: [{string.Join(", ", request.Symptoms)}] …";
            var actions = new List<string> { "Test A", "Test B" };

            var diag = new Diagnosis
            {
                PatientId = request.PatientId,
                DiagnosisText = diagText,
                Actions = string.Join("||", actions),
                Timestamp = DateTime.UtcNow
            };
            _context.Diagnoses.Add(diag);
            await _context.SaveChangesAsync();

            var response = new DiagnoseResponse
            {
                Diagnosis = diagText,
                Timestamp = diag.Timestamp.ToString("o")
            };
            response.Actions.AddRange(actions);
            return response;
        }

        // Task 2.1: RecordMedicalData
        public override async Task<MedicalDataResponse> RecordMedicalData(
            MedicalDataRequest request, ServerCallContext ctx)
        {
            var record = new MedicalRecord
            {
                PatientId = request.PatientId,
                Temperature = request.Temperature,
                BloodPressure = request.BloodPressure,
                Ecg = request.Ecg,
                Notes = request.Notes,
                Timestamp = DateTime.UtcNow
            };
            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();
            return new MedicalDataResponse { Success = true };
        }

        // Task 2.2: GetMedicalData
        public override async Task<GetMedicalDataResponse> GetMedicalData(
            GetMedicalDataRequest request, ServerCallContext ctx)
        {
            var resp = new GetMedicalDataResponse();
            var list = await _context.MedicalRecords
                             .Where(r => r.PatientId == request.PatientId)
                             .OrderByDescending(r => r.Timestamp)
                             .ToListAsync();

            foreach (var r in list)
            {
                resp.Records.Add(new MedicalRecordInfo
                {
                    RecordId = r.MedicalRecordId,
                    PatientId = r.PatientId,
                    Temperature = r.Temperature,
                    BloodPressure = r.BloodPressure,
                    Ecg = r.Ecg,
                    Notes = r.Notes,
                    Timestamp = r.Timestamp.ToString("o")
                });
            }

            return resp;
        }

// === Task 3.1: RecordRehabilitationProgress ===
        public override async Task<RehabilitationProgressResponse> RecordRehabilitationProgress(
            RehabilitationProgressRequest request, ServerCallContext ctx)
        {
            var session = new RehabilitationSession
            {
                PatientId = request.PatientId,
                Exercises = string.Join("||", request.Exercises),
                PainLevel = request.PainLevel,
                Notes     = request.Notes,
                Timestamp = DateTime.UtcNow
            };
            _context.RehabilitationSessions.Add(session);
            await _context.SaveChangesAsync();
            return new RehabilitationProgressResponse { Success = true };
        }

        // === Task 3.2: GetRehabilitationHistory ===
        public override async Task<GetRehabilitationResponse> GetRehabilitationHistory(
            GetRehabilitationRequest request, ServerCallContext ctx)
        {
            var resp = new GetRehabilitationResponse();
            var list = await _context.RehabilitationSessions
                             .Where(r => r.PatientId == request.PatientId)
                             .OrderByDescending(r => r.Timestamp)
                             .ToListAsync();

            foreach (var r in list)
            {
                resp.Sessions.Add(new RehabilitationSessionInfo {
                    SessionId = r.RehabilitationSessionId,
                    PatientId = r.PatientId,
                    Exercises = { r.Exercises.Split("||") },
                    PainLevel = r.PainLevel,
                    Notes     = r.Notes,
                    Timestamp = r.Timestamp.ToString("o")
                });
            }
            return resp;
        }

    }
}
