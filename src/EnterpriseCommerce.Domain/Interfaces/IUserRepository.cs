using EnterpriseCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<int> CreateAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
    }
}
