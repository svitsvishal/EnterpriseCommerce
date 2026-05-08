using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Common.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
            = Enumerable.Empty<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
