using FileProcessor.Core.Interfaces;
using FileProcessor.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Infrastructure.FileParsers
{
    
    public class ExcelFileParser : IFileParser
    {
        private readonly ILogger<ExcelFileParser> _logger;

        public ExcelFileParser(ILogger<ExcelFileParser> logger)
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
                    FileType = "XLS",
                    TotalRecords = lines.Length
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing XLS file");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
