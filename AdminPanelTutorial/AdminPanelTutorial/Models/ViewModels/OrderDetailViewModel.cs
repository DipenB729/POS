namespace AdminPanelTutorial.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>(); // List of products with quantities and prices

        public class ProductViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => Price * Quantity; // Calculate total price per product
        }
    }
}
