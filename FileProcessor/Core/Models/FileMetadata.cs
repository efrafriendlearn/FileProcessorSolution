using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Models
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime ProcessDate { get; set; } = DateTime.Now;
        public int TotalRecords { get; set; }
    }
}
