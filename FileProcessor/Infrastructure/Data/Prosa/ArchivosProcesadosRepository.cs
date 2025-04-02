using FileProcessor.Core.Models.Prosa;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using FileProcessor.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FileProcessor.Core.Interfaces.Prosa;

namespace FileProcessor.Infrastructure.Data.Prosa
{
    public class ArchivosProcesadosRepository:IArchivosProcesadosRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        // Constructor modificado para recibir IConfiguration
        public ArchivosProcesadosRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetConnectionString("DataBase")
                ?? throw new ArgumentNullException("Connection string 'DataBase' not found in configuration");
        }
        public async Task<bool> IsFileAlreadyProcessed(string fileName)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"SELECT 1 FROM ArchivosProcesados 
                       WHERE NombreArchivo = @fileName";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { fileName });

            return result.HasValue;
        }
        public async Task<int> RegisterFileProcessing(string fileName, int totalRecords)
        {
            

            using var connection = new SqlConnection(_connectionString);

            var sql = @"INSERT INTO ArchivosProcesados 
                       (NombreArchivo, FechaProcesamiento, TotalRegistros, RegistrosCargados)
                       OUTPUT INSERTED.ArchivoId
                       VALUES (@fileName, GETDATE(), @totalRecords, 0)";

            try
            {
                return await connection.ExecuteScalarAsync<int>(sql, new { fileName, totalRecords });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error al registrar el archivo para procesamiento");
                throw;
            }
        }
        public int InsertArchivosProcesadosRecord(ArchivosProcesadosRecord record, IDbTransaction transaction)
        {
            try
            {
                var sql = @"
                    INSERT INTO ArchivosProcesados (
                        NombreArchivo, FechaProcesamiento, TotalRegistros, RegistrosCargados
                    ) VALUES (
                        @NombreArchivo, @FechaProcesamiento, @TotalRegistros, @RegistrosCargados
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);"; // Obtener el ID insertado

                return transaction.Connection.QuerySingle<int>(sql, record, transaction);
            }
            catch (Exception)
            {
                return 0; // O lanzar una excepción, dependiendo de tu manejo de errores
            }
        }
    }
}
