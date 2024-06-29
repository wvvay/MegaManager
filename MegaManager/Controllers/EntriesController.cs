using MegaManager.Areas.Identity.Data;
using MegaManager.Models;
using MegaManager.Utilities;
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
        private readonly ILogger<EntriesController> _logger;
        private ISession _session => HttpContext.Session;
        private const string EntriesKey = "Entries";

        public EntriesController(DBContextMegaManager context, UserManager<ApplicationUser> userManager, ILogger<EntriesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Entries
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _context.Entries.Where(e => e.IdUser == userId).ToListAsync();
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
                try
                {
                    entry.IdUser = _userManager.GetUserId(User);
                    _context.Add(entry);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Entry saved successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving the entry.");
                }
            return View(entry);
        }

    }

}
