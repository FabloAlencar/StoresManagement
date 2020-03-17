using StoresManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoresManagement.ViewModels
{
    public class ProductFormViewModel
    {
        public int EntityId { get; set; }

        public IEnumerable<Branch> Branches { get; set; }

        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Quantity In Stock")]
        public int? QuantityInStock { get; set; }

        public float? Price { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public float? Weight { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }

        [Display(Name = "Branch")]
        public string BranchTitle
        {
            get { return Branch.Entity.Name + ", " + Branch.Name; }
        }

        [Display(Name = "Expiry Date")]
        public string ExpiryDay => String.Format("{0:dd/MM/yyyy}", ExpiryDate);
    }
}