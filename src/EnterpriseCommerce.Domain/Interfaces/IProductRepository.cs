using EnterpriseCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(Guid id);

        Task<int> CreateAsync(Product product);

        Task<int> UpdateAsync(Product product);

        Task<int> DeleteAsync(Guid id);

        Task<(IEnumerable<Product>, int TotalCount)>
GetPagedAsync(
    string? search,
    int pageNumber,
    int pageSize);
    }
}
