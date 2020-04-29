using System.Collections.Generic;

namespace Dashboard.Types
{
    public class Summary
    {
        public Customer Customer { get; set; }

        public List<Order> Orders { get; set; }

        public List<Claim> Claims { get; set; }
    }
}
