using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models.Prosa
{
    public class TrailerRecord
    {
        public string TipoRegistro { get; set; } = "TRAILER";
        public int TotalTransacciones { get; set; }
        public int TotalVentas { get; set; }
        public decimal ImporteVentas { get; set; }
        public int TotalDisposiciones { get; set; }
        public decimal ImporteDisposiciones { get; set; }
        public int TotalDebitos { get; set; }
        public decimal ImporteDebitos { get; set; }
        public int TotalPagosInterbancarios { get; set; }
        public decimal ImportePagosInterbancarios { get; set; }
        public int TotalDevoluciones { get; set; }
        public decimal ImporteDevoluciones { get; set; }
        public int TotalCreditos { get; set; }
        public decimal ImporteCreditos { get; set; }
        public int TotalRepresentaciones { get; set; }
        public decimal ImporteRepresentaciones { get; set; }
        public int TotalContracargos { get; set; }
        public decimal ImporteContracargos { get; set; }
        public decimal TotalComisiones { get; set; }
        public int ArchivoId { get; set; }
    }
}
