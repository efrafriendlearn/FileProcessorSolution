using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models.Prosa
{
    public class DetailEMVRecord
    {
        public int DetalleId { get; set; }
        public string NumeroAutorizacion { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoRegistro { get; set; } = "03";
        public string ApplicationCryptogram { get; set; }
        public string CryptogramInformationData { get; set; }
        public string IssuerApplicationData { get; set; }
        public string UnpredictableNumber { get; set; }
        public string ApplicationTransactionCounter { get; set; }
        public string TerminalVerificationResult { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string AmountAuthorized { get; set; }
        public string TransactionCurrencyCode { get; set; }
        public string ApplicationInterchangeProfile { get; set; }
        public string TerminalCountryCode { get; set; }
        public string AmountOther { get; set; }
        public string CardholderVerificationMethod { get; set; }
        public string TerminalCapabilities { get; set; }
        public string TerminalType { get; set; }
        public string InterfaceDeviceSerialNumber { get; set; }
        public string DedicatedFileName { get; set; }
        public string TerminalApplicationVersionNumber { get; set; }
        public string IssuerAuthenticationData { get; set; }
    }
}
