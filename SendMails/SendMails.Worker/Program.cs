using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendMails.Application.DTOs;
using SendMails.Application.Services;
using SendMails.Domain.Entities;
using SendMails.Domain.Interfaces;
using SendMails.Worker;
using SendMails.Infraestructure.Data;
using SendMails.Infraestructure.Repositories;
using SendMails.Infraestructure.Services;

var builder = Host.CreateApplicationBuilder(args);

// 1. Configurar el servicio para Windows
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "SendMailsService";
});

// 2. Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventLog();

// 3. Configurar appsettings.json
builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// 4. Configurar EmailSettings desde appsettings.json
var emailConfig = new EmailConfigDto();
builder.Configuration.GetSection("EmailSettings").Bind(emailConfig);
builder.Services.AddSingleton(emailConfig);

// 5. Configurar conexión a SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// 6. Registrar los repositorios y servicios
builder.Services.AddScoped<IRepository<Lista>, ListaRepository>();

// Registrar IEmailService con los parámetros de configuración
builder.Services.AddScoped<IEmailService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<SmtpEmailService>>();
    return new SmtpEmailService(
        emailConfig.SmtpServer,
        emailConfig.SmtpPort,
        emailConfig.SmtpUser,
        emailConfig.SmtpPassword,
        emailConfig.EnableSsl,
        logger);
});

// Registrar el servicio de procesamiento de emails
builder.Services.AddScoped<EmailProcessingService>();

// 7. Registrar el Worker (tu servicio principal)
builder.Services.AddHostedService<Worker>();

// 8. Configurar salud del servicio (opcional pero recomendado)
// Necesitas agregar el paquete: Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var host = builder.Build();

// 9. Inicializar la base de datos (opcional)
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones de base de datos");
    }
}

// 10. Ejecutar el host
await host.RunAsync();