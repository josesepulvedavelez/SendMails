using Microsoft.Extensions.Logging;
using SendMails.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Infraestructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            string smtpServer,
            int smtpPort,
            string smtpUser,
            string smtpPassword,
            bool enableSsl,
            ILogger<SmtpEmailService> logger)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
            _enableSsl = enableSsl;
            _logger = logger;
        }

        public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {                    
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 60000,
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_smtpUser, "Sistema de Notificaciones"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                mensaje.To.Add(destinatario);

                _logger.LogInformation($"Intentando enviar email a {destinatario} via {_smtpServer}:{_smtpPort}");

                await client.SendMailAsync(mensaje);
                _logger.LogInformation($"✅ Email enviado exitosamente a {destinatario}");
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, $"❌ Error SMTP al enviar email a {destinatario}. Código: {ex.StatusCode}");

                // Mensajes de ayuda específicos
                if (ex.StatusCode == SmtpStatusCode.MustIssueStartTlsFirst)
                {
                    _logger.LogError("Solución: Asegúrate de que EnableSsl = true y el puerto es 587 para Gmail");
                }
                else if (ex.StatusCode == SmtpStatusCode.ClientNotPermitted)
                {
                    _logger.LogError("Solución: Verifica que estás usando una Contraseña de Aplicación de Gmail, no tu contraseña normal");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error general al enviar email a {destinatario}");
                return false;
            }
        }
    }
}
