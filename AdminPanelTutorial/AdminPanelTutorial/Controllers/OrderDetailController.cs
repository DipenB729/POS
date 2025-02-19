using AdminPanelTutorial.Data;
using AdminPanelTutorial.Models;
using AdminPanelTutorial.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdminPanelTutorial.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetail/Create
        public IActionResult Create()
        {
            var products = _context.Products.ToList();
            if (products == null || !products.Any())
            {
                // Handle the case where no products are available (optional)
                return NotFound("No products available.");
            }

            ViewBag.Products = products;  // Passing products list to the view
            return View();  // Return the Create view
        }

        // POST: OrderDetail/Create or Edit
        [HttpPost]
        public IActionResult Create(string productIds, string quantities, string prices, string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "Payment Method is required.");
                return View(); // Return to the form with an error if payment method is missing
            }

            // Split product IDs, quantities, and prices into arrays
            var productIdsArray = productIds.Split(',').Select(int.Parse).ToList();
            var quantitiesArray = quantities.Split(',').Select(int.Parse).ToList();
            var pricesArray = prices.Split(',').Select(decimal.Parse).ToList();

            // Create an order first
            var newOrder = new Order
            {
                OrderNumber = GenerateOrderNumber(),  // Generate order number (you can implement this logic)
                CustomerId = null,  // Assuming null for walk-ins, set actual customer ID if applicable
                UserId = 1,  // Set UserId of the cashier handling the order (this should come from the logged-in user)
                TotalPrice = 0,  // Will calculate the total later
                PaymentMethod = paymentMethod,  // Cash/Card/Mobile Wallet
                Status = "Pending",  // Or any default status
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add the order to the context
            _context.Orders.Add(newOrder);
            _context.SaveChanges();  // Save to generate OrderId

            // Initialize total price for the order
            decimal totalPrice = 0;

            // Create order details for the selected products
            for (int i = 0; i < productIdsArray.Count; i++)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = newOrder.Id,  // Link this order detail to the newly created order
                    ProductId = productIdsArray[i],
                    Quantity = quantitiesArray[i],
                    Price = pricesArray[i]
                };

                // Add the order detail to the context
                _context.OrderItems.Add(orderDetail);

                // Calculate total price for the order
                totalPrice += pricesArray[i] * quantitiesArray[i];
            }

            // Update total price for the order
            newOrder.TotalPrice = totalPrice;
            _context.SaveChanges();  // Save the order details and update the total price

            return RedirectToAction("Index");  // Redirect to the index page or another page
        }

        private string GenerateOrderNumber()
        {
            // You can implement a custom logic to generate unique order numbers, e.g., Order-001, Order-002, etc.
            return "Order-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        // GET: OrderDetail/Index
        public IActionResult Index()
        {
            var orderDetails = _context.OrderItems
                .Include(od => od.Product)
                .Select(od => new OrderDetailViewModel
                {
                    OrderId = od.OrderId,
                    Products = new List<OrderDetailViewModel.ProductViewModel>
                    {
                new OrderDetailViewModel.ProductViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    Price = od.Price,
                    Quantity = od.Quantity
                }
                    }
                }).ToList();

            return View(orderDetails); // Passing OrderDetailViewModel list to the view
        }
        public IActionResult Delete(int id)
        {
            var orderDetail = _context.OrderItems
                .Include(od => od.Product)
                .FirstOrDefault(od => od.OrderId == id); // Assuming OrderId is the identifier for OrderDetail

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var orderDetail = _context.OrderItems
                .FirstOrDefault(od => od.OrderId == id);

            if (orderDetail != null)
            {
                _context.OrderItems.Remove(orderDetail);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index)); // Redirect to Index after deletion
        }

        // Handle the checkout logic here (if required)
        public IActionResult Checkout()
        {
            // Implement checkout logic
            return View();
        }
    }
}
