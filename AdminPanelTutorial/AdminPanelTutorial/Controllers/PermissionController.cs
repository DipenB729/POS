using Microsoft.AspNetCore.Mvc;
using AdminPanelTutorial.Models;
using System.Linq;
using AdminPanelTutorial.Data;

namespace AdminPanelTutorial.Controllers
{
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var permissions = _context.Permissions.ToList();
            return View(permissions);
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                _context.Permissions.Add(permission); // Add the new permission
                _context.SaveChanges(); // Save to the database
                TempData["SuccessMessage"] = "Permission created successfully!";
                return RedirectToAction(nameof(Index)); // Redirect to the index view
            }

            TempData["ErrorMessage"] = "Failed to create permission. Please check the input.";
            return RedirectToAction(nameof(Index));
        }
        // GET: Permission/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var permission = _context.Permissions.Find(id);
            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        // POST: Permission/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Permission permission)
        {
            if (ModelState.IsValid)
            {
                _context.Permissions.Update(permission);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to the list of permissions
            }

            return View(permission);
        }

        // GET: Permission/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0) // Validate id
            {
                TempData["ErrorMessage"] = "Invalid ID provided for deletion.";
                return RedirectToAction("Index");
            }

            var permission = _context.Permissions.Find(id);
            if (permission == null) // Check if the permission exists
            {
                TempData["ErrorMessage"] = "Permission not found.";
                return RedirectToAction("Index");
            }

            return View(permission);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Console.WriteLine($"Delete method called for ID: {id}");
            var permission = _context.Permissions.Find(id);
            if (permission == null)
            {
                TempData["ErrorMessage"] = "Permission not found for deletion.";
                return RedirectToAction("Index");
            }

            _context.Permissions.Remove(permission);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Permission deleted successfully!";
            return RedirectToAction("Index");
        }



    }
}
