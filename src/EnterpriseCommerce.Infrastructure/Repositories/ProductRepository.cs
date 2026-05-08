using EnterpriseCommerce.Domain.Entities;
using EnterpriseCommerce.Domain.Interfaces;
using EnterpriseCommerce.Infrastructure.Connections;
//using EnterpriseCommerce.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
//using Microsoft.EntityFrameworkCore;

namespace EnterpriseCommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ProductRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Product>(
                "sp_GetProducts",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Product>(
                "sp_GetProductById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Product product)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.ExecuteAsync(
                "sp_CreateProduct",
                new
                {
                    product.Id,
                    product.Name,
                    product.Price,
                    product.StockQuantity
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAsync(Product product)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            return await Task.FromResult(0);
        }

        public async Task<(IEnumerable<Product>, int TotalCount)>
GetPagedAsync(
    string? search,
    int pageNumber,
    int pageSize)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetPagedProducts",
                new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                },
                commandType: CommandType.StoredProcedure);

            var totalCount =
                await multi.ReadFirstAsync<int>();

            var products =
                await multi.ReadAsync<Product>();

            return (products, totalCount);
        }
    }
}
