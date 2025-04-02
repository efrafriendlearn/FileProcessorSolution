using FileProcessor.Core.Models.Prosa;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using FileProcessor.Core.Models;
using Microsoft.Data.SqlClient;
using FileProcessor.Core.Interfaces.Prosa;

namespace FileProcessor.Infrastructure.Data.Prosa
{
    public class DetailRepository : IDetailRepository
    {
        //private readonly string _connectionString;

        //public DetailRepository(IConfiguration configuration)
        //{
        //    _connectionString = configuration.GetConnectionString("DefaultConnection");
        //}

        public bool InsertDetailRecords(List<DetailRecord> records, IDbTransaction transaction)
        {
            try
            {
                var sql = @"
                    INSERT INTO Detalle (
                        ArchivoId, BancoEmisor, NumeroCuenta, NaturalezaContable, MarcaProducto, FechaConsumo, HoraConsumo, 
                        FechaProceso, TipoTransaccion, NumeroLiquidacion, ImporteOrigenTotal, ImporteOrigenConsumo, 
                        ClaveMonedaOrigen, ImporteDestinoTotal, ImporteDestinoConsumo, ClaveMonedaDestino, ParidadDestino, 
                        ImporteLiquidacionTotal, ImporteLiquidacionConsumo, ClaveMonedaLiquidacion, ParidadLiquidacion, 
                        ImporteCuotaIntercambio, IvaCuotaIntercambio, ImporteAplicacionTH, ImporteConsumoAplicacionTH, 
                        PorcentajeComisionAplicacionTH, ClaveComercio, MCCGiroComercio, NombreComercio, DireccionComercio, 
                        PaisOrigenTx, CodigoPostal, PoblacionComercio, PorcentajeCuotaIntercambio, FamiliaComercio, 
                        RFCComercio, EstatusComercio, NumeroFuente, NumeroAutorizacion, BancoReceptor, ReferenciaTransaccion, 
                        ModoAutorizacion, IndicadorMedioAcceso, Diferimiento, Parcializacion, TipoPlan, Sobretasa, 
                        IvaSobretasa, PorcentajeSobretasa, IndicadorCobroAutomatico, FIIDEmisor, IndicadorDatosCompletosTrack2, 
                        IndicadorComercioElectronico, IndicadorColectorAutenticacion, CapacidadTerminal, IndicadorTerminalActiva, 
                        TerminalID, ModoEntradaPos, IndicadorCV2, IndicadorCAVVUCAFAAV, FIIDAdquirente, IndicadorPagoInterbancario, 
                        CodigoServicio
                    ) VALUES (
                        @ArchivoId, @BancoEmisor, @NumeroCuenta, @NaturalezaContable, @MarcaProducto, @FechaConsumo, @HoraConsumo, 
                        @FechaProceso, @TipoTransaccion, @NumeroLiquidacion, @ImporteOrigenTotal, @ImporteOrigenConsumo, 
                        @ClaveMonedaOrigen, @ImporteDestinoTotal, @ImporteDestinoConsumo, @ClaveMonedaDestino, @ParidadDestino, 
                        @ImporteLiquidacionTotal, @ImporteLiquidacionConsumo, @ClaveMonedaLiquidacion, @ParidadLiquidacion, 
                        @ImporteCuotaIntercambio, @IvaCuotaIntercambio, @ImporteAplicacionTH, @ImporteConsumoAplicacionTH, 
                        @PorcentajeComisionAplicacionTH, @ClaveComercio, @MCCGiroComercio, @NombreComercio, @DireccionComercio, 
                        @PaisOrigenTx, @CodigoPostal, @PoblacionComercio, @PorcentajeCuotaIntercambio, @FamiliaComercio, 
                        @RFCComercio, @EstatusComercio, @NumeroFuente, @NumeroAutorizacion, @BancoReceptor, @ReferenciaTransaccion, 
                        @ModoAutorizacion, @IndicadorMedioAcceso, @Diferimiento, @Parcializacion, @TipoPlan, @Sobretasa, 
                        @IvaSobretasa, @PorcentajeSobretasa, @IndicadorCobroAutomatico, @FIIDEmisor, @IndicadorDatosCompletosTrack2, 
                        @IndicadorComercioElectronico, @IndicadorColectorAutenticacion, @CapacidadTerminal, @IndicadorTerminalActiva, 
                        @TerminalID, @ModoEntradaPos, @IndicadorCV2, @IndicadorCAVVUCAFAAV, @FIIDAdquirente, @IndicadorPagoInterbancario, 
                        @CodigoServicio
                    );";

                transaction.Connection.Execute(sql, records, transaction);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
