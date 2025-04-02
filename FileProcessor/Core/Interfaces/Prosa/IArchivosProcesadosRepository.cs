using FileProcessor.Core.Models.Prosa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Interfaces.Prosa
{
    public interface IArchivosProcesadosRepository
    {
        /// <summary>
        /// Verifica si un archivo ya ha sido procesado
        /// </summary>
        /// <param name="fileName">Nombre del archivo a verificar</param>
        /// <returns>True si el archivo ya fue procesado, False en caso contrario</returns>
        Task<bool> IsFileAlreadyProcessed(string fileName);

        ///// <summary>
        ///// Registra un archivo como procesado
        ///// </summary>
        ///// <param name="fileName">Nombre del archivo</param>
        ///// <param name="totalRecords">Total de registros procesados</param>
        ///// <returns>ID del registro creado</returns>
        //Task<int> RegisterFileProcessed(string fileName, int totalRecords);


        public int InsertArchivosProcesadosRecord(ArchivosProcesadosRecord record, IDbTransaction transaction);
    }  
}
