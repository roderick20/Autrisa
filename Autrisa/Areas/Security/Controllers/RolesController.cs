using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autrisa.Models;

namespace Autrisa.Areas.Security.Controllers
{
    [Area("Security")]
    public class RolesController : Controller
    {
        private readonly EFContext _context;

        public RolesController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.Roles
                   .Select(m => new Role
                   {
                       UniqueId = m.UniqueId,
                       Name = m.Name,
                       Created = m.Created,
                       AuthorName = _context.Users.FirstOrDefault(m => m.Id == m.Author).Name,
                       EditorName = _context.Users.FirstOrDefault(m => m.Id == m.Editor).Name,
                       Modified = m.Modified,
                       Editor = m.Editor,
                   })
                   .ToListAsync());
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Select(m => new Role
                {
                    UniqueId = m.UniqueId,
                    Name = m.Name,
                    Created = m.Created,
                    AuthorName = _context.Users.FirstOrDefault(m => m.Id == m.Author).Name,
                    EditorName = _context.Users.FirstOrDefault(m => m.Id == m.Editor).Name,
                    Modified = m.Modified,
                    Editor = m.Editor,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Role role)
        {
            try
            {
                role.UniqueId = Guid.NewGuid();
                role.Created = DateTime.Now;
                role.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

            }
            return View(role);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UniqueId,Name,Auth")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    role.Modified = DateTime.Now;
                    role.Editor = (int)HttpContext.Session.GetInt32("UserId");
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'EFContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(long id)
        {
          return _context.Roles.Any(e => e.Id == id);
        }
    }
}
