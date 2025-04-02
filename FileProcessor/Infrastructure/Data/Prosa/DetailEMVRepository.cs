using FileProcessor.Core.Models.Prosa;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using FileProcessor.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using FileProcessor.Core.Interfaces.Prosa;

namespace FileProcessor.Infrastructure.Data.Prosa
{
    public class DetailEMVRepository:IDetailEMVRepository
    {
        public bool InsertDetalleEMVRecords(List<DetailEMVRecord> records, IDbTransaction transaction)
        {
            try
            {
                var sql = @"
                    INSERT INTO DetalleEMV (
                        DetalleId, NumeroAutorizacion, NumeroCuenta, TipoRegistro, ApplicationCryptogram, 
                        CryptogramInformationData, IssuerApplicationData, UnpredictableNumber, 
                        ApplicationTransactionCounter, TerminalVerificationResult, TransactionDate, 
                        TransactionType, AmountAuthorized, TransactionCurrencyCode, 
                        ApplicationInterchangeProfile, TerminalCountryCode, AmountOther, 
                        CardholderVerificationMethod, TerminalCapabilities, TerminalType, 
                        InterfaceDeviceSerialNumber, DedicatedFileName, 
                        TerminalApplicationVersionNumber, IssuerAuthenticationData
                    ) VALUES (
                        @DetalleId, @NumeroAutorizacion, @NumeroCuenta, @TipoRegistro, @ApplicationCryptogram, 
                        @CryptogramInformationData, @IssuerApplicationData, @UnpredictableNumber, 
                        @ApplicationTransactionCounter, @TerminalVerificationResult, @TransactionDate, 
                        @TransactionType, @AmountAuthorized, @TransactionCurrencyCode, 
                        @ApplicationInterchangeProfile, @TerminalCountryCode, @AmountOther, 
                        @CardholderVerificationMethod, @TerminalCapabilities, @TerminalType, 
                        @InterfaceDeviceSerialNumber, @DedicatedFileName, 
                        @TerminalApplicationVersionNumber, @IssuerAuthenticationData
                    );";

                transaction.Connection.Execute(sql, records, transaction);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
