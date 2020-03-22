using System;

namespace StoresManagement.Models
{
    public class Product
    {
        public int EntityId { get; set; }

        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? QuantityInStock { get; set; }

        public float? Price { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public float? Weight { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }

        public virtual PurchaseItem PurchaseItem { get; set; }
    }
}