using StoresManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class PurchaseFormViewModel
    {
        public PurchaseFormViewModel()
        {
            Branches = new List<Branch>();
            Customers = new List<Customer>();
            Products = new List<Product>();
            PurchaseItems = new List<PurchaseItem>();
        }

        public IEnumerable<Branch> Branches { get; set; }

        public IEnumerable<Customer> Customers { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public int EntityId { get; set; }

        public int? BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Product Product { get; set; }

        public int Id { get; set; }

        public string Identification { get; set; }

        [Display(Name = "Purchase Number")]
        public string MaskedIdentification()
        {
            return Identification.Substring(1, 8);
        }

        [Display(Name = "DISCOUNT")]
        public decimal? Discount { get; set; }

        [Display(Name = "TOTAL")]
        public decimal? Total { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }

        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Purchase Date")]
        public string PurchaseDate => RegistrationDate.ToString("dd/MMM/yyyy HH:mm");
    }
}