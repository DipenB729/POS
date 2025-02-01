using AdminPanelTutorial.Data;
using AdminPanelTutorial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using AdminPanelTutorial.Models; // Replace with your actual models namespace

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Order
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
                                   .Include(o => o.Customer)  // Include related customer data
                                   .Include(o => o.User)      // Include related user (cashier) data
                                   .ToListAsync();
        return View(orders);
    }

    // GET: Order/Create
    public IActionResult Create()
    {
        // Get the list of customers
        var customers = _context.Customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        // Get the list of users
        var users = _context.Users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name
        }).ToList();

        // Pass them to the view using ViewBag
        ViewBag.CustomerId = new SelectList(customers, "Value", "Text");
        ViewBag.UserId = new SelectList(users, "Value", "Text");

        return View();
    }



    // POST: Order/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Order order)
    {
        if (ModelState.IsValid)
        {
            // Save the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // If validation fails, we need to repopulate ViewBag
        var customers = _context.Customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        var users = _context.Users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name
        }).ToList();

        ViewBag.CustomerId = new SelectList(customers, "Value", "Text");
        ViewBag.UserId = new SelectList(users, "Value", "Text");

        return View(order); // Return the model with validation errors
    }


    // GET: Order/Details/5
    public IActionResult Details(int id)
    {
        var order = _context.Orders
                            .Include(o => o.Customer)  // If you want to include the related Customer
                            .Include(o => o.User)      // If you want to include the related User
                            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    public IActionResult Edit(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound("Order not found.");
        }

        // Get the list of customers
        var customers = _context.Customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        // Get the list of users
        var users = _context.Users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name
        }).ToList();

        // Populate ViewBag for dropdowns
        ViewBag.CustomerId = new SelectList(customers, "Value", "Text", order.CustomerId);
        ViewBag.UserId = new SelectList(users, "Value", "Text", order.UserId);

        return View(order);
    }






    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // If validation fails, repopulate dropdowns
        var customers = _context.Customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        var users = _context.Users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name
        }).ToList();

        ViewBag.CustomerId = new SelectList(customers, "Value", "Text", order.CustomerId);
        ViewBag.UserId = new SelectList(users, "Value", "Text", order.UserId);

        return View(order);
    }





    public IActionResult Delete(int id)
    {
        var order = _context.Orders
                            .Include(o => o.Customer)  // Ensure Customer is loaded
                            .Include(o => o.User)      // Ensure User is loaded
                            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }



    // POST: Order/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orderDetail = await _context.OrderItems.FindAsync(id);

        if (orderDetail != null)
        {
            _context.OrderItems.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Redirect to the Index page after deleting
        return RedirectToAction(nameof(Index));
    }








    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
