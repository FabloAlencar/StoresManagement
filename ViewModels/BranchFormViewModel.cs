using StoresManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class BranchFormViewModel
    {
        public IEnumerable<Entity> Entities { get; set; }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        [Display(Name = "Branch Identification")]
        public string Identification { get; set; }

        public char? Type { get; set; }

        [Display(Name = "Branch Name")]
        public string Name { get; set; }

        public virtual Contact Contact { get; set; }

        public string Address { get; set; }
    }
}