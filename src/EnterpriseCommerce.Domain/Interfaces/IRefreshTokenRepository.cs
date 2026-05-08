using EnterpriseCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetByTokenAsync(string token);

        Task RevokeAsync(string token);
    }
}
