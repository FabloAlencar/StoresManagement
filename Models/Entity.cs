using System.Collections.Generic;

namespace StoresManagement.Models
{
    public class Entity
    {
        public Entity()
        {
            Branches = new HashSet<Branch>();
            Customers = new HashSet<Customer>();
            Operators = new HashSet<Operator>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Operator> Operators { get; set; }
    }
}