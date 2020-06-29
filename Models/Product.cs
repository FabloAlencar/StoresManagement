using System;
using System.Collections.Generic;

namespace StoresManagement.Models
{
    public class Product
    {
        public Product()
        {
            PurchaseItems = new HashSet<PurchaseItem>();
        }

        public int EntityId { get; set; }

        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public int? QuantityInStock { get; set; }

        public decimal Price { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }

        public string ProductTitle
        {
            get { return Name + ", " + Brand; }
        }
    }
}