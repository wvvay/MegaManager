using MegaManager.Areas.Identity.Data;
using MegaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegaManager.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private readonly DBContextMegaManager _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EntriesController(DBContextMegaManager context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Entries
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _context.Entries
                .Where(e => e.IdUser == userId)
                .ToListAsync();
            return View(entries);

        }

        // GET: Entries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("URL,Login,Password,Notes")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    entry.IdUser = _userManager.GetUserId(User);
                    _context.Add(entry);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception or examine it to understand the cause
                    var errorMessage = $"Error saving entry: {ex.Message}";
                    ModelState.AddModelError("", errorMessage);
                }
            }
            return View(entry);
        }

    }
}
