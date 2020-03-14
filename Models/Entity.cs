using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.Models
{
    public class Entity
    {
        public Entity()
        {
            Branches = new HashSet<Branch>();
        }

        public int Id { get; set; }

        [Display(Name = "Entity Name")]
        public string Name { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
    }
}