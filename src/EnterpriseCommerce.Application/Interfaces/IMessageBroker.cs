using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Interfaces
{
    public interface IMessageBroker
    {
        void Publish<T>(T message);
    }
}
