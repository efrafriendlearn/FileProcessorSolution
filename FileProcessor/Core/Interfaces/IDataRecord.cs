using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Core.Interfaces
{
    public interface IDataRecord
    {
        Dictionary<string, object> ToDictionary();
    }
}