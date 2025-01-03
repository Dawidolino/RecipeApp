﻿using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}