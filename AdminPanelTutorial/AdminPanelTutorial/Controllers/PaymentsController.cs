using AdminPanelTutorial.Data;
using AdminPanelTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class PaymentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Index - List all payments
    public async Task<IActionResult> Index()
    {
        var payments = await _context.Payment
            .Include(p => p.Order)  // Include related order information if needed
            .ToListAsync();
        return View(payments);
    }

    //Create - Display the form to create a new payment
    public IActionResult Create(int OrderId, decimal Amount, bool isCheckout = false)
    {
        ViewBag.IsCheckout = isCheckout;  // Set the flag in the viewbag

        if (isCheckout)
        {
            var payment = new Payment
            {
                OrderId = OrderId,
                Amount = Amount
            };

            return View(payment);
        }

        // If not coming from Checkout, show the page with order options
        ViewBag.Orders = _context.Orders.ToList();
        var amount = new Payment
        {
            Amount = Amount
        };
        return View(amount);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Payment payment)
    {
        if (ModelState.IsValid)
        {
            payment.CreatedAt = DateTime.Now;
            payment.UpdatedAt = DateTime.Now;
            _context.Add(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Orders = new SelectList(_context.Orders, "Id", "OrderNumber", payment.OrderId);  // Using ViewBag here as well
        return View(payment);
    }
  


    // Detail - View details of a specific payment
    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .Include(p => p.Order)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // Edit - Display the form to edit an existing payment
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment.FindAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        ViewBag.Orders = new SelectList(_context.Orders, "Id", "OrderNumber", payment.OrderId);  // Using ViewBag for orders
        return View(payment);
    }

    // Edit (POST) - Handle the form submission to update an existing payment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Payment payment)
    {
        if (id != payment.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                payment.UpdatedAt = DateTime.Now;
                _context.Update(payment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(payment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Orders = new SelectList(_context.Orders, "Id", "OrderNumber", payment.OrderId);  // Using ViewBag
        return View(payment);
    }

    // Delete - Display the confirmation page for deleting a payment
    // DELETE - Display the confirmation page for deleting a payment
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .Include(p => p.Order)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // DELETE (POST) - Handle the deletion of the payment
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payment = await _context.Payment.FindAsync(id);
        if (payment != null)
        {
            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }


    private bool PaymentExists(int id)
    {
        return _context.Payment.Any(e => e.Id == id);
    }
}
