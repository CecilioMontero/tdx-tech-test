using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using FileProcessorTDXTechTestData;
using FileProcessorTDXTechTestData.Models;
using Microsoft.Extensions.Configuration;
using Z.Dapper.Plus;

namespace FileProcessorTDXTechTestServices
{
    public class OrderServices : IOrderRepository
    {
        private readonly IConfiguration _config;

        public OrderServices(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection => new SqlConnection(_config.GetConnectionString("AppConnection"));

        public async Task<bool> OrderExistsAlready(List<Guid> rowOrderIds)
        {
            using (IDbConnection dbConnection = Connection)
            {
                const string sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Orders WHERE OrderId in @Id) THEN 1 ELSE 0 END as BIT)";
                return await dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = rowOrderIds });
            }           
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<Order>("SELECT * FROM Orders");
            }
        }

        public void StoreEmployeeDetails(List<Order> orders)
        {
            DapperPlusManager.Entity<Order>().Table("Orders");

            using (var connection = new SqlConnection(Connection.ConnectionString))
            {
                connection.BulkInsert(orders);
            }
        }
    }
}
