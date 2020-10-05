using StoresManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class CustomerFormViewModel
    {
        public CustomerFormViewModel()
        {
            Entities = new List<Entity>();
        }

        public IEnumerable<Entity> Entities { get; set; }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Customer Identification")]
        public string Identification { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public virtual Contact Contact { get; set; }

        [Display(Name = "Customer Name")]
        public string FullName { get; set; }

        public string Address { get; set; }
    }
}