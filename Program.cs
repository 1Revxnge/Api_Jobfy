using ApiJobfy.Data;
using ApiJobfy.Services;
using ApiJobfy.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiJobfy.Services.ApiJobfy.Services;
using ApiJobfy.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))  // Certifique-se de que a vers�o do MySQL est� correta
    ));

// Add controllers
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin() // Permite qualquer origem
                .AllowAnyHeader() // Permite qualquer cabe�alho
                .AllowAnyMethod(); // Permite qualquer m�todo HTTP
        });
});
// Add AutoMapper if needed (not added now for simplicity)

// Add custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<IEnderecoService, EnderecoService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();


// Configure Authentication with JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // For dev only
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Add authorization policy (RBAC)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("FuncionarioPolicy", policy => policy.RequireRole("Funcionario", "Admin"));
    options.AddPolicy("CandidatoPolicy", policy => policy.RequireRole("Candidato"));
});

var app = builder.Build();
app.UseCors("AllowAll"); // Aplica a pol�tica de CORS

// Configure the HTTP request pipeline.

app.UseRouting();
app.MapGet("/", () => "API funcionando!"); // Esta linha mapeia a raiz "/".

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
