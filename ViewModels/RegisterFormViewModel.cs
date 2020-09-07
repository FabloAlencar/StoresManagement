using StoresManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels

{
    public class RegisterFormViewModel
    {
        public int Id { get; set; }

        public RegisterFormViewModel()
        {
            Entities = new List<Entity>();
        }

        public IEnumerable<Entity> Entities { get; set; }

        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}