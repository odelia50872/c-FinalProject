using GatherUp.API.Middleware;
using GatherUp.API.Services;
using GatherUp.API.Settings;
using GatherUp.BL.Services;
using GatherUp.core.DO;
using GatherUp.core.interfaces;
using GatherUp.Infrastructure.Repositories;
using GatherUp.Infrastructure.Services;
using SmtpSettings = GatherUp.Infrastructure.Services.SmtpSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var xmlFolder = Path.Combine(AppContext.BaseDirectory, builder.Configuration["XmlFolder"] ?? "XML");
Directory.CreateDirectory(xmlFolder);

builder.Services.AddSingleton<IRepository<Event>>(_ => new XmlRepository<Event>(xmlFolder));
builder.Services.AddSingleton<IRepository<Participant>>(_ => new XmlRepository<Participant>(xmlFolder));
builder.Services.AddSingleton<IRepository<Poll>>(_ => new XmlRepository<Poll>(xmlFolder));

builder.Services.AddSingleton<IMailService>(sp =>
{
    var smtp = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>();
    return new MailService(Path.Combine(builder.Environment.ContentRootPath, "MailLog.txt"), smtp);
});

builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IParticipantService, ParticipantService>();
builder.Services.AddSingleton<IFinancialService, FinancialService>();
builder.Services.AddSingleton<IPollService, PollService>();
builder.Services.AddSingleton<IVendorService, VendorService>();
builder.Services.AddSingleton<EventNotificationHandler>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GatherUp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "Bearer",
        BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.Services.GetRequiredService<EventNotificationHandler>();

app.UseMiddleware<ExceptionMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
