using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPanelTutorial.Models;
namespace AdminPanelTutorial.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        // Foreign key to Customer, nullable for walk-ins
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        // Foreign key to User (Cashier handling the order)
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Total price of the order
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Payment method (Cash, Card, Mobile Wallet)
        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; }

        // Order status (Completed, Refunded, VPID)
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        // Date and time when the order was created
        public DateTime CreatedAt { get; set; }

        // Date and time when the order was last updated
        public DateTime UpdatedAt { get; set; }
    }
}
