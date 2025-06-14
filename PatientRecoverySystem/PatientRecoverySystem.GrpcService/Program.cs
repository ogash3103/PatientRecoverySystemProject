using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.GrpcService.Data;
using PatientRecoverySystem.GrpcService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PatientRecoverySystem.GrpcService.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) SQL Server bilan ulanish
builder.Services.AddDbContext<PatientRecoveryContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<PatientRecoveryContext>()
    .AddDefaultTokenProviders();


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

builder.Services.AddAuthorization();

// 2) gRPC servisini qo‘shish
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
    options.MaxSendMessageSize = 10 * 1024 * 1024;    // 10 MB
});

builder.Services.AddControllers();
var app = builder.Build();

// Auto-migrate bazani har startda yangilaydi
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PatientRecoveryContext>();
    db.Database.Migrate();
}*/

using (var scope = app.Services.CreateScope()) {
  var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  foreach (var role in new[] { "Physician","Nurse","Patient" }) {
    if (!await roleMgr.RoleExistsAsync(role))
      await roleMgr.CreateAsync(new IdentityRole(role));
  }
}


app.UseAuthentication();
app.UseAuthorization();

// 3) gRPC endpoint’larni xaritalash
app.MapGrpcService<PatientRecoveryServiceImpl>();
app.MapControllers();
app.MapGet("/", () => "Patient Recovery gRPC Service ishlayapti.");

app.Run();
