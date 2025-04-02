using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models.Prosa
{
    public class DetailRecord
    {
        public int DetalleId { get; set; }
        public int ArchivoId { get; set; }
        public string BancoEmisor { get; set; }
        public string NumeroCuenta { get; set; }
        public string NaturalezaContable { get; set; }
        public string MarcaProducto { get; set; }
        public string FechaConsumo { get; set; }
        public string HoraConsumo { get; set; }
        public string FechaProceso { get; set; }
        public string TipoTransaccion { get; set; }
        public string NumeroLiquidacion { get; set; }
        public decimal ImporteOrigenTotal { get; set; }
        public decimal ImporteOrigenConsumo { get; set; }
        public string ClaveMonedaOrigen { get; set; }
        public decimal ImporteDestinoTotal { get; set; }
        public decimal ImporteDestinoConsumo { get; set; }
        public string ClaveMonedaDestino { get; set; }
        public decimal ParidadDestino { get; set; }
        public decimal ImporteLiquidacionTotal { get; set; }
        public decimal ImporteLiquidacionConsumo { get; set; }
        public string ClaveMonedaLiquidacion { get; set; }
        public decimal ParidadLiquidacion { get; set; }
        public decimal? ImporteCuotaIntercambio { get; set; }
        public decimal? IvaCuotaIntercambio { get; set; }
        public decimal? ImporteAplicacionTH { get; set; }
        public decimal? ImporteConsumoAplicacionTH { get; set; }
        public decimal? PorcentajeComisionAplicacionTH { get; set; }
        public string ClaveComercio { get; set; }
        public string MCCGiroComercio { get; set; }
        public string NombreComercio { get; set; }
        public string DireccionComercio { get; set; }
        public string PaisOrigenTx { get; set; }
        public string CodigoPostal { get; set; }
        public string PoblacionComercio { get; set; }
        public decimal PorcentajeCuotaIntercambio { get; set; }
        public string FamiliaComercio { get; set; }
        public string RFCComercio { get; set; }
        public string EstatusComercio { get; set; }
        public string NumeroFuente { get; set; }
        public string NumeroAutorizacion { get; set; }
        public string BancoReceptor { get; set; }
        public string ReferenciaTransaccion { get; set; }
        public string ModoAutorizacion { get; set; }
        public string IndicadorMedioAcceso { get; set; }
        public string Diferimiento { get; set; }
        public string Parcializacion { get; set; }
        public string TipoPlan { get; set; }
        public decimal? Sobretasa { get; set; }
        public decimal? IvaSobretasa { get; set; }
        public decimal? PorcentajeSobretasa { get; set; }
        public string IndicadorCobroAutomatico { get; set; }
        public string FIIDEmisor { get; set; }
        public string IndicadorDatosCompletosTrack2 { get; set; }
        public string IndicadorComercioElectronico { get; set; }
        public string IndicadorColectorAutenticacion { get; set; }
        public string CapacidadTerminal { get; set; }
        public string IndicadorTerminalActiva { get; set; }
        public string TerminalID { get; set; }
        public string ModoEntradaPos { get; set; }
        public string IndicadorCV2 { get; set; }
        public string IndicadorCAVVUCAFAAV { get; set; }
        public string FIIDAdquirente { get; set; }
        public string IndicadorPagoInterbancario { get; set; }
        public string CodigoServicio { get; set; }
        public string IndicadorPresenciaTH { get; set; }
        public string IndicadorPresenciaTarjeta { get; set; }
        public string MetodoDeIdentificacionTH { get; set; }
    }
}
