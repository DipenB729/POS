using System;
using System.ComponentModel.DataAnnotations;
namespace AdminPanelTutorial.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Customer's Name

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } // Customer's Email

        [Required]
        [Phone]
        [StringLength(15)]
        public string Phone { get; set; } // Customer's Phone Number

        [Required]
        public string Address { get; set; } // Customer's Address

        [Required]
        public DateTime CreatedAt { get; set; } // Creation Timestamp

        [Required]
        public DateTime UpdatedAt { get; set; } // Last Updated Timestamp
    }
}
