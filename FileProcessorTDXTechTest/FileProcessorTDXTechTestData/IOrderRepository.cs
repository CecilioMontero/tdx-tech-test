using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileProcessorTDXTechTestData.Models;

namespace FileProcessorTDXTechTestData
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        void StoreEmployeeDetails(List<Order> fileData);
        Task<bool> OrderExistsAlready(List<Guid> rowOrderId);
    }
}
