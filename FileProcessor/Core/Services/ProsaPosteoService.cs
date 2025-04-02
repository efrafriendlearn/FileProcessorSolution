using FileProcessor.Core.Models.Prosa;
using FileProcessor.Infrastructure.Data.Prosa;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FileProcessor.Core.Interfaces.Prosa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Services
{
    public class ProsaPosteoService
    {

        //private readonly ArchivosProcesadosRepository _archivosProcesadosRepository; // Inyectar el nuevo repositorio
        private readonly IHeaderRepository _headerRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly IDetailEMVRepository _detailEMVRepository;
        private readonly ITrailerRepository _trailerRepository;
        private readonly string _connectionString;
        private IConfiguration configuration;
        private readonly IArchivosProcesadosRepository _archivosProcesadosRepository;
        private readonly ILogger<ProsaPosteoService> _logger;
        private readonly IConfiguration _configuration;



        //public ProsaPosteoService(
        //    //ArchivosProcesadosRepository archivosProcesadosRepository, // Inyectar el nuevo repositorio
        //    //HeaderRepository headerRepository,
        //    //DetailRepository detailRepository,
        //    //DetailEMVRepository detailEMVRepository,
        //    //TrailerRepository trailerRepository,
        //    //IConfiguration configuration)
        //    )
        //{
        //    //_archivosProcesadosRepository = archivosProcesadosRepository;
        //    //_headerRepository = headerRepository;
        //    //_detailRepository = detailRepository;
        //    //_detailEMVRepository = detailEMVRepository;
        //    //_trailerRepository = trailerRepository;
        //    _connectionString = configuration.GetConnectionString("DataBase");
        //}
        public ProsaPosteoService(
        IArchivosProcesadosRepository archivosProcesadosRepository,
        IHeaderRepository headerRepository,
        IDetailRepository detailRepository,
        IDetailEMVRepository detailEMVRepository,
        ITrailerRepository trailerRepository,
        ILogger<ProsaPosteoService> logger, IConfiguration configuration)
        {
            _archivosProcesadosRepository = archivosProcesadosRepository ?? throw new ArgumentNullException(nameof(archivosProcesadosRepository));
            _headerRepository = headerRepository ?? throw new ArgumentNullException(nameof(headerRepository));
            _detailRepository = detailRepository ?? throw new ArgumentNullException(nameof(detailRepository));
            _detailEMVRepository = detailEMVRepository ?? throw new ArgumentNullException(nameof(detailEMVRepository));
            _trailerRepository = trailerRepository ?? throw new ArgumentNullException(nameof(trailerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetConnectionString("DataBase")
                ?? throw new ArgumentNullException("Connection string 'DataBase' not found in configuration");

        }
        public async Task<bool> IsFileAlreadyProcessed(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(fileName));
            }

            try
            {
                return await _archivosProcesadosRepository.IsFileAlreadyProcessed(fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el archivo ya fue procesado: {FileName}", fileName);
                throw;
            }
        }


        public bool InsertAllRecordsTransactional(
            ArchivosProcesadosRecord archivosProcesadosRecord, // Recibir el registro de ArchivosProcesados
            HeaderRecord headerRecord,
            List<DetailRecord> detailRecords,
            List<DetailEMVRecord> detalleEMVRecords,
            TrailerRecord trailerRecord)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar en ArchivosProcesados y obtener el ArchivoId
                        int archivoId = _archivosProcesadosRepository.InsertArchivosProcesadosRecord(archivosProcesadosRecord, transaction);
                        if (archivoId == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Asignar el ArchivoId a los registros relacionados
                        headerRecord.ArchivoId= archivoId;
                        detailRecords.ForEach(d => d.ArchivoId = archivoId);
                        detalleEMVRecords.ForEach(e => e.DetalleId = archivoId); // Asumiendo que DetalleId se relaciona con ArchivoId
                        trailerRecord.ArchivoId = archivoId;

                        // Insertar en Header
                        if (!_headerRepository.InsertHeaderRecords(headerRecord, transaction))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Insertar en Detail
                        if (!_detailRepository.InsertDetailRecords(detailRecords, transaction))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Insertar en DetalleEMV
                        if (!_detailEMVRepository.InsertDetalleEMVRecords(detalleEMVRecords, transaction))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Insertar en Trailer
                        if (!_trailerRepository.InsertTrailerRecords(trailerRecord, transaction))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
