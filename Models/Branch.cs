using System.Collections.Generic;

namespace StoresManagement.Models
{
    public class Branch
    {
        public Branch()
        {
            Purchases = new HashSet<Purchase>();
        }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public string Identification { get; set; }

        public string Name { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}