using AdminPanelTutorial.Data;
using AdminPanelTutorial.Models;
using AdminPanelTutorial.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelTutorial.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Role/Index
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.Include(r => r.Permissions).ToListAsync();
            return View(roles);
        }

        // GET: Role/Show/5
        public async Task<IActionResult> Show(int id)
        {
            var role = await _context.Roles
                                     .Include(r => r.Permissions)
                                     .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Role/Create
       
        public IActionResult Create()
        {
            // Ensure this line correctly populates permissions
            ViewBag.Permissions = _context.Permissions.ToList();
            return View();
        }


        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Role role, int[] selectedPermissions)
        {
            // Remove Permissions validation
            ModelState.Remove("Permissions");

            if (ModelState.IsValid)
            {
                // Add the new role
                _context.Add(role);
                await _context.SaveChangesAsync();

                // Associate selected permissions with the role
                if (selectedPermissions != null && selectedPermissions.Length > 0)
                {
                    foreach (var permissionId in selectedPermissions)
                    {
                        var permission = await _context.Permissions.FindAsync(permissionId);
                        if (permission != null)
                        {
                            role.Permissions = role.Permissions ?? new List<Permission>();
                            role.Permissions.Add(permission);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Permissions = _context.Permissions.ToList(); // Repopulate permissions
            return View(role);
        }



        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles
                                     .Include(r => r.Permissions)
                                     .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            ViewBag.Permissions = _context.Permissions.ToList();
            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Role role, int[] selectedPermissions)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            // Remove Permissions validation
            ModelState.Remove("Permissions");

            if (ModelState.IsValid)
            {
                var existingRole = await _context.Roles
                                                 .Include(r => r.Permissions)
                                                 .FirstOrDefaultAsync(r => r.Id == id);

                if (existingRole == null)
                {
                    return NotFound();
                }

                // Update role name
                existingRole.Name = role.Name;

                // Update permissions
                existingRole.Permissions.Clear();
                if (selectedPermissions != null && selectedPermissions.Any())
                {
                    foreach (var permissionId in selectedPermissions)
                    {
                        var permission = await _context.Permissions.FindAsync(permissionId);
                        if (permission != null)
                        {
                            existingRole.Permissions.Add(permission);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Permissions = _context.Permissions.ToList(); // Repopulate permissions
            return View(role);
        }


        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles
                                     .Include(r => r.Permissions)
                                     .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles
                                     .Include(r => r.Permissions)
                                     .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
