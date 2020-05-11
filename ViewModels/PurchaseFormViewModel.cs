using StoresManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class PurchaseFormViewModel
    {
        public PurchaseFormViewModel()
        {
            Branches = new HashSet<Branch>();
            Customers = new HashSet<Customer>();
            Products = new HashSet<Product>();
            PurchaseItems = new HashSet<PurchaseItem>();
        }

        public IEnumerable<Branch> Branches { get; set; }

        public IEnumerable<Customer> Customers { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public int EntityId { get; set; }

        public int? BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Id { get; set; }

        public float? Discount { get; set; }

        public float? Total { get; set; }

        [Display(Name = "Branch")]
        public string BranchTitle { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}