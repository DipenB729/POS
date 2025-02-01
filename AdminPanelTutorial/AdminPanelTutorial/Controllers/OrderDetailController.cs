using AdminPanelTutorial.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class OrderDetailController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderDetailController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: OrderDetail/Index
    public async Task<IActionResult> Index()
    {
        var orderDetails = _context.OrderItems.Include(o => o.Order).Include(p => p.Product);
        return View(await orderDetails.ToListAsync());
    }

    // GET: OrderDetail/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var orderDetail = await _context.OrderItems
            .Include(o => o.Order)
            .Include(p => p.Product)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (orderDetail == null) return NotFound();

        return View(orderDetail);
    }

    // GET: OrderDetail/Create
    public IActionResult Create()
    {
      
        // Set ViewBag properties instead of ViewData
        ViewBag.Orders = _context.Orders.ToList(); // Fetch the list of orders
        ViewBag.Products = _context.Products.ToList(); // Fetch the list of products

        return View();
    }


    // POST: OrderDetail/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OrderId,Quantity,Price")] OrderDetail orderDetail, int[] ProductIds)
    {
        // Ensure ViewBag is populated with Orders and Products (in case of form validation failure)
        ViewBag.Orders = _context.Orders.ToList();
        ViewBag.Products = _context.Products.ToList();

        // Check if the model state is valid
        if (ModelState.IsValid)
        {
            // Loop through the selected product IDs
            foreach (var productId in ProductIds)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    // Create new order detail for each product selected
                    var newOrderDetail = new OrderDetail
                    {
                        OrderId = orderDetail.OrderId,
                        ProductId = productId,
                        Quantity = orderDetail.Quantity,
                        Price = orderDetail.Price
                    };
                    _context.OrderItems.Add(newOrderDetail); // Add the new order detail to the context
                }
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to Index or another action after successfully saving
            return RedirectToAction(nameof(Index));
        }

        // If the model is not valid, redisplay the form with validation errors
        return View(orderDetail);
    }



    // GET: OrderDetail/Edit/5
    public IActionResult Edit(int id)
    {
        // Retrieve the order detail by its ID
        var orderDetail = _context.OrderItems.Find(id);

        // If the order detail is not found, redirect to the index page
        if (orderDetail == null)
        {
            return NotFound();
        }

        // Populate ViewBag with Orders and Products for the dropdowns
        ViewBag.Orders = _context.Orders.ToList();
        ViewBag.Products = _context.Products.ToList();

        // Pass the order detail to the view
        return View(orderDetail);
    }


    // POST: OrderDetail/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,Quantity,Price")] OrderDetail orderDetail, int[] ProductIds)
    {
        // Ensure ViewBag is populated with Orders and Products (in case of form validation failure)
        ViewBag.Orders = _context.Orders.ToList();
        ViewBag.Products = _context.Products.ToList();

        // Check if the ID in the URL matches the ID in the form
        if (id != orderDetail.Id)
        {
            return NotFound();
        }

        // Check if the model state is valid
        if (ModelState.IsValid)
        {
            try
            {
                // Delete the old order details for this order
                var oldOrderDetails = _context.OrderItems.Where(od => od.OrderId == orderDetail.OrderId && od.Id != id).ToList();
                _context.OrderItems.RemoveRange(oldOrderDetails);

                // Add the new order details for the selected products
                foreach (var productId in ProductIds)
                {
                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        var newOrderDetail = new OrderDetail
                        {
                            OrderId = orderDetail.OrderId,
                            ProductId = productId,
                            Quantity = orderDetail.Quantity,
                            Price = orderDetail.Price
                        };
                        _context.OrderItems.Add(newOrderDetail);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderItems.Any(e => e.Id == orderDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to Index or another action after successfully saving
            return RedirectToAction(nameof(Index));
        }

        // If the model is not valid, redisplay the form with validation errors
        return View(orderDetail);
    }



    // GET: OrderDetail/Delete/5
    public IActionResult Delete(int id)
    {
        // Retrieve the order detail by its ID
        var orderDetail = _context.OrderItems
            .FirstOrDefault(m => m.Id == id);

        // If the order detail is not found, redirect to the index page
        if (orderDetail == null)
        {
            return NotFound();
        }

        // Pass the order detail to the view
        return View(orderDetail);
    }


    // POST: OrderDetail/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orderDetail = await _context.OrderItems.FindAsync(id);

        if (orderDetail != null)
        {
            _context.OrderItems.Remove(orderDetail); // Delete the order detail
            await _context.SaveChangesAsync(); // Save changes
        }

        // Redirect to Index after deletion
        return RedirectToAction(nameof(Index));
    }


}
