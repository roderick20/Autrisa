using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autrisa.Models;
using Autrisa.Helpers;

namespace Autrisa.Areas.Security.Controllers
{
    [Area("Security")]
    public class SettingsController : Controller
    {
        private readonly EFContext _context;

        public SettingsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings
                .Select(m => new Setting
                {
                    UniqueId = m.UniqueId,
                    Title = m.Title,
                    Key = m.Key,
                    Value = m.Value,
                    Type = m.Type,
                    Created = m.Created,
                    //AuthorName = _context.Users.FirstOrDefault(m => m.Id == m.Author).Name,
                    //EditorName = _context.Users.FirstOrDefault(m => m.Id == m.Editor).Name,
                    Modified = m.Modified,
                    Editor = m.Editor,
                })
                .ToListAsync());
        }
               
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(Guid UniqueId)
        {
                       var setting = await _context.Settings.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UniqueId,Value")] Setting setting)
        {
            try
            {
                Setting settingEdit = await _context.Settings.FirstOrDefaultAsync(m => m.UniqueId == setting.UniqueId);
                if (String.IsNullOrEmpty(setting.Value))
                {
                    settingEdit.Value = "-";
                }
                else
                {
                    settingEdit.Value = setting.Value;
                }               

                _context.Update(settingEdit);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetSettings(_context.Settings.ToDictionary(m => m.Key, m => m.Value));

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return View(setting);
        }
        
        
        public async Task<IActionResult> EditImagen(Guid UniqueId)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImagen([Bind("UniqueId,Value")] Setting setting, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/settings/images/" + file.FileName.Replace(" ", "-"));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    file.CopyTo(new FileStream(imagePath, FileMode.Create));

                    Setting settingEdit = await _context.Settings.FirstOrDefaultAsync(m => m.UniqueId == setting.UniqueId);

                    settingEdit.Value = file.FileName.Replace(" ", "-");


                    _context.Update(settingEdit);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetSettings(_context.Settings.ToDictionary(m => m.Key, m => m.Value));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return View(setting);
        }
    
    }
}