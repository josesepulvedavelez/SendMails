using Microsoft.Extensions.Options;
using SendMails.Application.DTOs;
using SendMails.Application.Services;

namespace SendMails.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailConfigDto _config;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IOptions<EmailConfigDto> config,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = config.Value;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de envío de emails iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailProcessingService>();

                    await emailService.ProcesarEnvioEmailsAsync();

                    _logger.LogInformation($"Esperando {_config.IntervaloMinutos} minutos para la próxima ejecución");
                    await Task.Delay(TimeSpan.FromMinutes(_config.IntervaloMinutos), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el ciclo principal del servicio");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Servicio deteniéndose...");
            await base.StopAsync(cancellationToken);
        }

    }
}
