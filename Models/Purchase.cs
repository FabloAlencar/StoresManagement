using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoresManagement.Models
{
    public class Purchase
    {
        public int EntityId { get; set; }

        public int? BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public int Id { get; set; }

        public float? Discount { get; set; }

        public float? Total { get; set; }
    }
}