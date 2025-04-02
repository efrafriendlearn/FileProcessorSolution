using FileProcessor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models
{
    public class FileParseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public List<IDataRecord> Records { get; set; } = new();
        public FileMetadata Metadata { get; set; }
    }
}
