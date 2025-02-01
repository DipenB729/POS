using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPanelTutorial.Models;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }  // Navigation property

    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }  // Navigation property

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }  // Price at the time of sale

    [NotMapped] // Not stored in DB, calculated in the application
    public decimal TotalPrice => Quantity * Price;
}
