using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models
{
    public class Entity
    {
        public int Id { get; set; }

        [Display(Name = "Entity Name")]
        public string Name { get; set; }
    }
}