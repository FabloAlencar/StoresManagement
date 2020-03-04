using StoresManagement.Models;

namespace StoresManagement.ViewModels
{
    public class BranchFormViewModel
    {
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public string Identification { get; set; }

        public char? Type { get; set; }

        public string Name { get; set; }

        public virtual Contact Contact { get; set; }

        public string Address
        {
            get { return Contact.AddressStreet + ", " + Contact.AddressCity + ", " + Contact.AddressState; }
        }
    }
}