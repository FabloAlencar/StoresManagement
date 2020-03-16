using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.Models
{
    public class Branch
    {
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        [Display(Name = "Branch Identification")]
        public string Identification { get; set; }

        [Display(Name = "Branch Name")]
        public string Name { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}