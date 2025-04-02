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
    public class HeaderRepository: IHeaderRepository
    {
        
        public bool InsertHeaderRecords(HeaderRecord record, IDbTransaction transaction)
        {
            try
            {
                var sql = @"
                    INSERT INTO Header (
                        ArchivoId, TipoRegistro, InstitucionGenera, InstitucionRecibe, FechaProceso, 
                        NumeroConsecutivo, CaracteristicasArchivo
                    ) VALUES (
                        @ArchivoId, @TipoRegistro, @InstitucionGenera, @InstitucionRecibe, @FechaProceso, 
                        @NumeroConsecutivo, @CaracteristicasArchivo
                    );";

                transaction.Connection.Execute(sql, record, transaction);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}
