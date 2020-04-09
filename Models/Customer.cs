using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.Models
{
    public class Customer
    {
        public Customer()
        {
            Purchases = new HashSet<Purchase>();
        }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public string Identification { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }

        [Display(Name = "Customer Name")]
        public string FullName
        {
            get { return Surname + ", " + Name; }
        }
    }
}