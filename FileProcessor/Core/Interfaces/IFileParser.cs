using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileProcessor.Core.Models;

namespace FileProcessor.Core.Interfaces
{
    public interface IFileParser
    {
        Task<FileParseResult> ParseAsync(string filePath);
        bool CanParse(string fileExtension);
    }
}
