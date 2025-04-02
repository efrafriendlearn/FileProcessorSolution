using FileProcessor.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileProcessor.Infrastructure.FileParsers
{
    public class FileParserFactory
    {
        private readonly IEnumerable<IFileParser> _parsers;
        private readonly ILogger<FileParserFactory> _logger;

        public FileParserFactory(
            IEnumerable<IFileParser> parsers,
            ILogger<FileParserFactory> logger)
        {
            _parsers = parsers;
            _logger = logger;
        }

        public IFileParser GetParser(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var parser = _parsers.FirstOrDefault(p => p.CanParse(extension));

            if (parser == null)
                throw new NotSupportedException($"File type {extension} is not supported");

            return parser;
        }
    }
}