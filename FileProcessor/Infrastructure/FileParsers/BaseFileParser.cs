using FileProcessor.Core.Interfaces;
using FileProcessor.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FileProcessor.Infrastructure.FileParsers
{
    public abstract class BaseFileParser : IFileParser
    {
        protected readonly ILogger<BaseFileParser> _logger;

        protected BaseFileParser(ILogger<BaseFileParser> logger)
        {
            _logger = logger;
        }

        public abstract bool CanParse(string fileExtension);
        public abstract Task<FileParseResult> ParseAsync(string filePath);

        protected bool IsProsaFile(string fileName)
        {
            string pattern = @"^I\d{4}\.B\d{4}EMI(\.TXT)?$";
            return Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase);
        }


        protected async Task<string[]> ReadFileLinesAsync(string filePath)
        {
            return await File.ReadAllLinesAsync(filePath);
        }
    }
}
