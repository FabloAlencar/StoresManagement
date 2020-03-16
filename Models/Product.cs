using System;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.Models
{
    public class Product
    {
        public int EntityId { get; set; }

        [Display(Name = "Branch Id")]
        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public float? Price { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        public float? Weight { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }

        [Display(Name = "Quantity In Stock")]
        public int? QuantityInStock { get; set; }
    }
}