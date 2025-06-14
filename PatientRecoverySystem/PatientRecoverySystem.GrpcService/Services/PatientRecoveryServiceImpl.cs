using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Data;
using PatientRecoverySystem.GrpcService.Models;
using PatientRecoverySystem.GrpcService;

namespace PatientRecoverySystem.GrpcService.Services
{
    public class PatientRecoveryServiceImpl : PatientRecoveryService.PatientRecoveryServiceBase
    {
        private readonly PatientRecoveryContext _context;
        public PatientRecoveryServiceImpl(PatientRecoveryContext context) => _context = context;

        // Task 1: Diagnose
        public override async Task<DiagnoseResponse> DiagnosePatient(
            DiagnoseRequest request, ServerCallContext context)
        {
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

        // Task 2: Monitor
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

            // Alert logic: high temperature
            if (request.Temperature > 38)
            {
                var alert = new Alert
                {
                    PatientId = request.PatientId,
                    AlertType = "HighTemperature",
                    Message = $"High temp: {request.Temperature}°C",
                    RecipientRole = "Physician",
                    IsSent = false,
                    IsAcknowledged = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Alerts.Add(alert);
            }

            await _context.SaveChangesAsync();
            return new MedicalDataResponse { Success = true };
        }

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

        // Task 3: Rehabilitation
        public override async Task<RehabilitationProgressResponse> RecordRehabilitationProgress(
            RehabilitationProgressRequest request, ServerCallContext ctx)
        {
            var session = new RehabilitationSession
            {
                PatientId = request.PatientId,
                Exercises = string.Join("||", request.Exercises),
                PainLevel = request.PainLevel,
                Notes = request.Notes,
                Timestamp = DateTime.UtcNow
            };
            _context.RehabilitationSessions.Add(session);
            await _context.SaveChangesAsync();
            return new RehabilitationProgressResponse { Success = true };
        }

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
                var info = new RehabilitationSessionInfo
                {
                    SessionId = r.RehabilitationSessionId,
                    PatientId = r.PatientId,
                    PainLevel = r.PainLevel,
                    Notes = r.Notes,
                    Timestamp = r.Timestamp.ToString("o")
                };
                info.Exercises.AddRange(r.Exercises.Split("||"));
                resp.Sessions.Add(info);
            }
            return resp;
        }

        // Task 4: Alerts
        public override async Task<GetAlertsResponse> GetAlerts(
            GetAlertsRequest request, ServerCallContext ctx)
        {
            var resp = new GetAlertsResponse();
            var list = await _context.Alerts
                .Where(a => a.PatientId == request.PatientId && a.RecipientRole == request.Role)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            foreach (var a in list)
            {
                resp.Alerts.Add(new AlertInfo
                {
                    AlertId = a.AlertId,
                    PatientId = a.PatientId,
                    AlertType = a.AlertType,
                    Message = a.Message,
                    RecipientRole = a.RecipientRole,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = a.CreatedAt.ToString("o")
                });
            }
            return resp;
        }

        public override async Task<AcknowledgeAlertResponse> AcknowledgeAlert(
            AcknowledgeAlertRequest request, ServerCallContext ctx)
        {
            var alert = await _context.Alerts.FindAsync(request.AlertId);
            if (alert == null)
                return new AcknowledgeAlertResponse { Success = false };

            alert.IsAcknowledged = true;
            alert.AckAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new AcknowledgeAlertResponse { Success = true };
        }
    }
}
