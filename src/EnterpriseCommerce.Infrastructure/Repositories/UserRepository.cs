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
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(User user)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.ExecuteAsync(
                "sp_CreateUser",
                user,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            using IDbConnection connection =
                _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
