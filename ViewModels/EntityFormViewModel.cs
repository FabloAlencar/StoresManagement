﻿using System.ComponentModel.DataAnnotations;

namespace StoresManagement.ViewModels

{
    public class EntityFormViewModel
    {
        public int Id { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Entity Name")]
        public string Name { get; set; }
    }
}