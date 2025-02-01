using Microsoft.AspNetCore.Mvc;
using AdminPanelTutorial.Models; // Replace with your actual namespace
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AdminPanelTutorial.Data;

public class InventoryLogController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventoryLogController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: InventoryLog
    public IActionResult Index()
    {
        var inventoryLogs = _context.InventoryLogs.Include(i => i.Product).ToList();
        return View(inventoryLogs);
    }

    // GET: InventoryLog/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventoryLog = _context.InventoryLogs
            .Include(i => i.Product)
            .FirstOrDefault(m => m.Id == id);

        if (inventoryLog == null)
        {
            return NotFound();
        }

        return View(inventoryLog);
    }

    // GET: InventoryLog/Create
    public IActionResult Create()
    {
        // Populate ViewBag with products for foreign key selection
        ViewBag.Products = _context.Products.ToList();
        return View();
    }

    // POST: InventoryLog/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(InventoryLog inventoryLog)
    {
        if (ModelState.IsValid)
        {
            _context.Add(inventoryLog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Products = _context.Products.ToList(); // Repopulate ViewBag in case of errors
        return View(inventoryLog);
    }

    // GET: InventoryLog/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventoryLog = _context.InventoryLogs.Find(id);
        if (inventoryLog == null)
        {
            return NotFound();
        }

        // Populate ViewBag with products for foreign key selection
        ViewBag.Products = _context.Products.ToList();
        return View(inventoryLog);
    }

    // POST: InventoryLog/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, InventoryLog inventoryLog)
    {
        if (id != inventoryLog.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(inventoryLog);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.InventoryLogs.Any(e => e.Id == inventoryLog.Id))
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

        ViewBag.Products = _context.Products.ToList(); // Repopulate ViewBag in case of errors
        return View(inventoryLog);
    }

    // GET: InventoryLog/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventoryLog = _context.InventoryLogs
            .Include(i => i.Product)
            .FirstOrDefault(m => m.Id == id);

        if (inventoryLog == null)
        {
            return NotFound();
        }

        return View(inventoryLog);
    }

    // POST: InventoryLog/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var inventoryLog = _context.InventoryLogs.Find(id);
        if (inventoryLog != null)
        {
            _context.InventoryLogs.Remove(inventoryLog);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
