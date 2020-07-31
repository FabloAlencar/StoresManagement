using System;
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

        public string Identification { get; set; }

        public decimal Discount { get; set; }

        public decimal Total { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}