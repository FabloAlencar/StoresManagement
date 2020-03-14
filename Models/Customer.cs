using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoresManagement.Models
{
    public class Customer
    {
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        [Display(Name = "Customer Identification")]
        public string Identification { get; set; }

        [Display(Name = "Customer Name")]
        public string Name { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
    }
}