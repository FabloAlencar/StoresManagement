using StoresManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class ProductFormViewModel
    {
        public IEnumerable<Branch> Branches { get; set; }

        public int EntityId { get; set; }

        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        [Display(Name = "Product")]
        public string ProductTitle { get; set; }

        [Display(Name = "Quantity In Stock")]
        public int? QuantityInStock { get; set; }

        public decimal? Price { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        [Display(Name = "Branch")]
        public string BranchTitle { get; set; }

        [Display(Name = "Expiry Date")]
        public string ExpiryDay => String.Format("{0:dd/MM/yyyy}", ExpiryDate);
    }
}