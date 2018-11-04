using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileProcessorTDXTechTest.Models
{
    public class CsvFileRow
    {
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public string ItemType { get; set; }
        public DateTime OrderDate { get; set; }
        public int UnitsSold { get; set; }
        public double UnitPrice { get; set; }
    }
}
