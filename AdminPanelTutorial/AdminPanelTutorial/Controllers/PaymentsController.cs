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
    public IActionResult Create(string[] name, int[] quantity, decimal[] price, decimal[] total, decimal Amount, string PaymentMethod)
    {
        // Store data in ViewBag to display in the view
        ViewBag.Products = name.Select((n, i) => new { Name = n, Quantity = quantity[i], Price = price[i], Total = total[i] }).ToList();
        ViewBag.Amount = Amount;
        ViewBag.PaymentMethod = PaymentMethod;

        return View();
    }



    [HttpPost]
    public IActionResult Create(Payment payment)
    {
        if (ModelState.IsValid)
        {
            payment.CreatedAt = DateTime.Now;
            payment.UpdatedAt = DateTime.Now;
            _context.Payment.Add(payment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
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
        // Using ViewBag for orders
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
         // Using ViewBag
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
