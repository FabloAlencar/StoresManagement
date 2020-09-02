using System;

namespace StoresManagement.Models
{
    public class PurchaseItem
    {
        public int EntityId { get; set; }

        public int PurchaseId { get; set; }

        public virtual Purchase Purchase { get; set; }

        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Id { get; set; }

        public int ProductQuantity { get; set; }

        public decimal ProductCurrentPrice { get; set; }

        public decimal Total { get; set; }

        public decimal DiscountTotal { get; set; }
    }
}