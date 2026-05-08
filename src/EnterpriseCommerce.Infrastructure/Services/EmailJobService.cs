using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Infrastructure.Services
{
    public class EmailJobService
    {
        private readonly ILogger<EmailJobService> _logger;

        public EmailJobService(
            ILogger<EmailJobService> logger)
        {
            _logger = logger;
        }

        public void SendWelcomeEmail(string email)
        {
            _logger.LogInformation(
                "Sending welcome email to {Email}",
                email);
        }
    }
}
