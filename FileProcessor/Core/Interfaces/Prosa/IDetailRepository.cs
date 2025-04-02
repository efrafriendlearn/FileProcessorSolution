using FileProcessor.Core.Models.Prosa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FileProcessor.Core.Interfaces.Prosa
{
    public interface IDetailRepository
    {
        public bool InsertDetailRecords(List<DetailRecord> records, IDbTransaction transaction);


    }
}
