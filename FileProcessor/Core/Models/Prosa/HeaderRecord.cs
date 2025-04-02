using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models.Prosa
{
    public class HeaderRecord
    {
        public int ArchivoId { get; set; }

        public string TipoRegistro { get; set; } = "HEADER";

        public string InstitucionGenera { get; set; } = "PROSA-ENVIA-A:";

        public string InstitucionRecibe { get; set; }

        public string LeyendaFijaFecha { get; set; } = "FECHA:";

        public string FechaProceso { get; set; } // RRMMDD

        public string LeyendaFijaConsecutivo { get; set; } = "CONSECUTIVO";

        public string NumeroConsecutivo { get; set; }

        public string LeyendaFijaTipoProceso { get; set; } = "TIPO-PROCESO:";

        public string CaracteristicasArchivo { get; set; }




    }
}

