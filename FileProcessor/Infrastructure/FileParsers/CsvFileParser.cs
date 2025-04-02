using FileProcessor.Core.Interfaces;
using FileProcessor.Core.Models;
using FileProcessor.Infrastructure.FileParsers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Infrastructure.FileParsers
{
    public class CsvFileParser : IFileParser
    {
        private readonly ILogger<CsvFileParser> _logger;

        public CsvFileParser(ILogger<CsvFileParser> logger)
        {
            _logger = logger;
        }

        public bool CanParse(string fileExtension) =>
            fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);

        public async Task<FileParseResult> ParseAsync(string filePath)
        {
            var result = new FileParseResult();
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                // Implementación específica del parsing...

                result.Metadata = new FileMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    FileType = "CSV",
                    TotalRecords = lines.Length
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing csv file");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
