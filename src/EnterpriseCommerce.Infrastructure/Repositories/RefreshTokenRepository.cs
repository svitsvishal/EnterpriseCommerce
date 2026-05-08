using Dapper;
using EnterpriseCommerce.Domain.Entities;
using EnterpriseCommerce.Domain.Interfaces;
using EnterpriseCommerce.Infrastructure.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EnterpriseCommerce.Infrastructure.Repositories
{
    public class RefreshTokenRepository
     : IRefreshTokenRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public RefreshTokenRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(RefreshToken refreshToken)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                "sp_CreateRefreshToken",
                refreshToken,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RefreshToken?> GetByTokenAsync(
            string token)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection
                .QueryFirstOrDefaultAsync<RefreshToken>(
                    "sp_GetRefreshToken",
                    new { Token = token },
                    commandType: CommandType.StoredProcedure);
        }

        public async Task RevokeAsync(string token)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                "sp_RevokeRefreshToken",
                new { Token = token },
                commandType: CommandType.StoredProcedure);
        }
    }
}
