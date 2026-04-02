using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Application.DTOs
{
    public class EmailConfigDto
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public int CantidadEmailsPorEjecucion { get; set; }
        public int IntervaloMinutos { get; set; }
    }
}
