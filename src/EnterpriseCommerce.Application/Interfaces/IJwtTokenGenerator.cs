using EnterpriseCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
