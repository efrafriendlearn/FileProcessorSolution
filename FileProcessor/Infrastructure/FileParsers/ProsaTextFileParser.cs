using FileProcessor.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using FileProcessor.Core.Models;
using FileProcessor.Core.Models.Prosa;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using FileProcessor.Infrastructure.Data.Prosa;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using FileProcessor.Core.Services;

namespace FileProcessor.Infrastructure.FileParsers
{
    public class ProsaTextFileParser : BaseFileParser, IFileProcessor
    {
        private readonly ILogger<ProsaTextFileParser> _logger;
        private readonly ProsaPosteoService _prosaPosteoService;
        private readonly IConfiguration _configuration;

        public ProsaTextFileParser(
            IConfiguration configuration,
            ILogger<ProsaTextFileParser> logger,
            ProsaPosteoService prosaPosteoService)
            : base(logger) // Asegúrate que BaseFileParser recibe el logger
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _prosaPosteoService = prosaPosteoService ?? throw new ArgumentNullException(nameof(prosaPosteoService));
        }



        public async Task<FileParseResult> ProcessFileAsync(string filePath)
        {
            try
            {
                // Validar si el archivo ya fue procesado
                bool alreadyProcessed = await _prosaPosteoService.IsFileAlreadyProcessed(
                    Path.GetFileName(filePath));

                if (alreadyProcessed)
                {
                    return new FileParseResult
                    {
                        Success = false,
                        ErrorMessage = "El archivo ya ha sido procesado anteriormente"
                    };
                }

                // Leer y procesar el archivo
                string[] lines = await ReadFileLinesAsync(filePath);

                int totalRecords = lines.Length - 2; // Restar HEADER y TRAILER
                int loadedRecords = 0;

                // Procesar HEADER (primera línea)
                var header = ParseHeaderLine(lines[0]);

                //// Registrar archivo
                //int archivoId = await RegisterFileProcessing(
                //    Path.GetFileName(filePath),
                //    totalRecords);
                ArchivosProcesadosRecord archivosProcesadosRecord = new ArchivosProcesadosRecord()
                {
                     NombreArchivo = Path.GetFileName(filePath),
                     TotalRegistros = totalRecords,
                     FechaProcesamiento = DateTime.Now,

                };
                DetailRecord detalle = new DetailRecord();
                List<DetailRecord> detailRecords = new List<DetailRecord>();
                DetailEMVRecord emvDetail = new DetailEMVRecord();
                List<DetailEMVRecord> detalleEMVRecords = new List<DetailEMVRecord>();

                // Procesar líneas de detalle
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    if ((lines[i].Substring(24, 1).Trim() == "D" || lines[i].Substring(24, 1).Trim() == "C"))
                    {
                        //Es Detalle
                        detalle = ParseDetailLine(lines[i]);
                        detailRecords.Add(detalle);
                    }
                    else if( detalle.NumeroAutorizacion == lines[i].Substring(0, 6))
                    {
                        //Es EMVLine
                        emvDetail = ParseEMVLine(lines[i]);
                        detalleEMVRecords.Add(emvDetail);
                    }
                    loadedRecords++;
                    
                }
                archivosProcesadosRecord.RegistrosCargados = loadedRecords;

                // Procesar TRAILER (última línea)
                var trailer = ParseTrailerLine(lines[lines.Length - 1]);
                
                

                //prosaPosteoService.InsertAllRecordsTransactional(archivosProcesadosRecord, header, detailRecords, detalleEMVRecords, trailer);
                _prosaPosteoService.InsertAllRecordsTransactional(archivosProcesadosRecord, header, detailRecords, detalleEMVRecords, trailer);


                return new FileParseResult
                {
                    Success = true,
                    //TotalRecords = totalRecords,
                    //LoadedRecords = loadedRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar el archivo {filePath}");
                return new FileParseResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        public override bool CanParse(string fileExtension) =>
    fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        public override async Task<FileParseResult> ParseAsync(string filePath)
        {
            return await ProcessFileAsync(filePath);
        }
        public async Task<bool> IsFileAlreadyProcessed(string fileName)
        {
            return await _prosaPosteoService.IsFileAlreadyProcessed(fileName);
        }

        private HeaderRecord ParseHeaderLine(string line)
        {
            if (line.Length < 210) throw new FormatException("La línea HEADER es demasiado corta");

            return new HeaderRecord
            {
                TipoRegistro = line.Substring(0, 10).Trim(),
                InstitucionGenera = line.Substring(11, 20).Trim(),
                InstitucionRecibe = line.Substring(32, 20).Trim(),
                LeyendaFijaFecha = line.Substring(53, 18).Trim(),
                FechaProceso = line.Substring(72, 6),
                NumeroConsecutivo = line.Substring(92, 6),
                CaracteristicasArchivo = line.Substring(113, 10).Trim()
            };
        }
               
        private static DetailRecord ParseDetailLine(string line)
        {
            if (line.Length < 510) line = line.PadRight(510);

            var record = new DetailRecord
            {
                BancoEmisor = line.Substring(0, 5).Trim(),
                NumeroCuenta = line.Substring(5, 19).Trim(),
                NaturalezaContable = line.Substring(24, 1).Trim(),
                MarcaProducto = line.Substring(25, 1).Trim(),
                FechaConsumo = line.Substring(28, 6).Trim(),
                HoraConsumo = line.Substring(34, 6).Trim(),
                FechaProceso = line.Substring(40, 6).Trim(),
                TipoTransaccion = line.Substring(46, 2).Trim(),
                NumeroLiquidacion = line.Substring(48, 2).Trim(),
                ImporteOrigenTotal = ParseDecimal(line.Substring(50, 13), 11, 2),
                ImporteOrigenConsumo = ParseDecimal(line.Substring(63, 13), 11, 2),
                ClaveMonedaOrigen = line.Substring(76, 3).Trim(),
                ImporteDestinoTotal = ParseDecimal(line.Substring(79, 13), 11, 2),
                ImporteDestinoConsumo = ParseDecimal(line.Substring(92, 13), 11, 2),
                ClaveMonedaDestino = line.Substring(105, 3).Trim(),
                ParidadDestino = ParseDecimal(line.Substring(108, 7), 3, 4),
                ImporteLiquidacionTotal = ParseDecimal(line.Substring(115, 13), 11, 2),
                ImporteLiquidacionConsumo = ParseDecimal(line.Substring(128, 13), 11, 2),
                ClaveMonedaLiquidacion = line.Substring(141, 3).Trim(),
                ParidadLiquidacion = ParseDecimal(line.Substring(144, 7), 3, 4),
                ClaveComercio = line.Substring(242, 15).Trim(),
                MCCGiroComercio = line.Substring(257, 5).Trim(),
                NombreComercio = line.Substring(262, 30).Trim(),
                DireccionComercio = line.Substring(292, 40).Trim(),
                PaisOrigenTx = line.Substring(332, 3).Trim(),
                CodigoPostal = line.Substring(335, 10).Trim(),
                PoblacionComercio = line.Substring(345, 13).Trim(),
                PorcentajeCuotaIntercambio = ParseDecimal(line.Substring(358, 5)),
                FamiliaComercio = line.Substring(363, 2).Trim(),
                RFCComercio = line.Substring(365, 13).Trim(),
                EstatusComercio = line.Substring(378, 2).Trim(),
                NumeroFuente = line.Substring(380, 5).Trim(),
                NumeroAutorizacion = line.Substring(385, 6).Trim(),
                BancoReceptor = line.Substring(391, 5).Trim(),
                ReferenciaTransaccion = line.Substring(396, 23).Trim(),
                ModoAutorizacion = line.Substring(419, 1).Trim(),
                IndicadorMedioAcceso = line.Substring(420, 2).Trim(),
                Diferimiento = line.Substring(422, 2).Trim(),
                Parcializacion = line.Substring(424, 2).Trim(),
                TipoPlan = line.Substring(426, 2).Trim(),
                IndicadorCobroAutomatico = line.Substring(448, 1).Trim(),
                FIIDEmisor = line.Substring(449, 4).Trim(),
                IndicadorDatosCompletosTrack2 = line.Substring(464, 1).Trim(),
                IndicadorComercioElectronico = line.Substring(465, 1).Trim(),
                IndicadorColectorAutenticacion = line.Substring(466, 1).Trim(),
                CapacidadTerminal = line.Substring(467, 1).Trim(),
                IndicadorTerminalActiva = line.Substring(468, 1).Trim(),
                TerminalID = line.Substring(469, 10).Trim(),
                ModoEntradaPos = line.Substring(479, 2).Trim(),
                IndicadorCV2 = line.Substring(481, 1).Trim(),
                IndicadorCAVVUCAFAAV = line.Substring(482, 1).Trim(),
                FIIDAdquirente = line.Substring(483, 4).Trim(),
                IndicadorPagoInterbancario = line.Substring(487, 1).Trim(),
                IndicadorPresenciaTH = line.Substring(489, 1).Trim(),
                IndicadorPresenciaTarjeta = line.Substring(490, 1).Trim(),
                MetodoDeIdentificacionTH = line.Substring(491, 1).Trim()
            };

            // Parse optional fields if they contain non-space/non-zero values
            if (line.Substring(151, 17).Trim().Length > 0)
                record.ImporteCuotaIntercambio = ParseDecimal(line.Substring(151, 17), 11, 6);

            if (line.Substring(168, 17).Trim().Length > 0)
                record.IvaCuotaIntercambio = ParseDecimal(line.Substring(168, 17), 11, 6);

            if (line.Substring(185, 13).Trim().Length > 0)
                record.ImporteAplicacionTH = ParseDecimal(line.Substring(185, 13), 11, 2);

            if (line.Substring(198, 13).Trim().Length > 0)
                record.ImporteConsumoAplicacionTH = ParseDecimal(line.Substring(198, 13), 11, 2);

            if (line.Substring(211, 5).Trim().Length > 0)
                record.PorcentajeComisionAplicacionTH = ParseDecimal(line.Substring(211, 5), 3, 2);

            if (line.Substring(428, 8).Trim().Length > 0)
                record.Sobretasa = ParseDecimal(line.Substring(428, 8), 6, 2);

            if (line.Substring(436, 7).Trim().Length > 0)
                record.IvaSobretasa = ParseDecimal(line.Substring(436, 7), 5, 2);

            if (line.Substring(443, 5).Trim().Length > 0)
                record.PorcentajeSobretasa = ParseDecimal(line.Substring(443, 5), 3, 2);

            return record;
        }

        private static decimal ParseDecimal(string value, int integerPlaces = 0, int decimalPlaces = 0)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0m;

            // Handle numeric fields with implied decimal
            if (integerPlaces > 0 || decimalPlaces > 0)
            {
                value = value.PadLeft(integerPlaces + decimalPlaces, '0');
                if (decimalPlaces > 0)
                {
                    value = value.Insert(value.Length - decimalPlaces, CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                }
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return 0m;
        }
        private DetailEMVRecord ParseEMVLine(string line)
        {
            if (line.Length < 250) throw new FormatException("La línea EMV es demasiado corta");

            return new DetailEMVRecord
            {
                // Campo 1: Número de autorización (posiciones 1-6, longitud 6)
                NumeroAutorizacion = line.Substring(0, 6), // Se corrige a 0-based index

                // Campo 2: Número de cuenta (posiciones 7-25, longitud 19)
                NumeroCuenta = line.Substring(6, 19).Trim(),

                // Campo 3: Tipo de registro (posiciones 26-27, longitud 2)
                TipoRegistro = line.Substring(25, 2),

                // Campo 4: Application Cryptogram (posiciones 28-43, longitud 16)
                ApplicationCryptogram = line.Substring(27, 16),

                // Campo 5: Cryptogram Information Data (posiciones 44-45, longitud 2)
                CryptogramInformationData = line.Substring(43, 2),

                // Campo 6: Issuer Application Data (posiciones 46-109, longitud 64)
                IssuerApplicationData = line.Length > 45 ? line.Substring(45, 64) : string.Empty,

                // Campo 7: Unpredictable Number (posiciones 110-117, longitud 8)
                UnpredictableNumber = line.Length > 109 ? line.Substring(109, 8) : string.Empty,

                // Campo 8: Application Transaction Counter (posiciones 118-121, longitud 4)
                ApplicationTransactionCounter = line.Length > 117 ? line.Substring(117, 4) : string.Empty,

                // Campo 9: Terminal Verification Result (posiciones 122-131, longitud 10)
                TerminalVerificationResult = line.Length > 121 ? line.Substring(121, 10) : string.Empty,

                // Campo 10: Transaction Date (posiciones 132-137, longitud 6)
                TransactionDate = line.Length > 131 ? line.Substring(131, 6) : string.Empty,

                // Campo 11: Transaction Type (posiciones 138-139, longitud 2)
                TransactionType = line.Length > 137 ? line.Substring(137, 2) : string.Empty,

                // Campo 12: Amount Authorized (posiciones 140-151, longitud 12)
                AmountAuthorized = line.Length > 139 ? line.Substring(139, 12) : string.Empty,

                // Campo 13: Transaction Currency Code (posiciones 152-155, longitud 4)
                TransactionCurrencyCode = line.Length > 151 ? line.Substring(151, 4) : string.Empty,

                // Campo 14: Application Interchange Profile (posiciones 156-159, longitud 4)
                ApplicationInterchangeProfile = line.Length > 155 ? line.Substring(155, 4) : string.Empty,

                // Campo 15: Terminal Country Code (posiciones 160-163, longitud 4)
                TerminalCountryCode = line.Length > 159 ? line.Substring(159, 4) : string.Empty,

                // Campo 16: Amount Other (posiciones 164-175, longitud 12)
                AmountOther = line.Length > 163 ? line.Substring(163, 12) : string.Empty,

                // Campo 17: Cardholder Verification Method (posiciones 176-181, longitud 6)
                CardholderVerificationMethod = line.Length > 175 ? line.Substring(175, 6) : string.Empty,

                // Campo 18: Terminal Capabilities (posiciones 182-189, longitud 8)
                TerminalCapabilities = line.Length > 181 ? line.Substring(181, 8) : string.Empty,

                // Campo 19: Terminal Type (posiciones 190-191, longitud 2)
                TerminalType = line.Length > 189 ? line.Substring(189, 2) : string.Empty,

                // Campo 20: Interface Device Serial Number (posiciones 192-199, longitud 8)
                InterfaceDeviceSerialNumber = line.Length > 191 ? line.Substring(191, 8) : string.Empty,

                // Campo 21: Dedicated File Name (posiciones 200-231, longitud 32)
                DedicatedFileName = line.Length > 199 ? line.Substring(199, 32) : string.Empty,

                // Campo 22: Terminal Application Version Number (posiciones 232-235, longitud 4)
                TerminalApplicationVersionNumber = line.Length > 231 ? line.Substring(231, 4) : string.Empty,

                // Campo 23: Issuer Authentication Data (posiciones 236-267, longitud 32)
                IssuerAuthenticationData = line.Length > 235 ? line.Substring(235, 32) : string.Empty
            };
        }
        //private DetailEMVRecord ParseEMVLine(string line)
        //{
        //    if (line.Length < 250) throw new FormatException("La línea EMV es demasiado corta");

        //    return new DetailEMVRecord
        //    {
        //        NumeroAutorizacion = line.Substring(1, 6),
        //        NumeroCuenta = line.Substring(7, 19).Trim(),
        //        TipoRegistro = line.Substring(26, 2),
        //        ApplicationCryptogram = line.Substring(28, 16),
        //        CryptogramInformationData = line.Substring(44, 2),
        //        IssuerApplicationData = line.Length > 46 ? line.Substring(46, 64) : string.Empty,
        //        UnpredictableNumber = line.Length > 110 ? line.Substring(110, 8) : string.Empty,
        //        ApplicationTransactionCounter = line.Length > 118 ? line.Substring(118, 4) : string.Empty,
        //        TerminalVerificationResult = line.Length > 122 ? line.Substring(122, 10) : string.Empty,
        //        TransactionDate = line.Length > 132 ? line.Substring(132, 6) : string.Empty,
        //        TransactionType = line.Length > 138 ? line.Substring(138, 2) : string.Empty,
        //        AmountAuthorized = line.Length > 140 ? line.Substring(140, 12) : string.Empty,
        //        TransactionCurrencyCode = line.Length > 152 ? line.Substring(152, 4) : string.Empty,
        //        ApplicationInterchangeProfile = line.Length > 156 ? line.Substring(156, 4) : string.Empty,
        //        TerminalCountryCode = line.Length > 160 ? line.Substring(160, 4) : string.Empty,
        //        AmountOther = line.Length > 164 ? line.Substring(164, 12) : string.Empty,
        //        CardholderVerificationMethod = line.Length > 176 ? line.Substring(176, 6) : string.Empty,
        //        TerminalCapabilities = line.Length > 182 ? line.Substring(182, 8) : string.Empty,
        //        TerminalType = line.Length > 190 ? line.Substring(190, 2) : string.Empty,
        //        InterfaceDeviceSerialNumber = line.Length > 192 ? line.Substring(192, 8) : string.Empty,
        //        DedicatedFileName = line.Length > 200 ? line.Substring(200, 32) : string.Empty,
        //        TerminalApplicationVersionNumber = line.Length > 232 ? line.Substring(232, 4) : string.Empty,
        //        IssuerAuthenticationData = line.Length > 236 ? line.Substring(236, 32) : string.Empty
        //    };
        //}

        private TrailerRecord ParseTrailerLine(string line)
        {
            if (line.Length < 216) throw new FormatException("La línea TRAILER es demasiado corta");

            return new TrailerRecord
            {
                TipoRegistro = line.Substring(0, 7),
                TotalTransacciones = ParseIntField(line, 8, 8),
                TotalVentas = ParseIntField(line, 17, 6),
                ImporteVentas = ParseDecimalField(line, 24, 15),
                TotalDisposiciones = ParseIntField(line, 40, 6),
                ImporteDisposiciones = ParseDecimalField(line, 47, 15),
                TotalDebitos = ParseIntField(line, 63, 6),
                ImporteDebitos = ParseDecimalField(line, 70, 15),
                TotalPagosInterbancarios = ParseIntField(line, 86, 6),
                ImportePagosInterbancarios = ParseDecimalField(line, 93, 15),
                TotalDevoluciones = ParseIntField(line, 109, 6),
                ImporteDevoluciones = ParseDecimalField(line, 116, 15),
                TotalCreditos = ParseIntField(line, 132, 6),
                ImporteCreditos = ParseDecimalField(line, 139, 15),
                TotalRepresentaciones = ParseIntField(line, 155, 6),
                ImporteRepresentaciones = ParseDecimalField(line, 162, 15),
                TotalContracargos = ParseIntField(line, 178, 6),
                ImporteContracargos = ParseDecimalField(line, 185, 15),
                TotalComisiones = ParseDecimalField(line, 201, 15)
            };
        }

        private int ParseIntField(string line, int start, int length)
        {
            var field = line.Substring(start, length).Replace(" ", "");

            if (string.IsNullOrWhiteSpace(field) || field.All(c => c == '0'))
            {
                return 0;
            }

            // Intenta parsear como decimal primero (por si acaso)
            if (decimal.TryParse(field, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                return (int)decimalValue;
            }

            _logger.LogWarning($"No se pudo parsear el campo entero: '{field}'");
            return 0;
        }

        private decimal ParseDecimalField(string line, int start, int length)
        {
            var field = line.Substring(start, length).Replace(" ", ""); // Elimina todos los espacios

            if (string.IsNullOrWhiteSpace(field) || field.All(c => c == '0'))
            {
                return 0m;
            }

            if (decimal.TryParse(field, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result / 100;
            }

            _logger.LogWarning($"No se pudo parsear el campo decimal: '{field}'");
            return 0m;
        }
        //private TrailerRecord ParseTrailerLine(string line)
        //{
        //    if (line.Length < 200) throw new FormatException("La línea TRAILER es demasiado corta");

        //    return new TrailerRecord
        //    {
        //        TipoRegistro = line.Substring(0, 7),
        //        TotalTransacciones = int.Parse(line.Substring(8, 8)),
        //        TotalVentas = int.Parse(line.Substring(17, 6)),
        //        ImporteVentas = decimal.Parse(line.Substring(24, 15)) / 100,
        //        TotalDisposiciones = int.Parse(line.Substring(40, 6)),
        //        ImporteDisposiciones = decimal.Parse(line.Substring(47, 15)) / 100,
        //        TotalDebitos = int.Parse(line.Substring(63, 6)),
        //        ImporteDebitos = decimal.Parse(line.Substring(70, 15)) / 100,
        //        TotalPagosInterbancarios = int.Parse(line.Substring(86, 6)),
        //        ImportePagosInterbancarios = decimal.Parse(line.Substring(93, 15)) / 100,
        //        TotalDevoluciones = int.Parse(line.Substring(109, 6)),
        //        ImporteDevoluciones = decimal.Parse(line.Substring(116, 15)) / 100,
        //        TotalCreditos = int.Parse(line.Substring(132, 6)),
        //        ImporteCreditos = decimal.Parse(line.Substring(139, 15)) / 100,
        //        TotalRepresentaciones = int.Parse(line.Substring(155, 6)),
        //        ImporteRepresentaciones = decimal.Parse(line.Substring(162, 15)) / 100,
        //        TotalContracargos = int.Parse(line.Substring(178, 6)),
        //        ImporteContracargos = decimal.Parse(line.Substring(185, 15)) / 100,
        //        TotalComisiones = decimal.Parse(line.Substring(201, 15)) / 100
        //    };
        //}
    }

}
