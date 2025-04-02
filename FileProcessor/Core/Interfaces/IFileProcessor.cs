using FileProcessor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Interfaces
{
    public interface IFileProcessor
    {
        /// <summary>
        /// Verifica si el archivo ya fue procesado.
        /// </summary>
        Task<bool> IsFileAlreadyProcessed(string filePath);

        /// <summary>
        /// Procesa el archivo y extrae la información necesaria.
        /// </summary>
        Task<FileParseResult> ProcessFileAsync(string filePath);
    }
}
