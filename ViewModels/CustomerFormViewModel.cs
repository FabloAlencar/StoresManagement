using StoresManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels
{
    public class CustomerFormViewModel
    {
        public IEnumerable<Entity> Entities { get; set; }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        [Display(Name = "Customer Identification")]
        public string Identification { get; set; }

        [Display(Name = "Customer Name")]
        public string Name { get; set; }

        public virtual Contact Contact { get; set; }

        public string Address
        {
            get { return Contact.AddressStreet + ", " + Contact.AddressCity + ", " + Contact.AddressState; }
        }
    }
}