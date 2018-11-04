using System;

namespace FileProcessorTDXTechTestData.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string Country { get; set; }
        public string ItemType { get; set; }
        public DateTime OrderDate { get; set; }
        public int UnitsSold { get; set; }
        public double UnitPrice { get; set; }
    }
}
