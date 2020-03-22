using System.ComponentModel.DataAnnotations;

namespace StoresManagement.Models
{
    public class Contact
    {
        public int EntityId { get; set; }

        public int Id { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [Display(Name = "Address Country")]
        public string AddressCountry { get; set; }

        [Display(Name = "Address Postal Code")]
        public string AddressPostalCode { get; set; }

        [Display(Name = "Address State")]
        public string AddressState { get; set; }

        [Display(Name = "Address City")]
        public string AddressCity { get; set; }

        [Display(Name = "Address Street")]
        public string AddressStreet { get; set; }

        [Display(Name = "Address Number")]
        public int? AddressNumber { get; set; }

        [Display(Name = "Address Complement")]
        public string AddressComplement { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Customer Customer { get; set; }

        public string Address
        {
            get { return AddressStreet + ", " + AddressCity + ", " + AddressState; }
        }
    }
}