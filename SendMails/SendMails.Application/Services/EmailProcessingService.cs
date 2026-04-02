using Microsoft.Extensions.Logging;
using SendMails.Application.DTOs;
using SendMails.Domain.Entities;
using SendMails.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Application.Services
{
    public class EmailProcessingService
    {
        private readonly IRepository<Lista> _repository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailProcessingService> _logger;
        private readonly EmailConfigDto _config;

        public EmailProcessingService(
            IRepository<Lista> repository,
            IEmailService emailService,
            ILogger<EmailProcessingService> logger,
            EmailConfigDto config)
        {
            _repository = repository;
            _emailService = emailService;
            _logger = logger;
            _config = config;
        }

        public async Task<int> ProcesarEnvioEmailsAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de envío de emails");

                // Obtener registros pendientes
                var pendientes = await _repository.GetPendientesEnvioAsync(_config.CantidadEmailsPorEjecucion);

                if (!pendientes.Any())
                {
                    _logger.LogInformation("No hay emails pendientes por enviar");
                    return 0;
                }

                int enviados = 0;
                int errores = 0;

                foreach (var registro in pendientes)
                {
                    try
                    {
                        var asunto = $"Información para {registro.Nombre}";
                        var cuerpo = GenerarCuerpoEmail(registro);

                        var resultado = await _emailService.EnviarEmailAsync(registro.Email, asunto, cuerpo);

                        if (resultado)
                        {
                            registro.EmailEnviado = true;
                            registro.FechaEnvio = DateTime.Now;
                            await _repository.UpdateAsync(registro);
                            enviados++;
                            _logger.LogInformation($"Email enviado a: {registro.Email}");
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            errores++;
                            _logger.LogWarning($"Error al enviar email a: {registro.Email}");
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        errores++;
                        _logger.LogError(ex, $"Excepción al enviar email a: {registro.Email}");
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }

                _logger.LogInformation($"Proceso completado. Enviados: {enviados}, Errores: {errores}");
                return enviados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de envío de emails");
                throw;
            }
        }

        private string GenerarCuerpoEmail(Lista registro)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #2c3e50; color: white; padding: 20px; text-align: center; }}
                .content {{ padding: 20px; background-color: #f9f9f9; }}
                .footer {{ background-color: #ecf0f1; padding: 15px; text-align: center; font-size: 12px; }}
                .button {{ display: inline-block; padding: 10px 20px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; }}
                .contact-info {{ background-color: #e8f4fd; padding: 15px; border-radius: 5px; margin-top: 20px; }}
                hr {{ border: none; border-top: 1px solid #ddd; margin: 20px 0; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>¡Hola {registro.Nombre ?? "cliente"}!</h2>
                </div>
                
                <div class='content'>
                    <p>Te escribimos desde NOMBRE DE LA EMPRESA 💡. Ayudamos a empresas a crecer con soluciones tecnológicas a la medida.</p>
                    
                    <p>En <strong>NOMBRE DE LA EMPRESA</strong> nos apasiona ayudar a empresas como la tuya a transformar sus ideas en realidad digital. Ofrecemos servicios de:</p>
                    
                    <ul>
                        <li>🌐 <strong>Desarrollo de sitios web</strong> - Presencia digital profesional y atractiva</li>
                        <li>💻 <strong>Aplicaciones web a medida</strong> - Soluciones personalizadas para tus procesos</li>
                        <li>📱 <strong>Aplicaciones móviles</strong></li>
                        <li>🤖 <strong>Automatización con IA</strong> - Optimización de procesos con inteligencia artificial</li>
                    </ul>
                    
                    <p>Creemos que podemos colaborar y aportar valor a tu organización con nuestras soluciones tecnológicas.</p>
                    
                    <div class='contact-info'>
                        <h3>📬 Contáctame:</h3>
                        <p><strong>NOMBRE DEL GERENTE O CTO</strong><br/>
                        <strong>CTO</strong></p>
                        <p>🌐 <a href='https://NOMBREEMPRESA.COM' target='_blank' style='color: #3498db;'>NOMBREEMPRESA.COM</a><br/>
                        📞 <strong>Teléfonos:</strong>TELEFONOS DE LA EMPRESA</p>
                    </div>
                    
                    <hr/>                    
                </div>
                
                <div class='footer'>
                    <p>NOMBRE DE LA EMPRESA - Transformando ideas en soluciones digitales</p>
                    <p>Este es un correo de contacto comercial. Si no deseas recibir más comunicaciones, puedes responder solicitando no recibir más mensajes.</p>
                </div>
            </div>
        </body>
        </html>";
        }

    }
}
