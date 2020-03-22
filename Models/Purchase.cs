using System.Collections.Generic;

namespace StoresManagement.Models
{
    public class Purchase
    {
        public Purchase()
        {
            PurchaseItems = new HashSet<PurchaseItem>();
        }

        public int EntityId { get; set; }

        public int? BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public int Id { get; set; }

        public float? Discount { get; set; }

        public float? Total { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}