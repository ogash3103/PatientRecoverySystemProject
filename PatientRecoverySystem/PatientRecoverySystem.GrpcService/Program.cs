using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Data;
using PatientRecoverySystem.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) SQL Server bilan ulanish
builder.Services.AddDbContext<PatientRecoveryContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) gRPC servisini qo‘shish
builder.Services.AddGrpc();

var app = builder.Build();

// Auto-migrate bazani har startda yangilaydi
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PatientRecoveryContext>();
    db.Database.Migrate();
}

// 3) gRPC endpoint’larni xaritalash
app.MapGrpcService<PatientRecoveryServiceImpl>();
app.MapGet("/", () => "Patient Recovery gRPC Service ishlayapti.");

app.Run();
