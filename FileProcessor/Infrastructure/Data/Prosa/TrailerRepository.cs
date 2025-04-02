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
    public class TrailerRepository:ITrailerRepository
    {
        public bool InsertTrailerRecords(TrailerRecord records, IDbTransaction transaction)
        {
            try
            {
                var sql = @"
                    INSERT INTO Trailer (
                        ArchivoId, TipoRegistro, TotalTransacciones, TotalVentas, ImporteVentas, 
                        TotalDisposiciones, ImporteDisposiciones, TotalDebitos, ImporteDebitos, 
                        TotalPagosInterbancarios, ImportePagosInterbancarios, TotalDevoluciones, 
                        ImporteDevoluciones, TotalCreditos, ImporteCreditos, TotalRepresentaciones, 
                        ImporteRepresentaciones, TotalContracargos, ImporteContracargos, TotalComisiones
                    ) VALUES (
                        @ArchivoId, @TipoRegistro, @TotalTransacciones, @TotalVentas, @ImporteVentas, 
                        @TotalDisposiciones, @ImporteDisposiciones, @TotalDebitos, @ImporteDebitos, 
                        @TotalPagosInterbancarios, @ImportePagosInterbancarios, @TotalDevoluciones, 
                        @ImporteDevoluciones, @TotalCreditos, @ImporteCreditos, @TotalRepresentaciones, 
                        @ImporteRepresentaciones, @TotalContracargos, @ImporteContracargos, @TotalComisiones
                        
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
