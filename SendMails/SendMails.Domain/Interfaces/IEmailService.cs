using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Domain.Interfaces
{
    public interface IEmailService
    {
        Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo);
    }
}
