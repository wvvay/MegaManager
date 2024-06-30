using MegaManager.Areas.Identity.Data;
using MegaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MegaManager.Utilities;
using System.Text.RegularExpressions;

namespace MegaManager.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private readonly DBContextMegaManager _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EntriesController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add IHttpContextAccessor for session
        Cypher cipher = new Cypher();
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
            // Проверка на наличие значения secretWord
            if (string.IsNullOrWhiteSpace(secretWord))
            {
                TempData["ErrorMessage"] = "Необходимо ввести Мастер-пароль";
                return RedirectToAction("SecretWord");
            }

            // Проверка на разрешённые символы (только буквы, цифры и некоторые специальные символы)
            // Можно настроить регулярное выражение для других требований
            string allowedPattern = @"^[a-zA-Z0-9!@#$%^&*()-=_+[\]{};':""|,.<>?\/\s]*$";
            if (!Regex.IsMatch(secretWord, allowedPattern))
            {
                TempData["ErrorMessage"] = "Недопустимые символы в Мастер-пароле";
                return RedirectToAction("SecretWord");
            }

            // Действие, если секретное слово прошло валидацию
            _httpContextAccessor.HttpContext.Session.SetString("SecretWord", secretWord);
            return RedirectToAction("Index");
        }

        // GET: Entries
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _context.Entries.Where(e => e.IdUser == userId).ToListAsync();
            var secretWord = _httpContextAccessor.HttpContext.Session.GetString("SecretWord");
            _logger.LogWarning(secretWord);

            // Дешифруем пароли
            foreach (var entry in entries)
            {
                entry.Password = cipher.Decrypt(entry.Password, secretWord); // Замените "YourMasterPassword" на реальный мастер-пароль
            }

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
                var secretWord = _httpContextAccessor.HttpContext.Session.GetString("SecretWord");
                entry.Password = cipher.Decrypt(entry.Password, secretWord);

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
                var secretWord = _httpContextAccessor.HttpContext.Session.GetString("SecretWord");
                entry.Password = cipher.Encrypt(entry.Password, secretWord);

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
                var secretWord = _httpContextAccessor.HttpContext.Session.GetString("SecretWord");
                entry.Password = cipher.Decrypt(entry.Password, secretWord);

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
