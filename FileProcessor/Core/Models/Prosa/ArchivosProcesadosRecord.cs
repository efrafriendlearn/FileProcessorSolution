using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models.Prosa
{
    public class ArchivosProcesadosRecord
    {
        public int ArchivoId { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime FechaProcesamiento { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosCargados { get; set; }
        public string Estado { get; set; }
        public string HashArchivo { get; set; }
    }
}
