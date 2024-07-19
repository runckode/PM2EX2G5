using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2EX2G5.Models
{
    public class Sitios
    {
        public int Id { get; set; }
        public double Latitud { get; set; } 
        public double Longitud { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public byte[] FirmaDigital { get; set; }
        public byte[] AudioFile { get; set; }
    }
}
