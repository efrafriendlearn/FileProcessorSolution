using FileProcessor.Core.Interfaces;
using FileProcessor.Core.Models;
using FileProcessor.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FileProcessor.Infrastructure.FileParsers
{
    public class TextFileParser : BaseFileParser, IFileParser
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TextFileParser> _logger;
        private readonly ILogger<ProsaTextFileParser> _prosaLogger;
        private readonly ProsaPosteoService _prosaPosteoService;


        public TextFileParser(IConfiguration configuration, ILogger<TextFileParser> logger, ILogger<ProsaTextFileParser> prosaLogger, ProsaPosteoService prosaPosteoService)
     : base(logger)
        {
            _configuration = configuration;
            _logger = logger;
            _prosaLogger = prosaLogger;
            _prosaPosteoService = prosaPosteoService ?? throw new ArgumentNullException(nameof(prosaPosteoService));
        }





        //public override async Task<FileParseResult> ParseAsync(string filePath)
        //{
        //    var prosaTextFileParser = new ProsaTextFileParser(_configuration, _prosaLogger, _prosaPosteoService);
        //    return await prosaTextFileParser.ParseAsync(filePath);
        //}


        public override bool CanParse(string fileExtension) =>
     fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);


        public override async Task<FileParseResult> ParseAsync(string filePath)
        {
            var result = new FileParseResult();
            try
            {
                var lines = await ReadFileLinesAsync(filePath);
                // Implementación específica del parsing...
                var fileName = Path.GetFileName(filePath);

                if (!IsProsaFile(fileName))
                {
                    _logger.LogWarning($"Archivo no válido: {fileName}");
                    result.Success = false;
                    result.Message = "El archivo no tiene el formato esperado para Prosa.";
                    return result;
                }
                else
                {
                    var prosaTextFileParser = new ProsaTextFileParser(_configuration, _prosaLogger, _prosaPosteoService);
                    var parseResult = await prosaTextFileParser.ProcessFileAsync(filePath);

                    if (!parseResult.Success)
                    {
                        _logger.LogWarning($"ProsaTextFileParser falló en procesar el archivo: {filePath}. Error: {parseResult.ErrorMessage}");
                        result.Success = false;
                        result.Message = parseResult.ErrorMessage;
                        return result;
                    }
                }



                result.Metadata = new FileMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    FileType = "TXT",
                    TotalRecords = lines.Length
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing text file");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

    }
}