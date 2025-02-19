using System;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPanelTutorial.Models;
namespace AdminPanelTutorial.Models
{
    public class Payment
    {
        public int Id { get; set; }  // Primary Key (id)
     
      
        public decimal Amount { get; set; }  // Amount of the payment
        public string PaymentMethod { get; set; }  // Method of payment (e.g., Credit Card, PayPal)
        public DateTime PaymentDate { get; set; }  // Date and time when the payment was made
        public DateTime CreatedAt { get; set; }  // Timestamp when the payment record was created
        public DateTime UpdatedAt { get; set; }  // Timestamp when the payment record was last updated

        // Navigation property for related Order (assuming Order class is defined with Id as primary key)
        public virtual Order ?Order { get; set; }
    }
}
