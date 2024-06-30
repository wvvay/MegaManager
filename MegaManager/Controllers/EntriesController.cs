using MegaManager.Areas.Identity.Data;
using MegaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor; // Add IHttpContextAccessor for session

        public EntriesController(DBContextMegaManager context, UserManager<ApplicationUser> userManager, 
            ILogger<EntriesController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        // GET: Entries/SecretWord
        [AllowAnonymous] // Allow access to this action without authentication
        public IActionResult SecretWord()
        {
            return View();
        }

        // POST: Entries/VerifySecretWord
        [AllowAnonymous] // Allow access to this action without authentication
        [HttpPost]
        public IActionResult VerifySecretWord(string secretWord)
        {
            // Replace "mySecretWord" with your actual secret word
            if (secretWord != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("SecretWord", secretWord);
                // Redirect to the Entries/Index action upon successful verification
                return RedirectToAction("Index");
            }
            else
            {
                // Redirect back to the SecretWord view with an error message
                TempData["ErrorMessage"] = "Incorrect secret word.";
                return RedirectToAction("SecretWord");
            }
        }








        // GET: Entries
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _context.Entries.Where(e => e.IdUser == userId).ToListAsync();
            var secretWord = _httpContextAccessor.HttpContext.Session.GetString("SecretWord");
            _logger.LogWarning(secretWord);


            return View(entries);
        }

        // GET: Entries/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Entries/Update
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var entry = await _context.Entries.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning("Entity with ID {Id} not found.", id);
                    return NotFound();
                }

                if (entry.IdUser != _userManager.GetUserId(User))
                {
                    _logger.LogWarning("UserId {Id} trying delete foreign entry", _userManager.GetUserId(User));
                    return NotFound();
                }

                return View(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting entity with ID {Id}.", id);
                return NotFound();
            }
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


        // POST: Entries/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Entry entry)
        {
            try
            {
                entry.IdUser = _userManager.GetUserId(User);
                _context.Update(entry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Entry updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the entry.");
            }
            return View(entry);
        }



        // POST: Entries/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entry = await _context.Entries.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning("Entity with ID {Id} not found.", id);
                    return NotFound();
                }

                if (entry.IdUser != _userManager.GetUserId(User))
                {
                    _logger.LogWarning("UserId {Id} trying delete foreign entry", _userManager.GetUserId(User));
                    return NotFound();
                }

                _context.Entries.Remove(entry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Entity with ID {Id} has been deleted.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting entity with ID {Id}.", id);
                return NotFound();
            }
        }
    }
}
