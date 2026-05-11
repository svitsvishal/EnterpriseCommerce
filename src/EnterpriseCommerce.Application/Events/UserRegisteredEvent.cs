using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Events
{
    public class UserRegisteredEvent
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}
