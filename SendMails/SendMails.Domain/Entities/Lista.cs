using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Domain.Entities
{
    [Table("Lista")]
    public class Lista
    {
        [Key]
        public int Id { get; set; }

        // Permitir NULL con string? (nullable)
        public string? Nombre { get; set; }
        public string? Sector { get; set; }
        public string? Email { get; set; }
        public string? Web { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }

        public bool EmailEnviado { get; set; }
        public DateTime? FechaEnvio { get; set; }
    }
}
