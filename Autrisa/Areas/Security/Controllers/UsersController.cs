using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autrisa.Models;
using Autrisa.Helper;
using Autrisa.Helpers;

namespace Autrisa.Areas.Security.Controllers
{
    [Area("Security")]
    public class UsersController : Controller
    {
        private readonly EFContext _context;

        public UsersController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(m => m.UserRoles)
                .ThenInclude(m => m.Role)
                .Select(m => new User
                {
                    UniqueId = m.UniqueId,
                    Name = m.Name,
                    Email = m.Email,
                    Password = m.Password,
                    LastAccess = m.LastAccess,
                    Enabled = m.Enabled,
                    Created = m.Created,
                    AuthorName = _context.Users.FirstOrDefault(m => m.Id == m.Author).Name,
                    EditorName = _context.Users.FirstOrDefault(m => m.Id == m.Editor).Name,
                    Modified = m.Modified,
                    Editor = m.Editor,
                    Roles = String.Join(", ", m.UserRoles.Select(x => x.Role.Name))
                }).ToListAsync());
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var User = await _context.Users
                .Select(m => new User
                {
                    UniqueId = m.UniqueId,
                    Name = m.Name,
                    Email = m.Email,
                    Password = m.Password,
                    LastAccess = m.LastAccess,
                    Enabled = m.Enabled,
                    Created = m.Created,
                    AuthorName = _context.Users.FirstOrDefault(m => m.Id == m.Author).Name,
                    EditorName = _context.Users.FirstOrDefault(m => m.Id == m.Editor).Name,
                    Modified = m.Modified,
                    Editor = m.Editor,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (User == null)
            {
                return NotFound();
            }

            return View(User);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _context.Roles.ToArrayAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Password,Enabled")] User user, List<int> Roles)
        {
            try
            {
                user.UniqueId = Guid.NewGuid();
                user.LastAccess = DateTime.Now;
                user.Password = PasswordEncrypt.Encrypt(user.Password);
                user.Created = DateTime.Now;
                user.Author = (int)HttpContext.Session.GetInt32("UserId");

                _context.Add(User);
                await _context.SaveChangesAsync();

                foreach (int rol in Roles)
                {
                    UserRole userRole = new UserRole();
                    userRole.UniqueId = Guid.NewGuid();
                    userRole.UserId = user.Id;
                    userRole.RoleId = rol;
                    userRole.Created = DateTime.Now;
                    userRole.Author = (int)HttpContext.Session.GetInt32("UserId");

                    _context.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

            }
            ViewBag.Roles = await _context.Roles.ToArrayAsync();
            return View(User);
        }

        public async Task<IActionResult> Edit(Guid? UniqueId)
        {
            try
            {
                var User = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);

                ViewBag.Roles = await _context.Roles.ToArrayAsync();
                ViewBag.UserRoles = await _context.UserRoles.Where(x => x.UserId == User.Id).ToArrayAsync();
                
                return View(User);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UniqueId,Name,Email,Enabled")] User User, List<int> Roles)
        {
            try
            {
                var UserEdit = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == User.UniqueId);
                UserEdit.Name = User.Name;
                UserEdit.Email = User.Email;
                UserEdit.Enabled = User.Enabled;

                UserEdit.Modified = DateTime.Now;
                UserEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");

                _context.Update(UserEdit);
                await _context.SaveChangesAsync();

                _context.UserRoles.RemoveRange(_context.UserRoles.Where(x => x.UserId == UserEdit.Id));
                await _context.SaveChangesAsync();

                foreach (int rol in Roles)
                {
                    UserRole userRole = new UserRole();
                    userRole.UniqueId = Guid.NewGuid();
                    userRole.UserId = UserEdit.Id;
                    userRole.RoleId = rol;
                    userRole.Created = DateTime.Now;
                    userRole.Author = (int)HttpContext.Session.GetInt32("UserId");

                    _context.Add(userRole);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //ViewBag.UserRoles = await _context.UserRoles.Where(x => x.UserId == User.Id).ToArrayAsync();
            ViewBag.Roles = await _context.Roles.ToArrayAsync();
            return View(User);
        }

        public async Task<IActionResult> EditPassword(Guid? UniqueId)
        {
            try
            {
                var User = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                return View(User);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword([Bind("UniqueId,Password")] User User)
        {
            try
            {
                var UserEdit = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == User.UniqueId);
                UserEdit.Password = PasswordEncrypt.Encrypt(User.Password);

                UserEdit.Modified = DateTime.Now;
                UserEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");

                _context.Update(UserEdit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(User);
        }

        // GET: Security/Users/Delete/5
        public async Task<IActionResult> Delete(Guid? UniqueId)
        {
            try
            {
                var User = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                return View(User);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // POST: Security/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? UniqueId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'EFContext.Users'  is null.");
            }
            var User = await _context.Users.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (User != null)
            {
                _context.Users.Remove(User);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}