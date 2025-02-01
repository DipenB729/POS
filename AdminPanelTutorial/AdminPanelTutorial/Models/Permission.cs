﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace AdminPanelTutorial.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
