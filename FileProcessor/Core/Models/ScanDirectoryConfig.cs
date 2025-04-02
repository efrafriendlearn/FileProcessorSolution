using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models
{
    public class ScanDirectoryConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string[] FilePatterns { get; set; }
        public string ProcessedDirectory { get; set; }
        public string ErrorDirectory { get; set; }
    }

    public class FileProcessorConfig
    {
        public ScanDirectoryConfig[] ScanDirectories { get; set; }
        public int ScanIntervalMinutes { get; set; }
        public int MaxParallelProcesses { get; set; }
    }
}
