using System;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPanelTutorial.Models;
namespace AdminPanelTutorial.Models
{
    public class InventoryLog
    {
        public int Id { get; set; } // Primary Key

        [ForeignKey("Product")]
        public int ProductId { get; set; } // Foreign Key to Products table

        public int QuantityChanged { get; set; } // Quantity changed (positive or negative)
        public string ChangeType { get; set; } // Type of change (addition, sale, manual adjustment)
        public string? Reason { get; set; } // Nullable reason (e.g., "restocked", "damaged")
        public DateTime CreatedAt { get; set; } // Record creation date
        public DateTime UpdatedAt { get; set; } // Record last updated date

        // Navigation property for the Product
        public virtual Product? Product { get; set; }
    }
}