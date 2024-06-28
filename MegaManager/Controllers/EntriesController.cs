using MegaManager.Models;
using MegaManager.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MegaManager.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private ISession _session => HttpContext.Session;
        private const string EntriesKey = "Entries";

        public IActionResult Index()
        {
            // Получаем данные Entries из сессии или создаем новый список
            var entries = _session.GetObject<List<Entry>>(EntriesKey) ?? new List<Entry>
            {
                new Entry { Id = 1, Address = "example1@example.com", Password = "password1" },
                new Entry { Id = 2, Address = "example2@example.com", Password = "password2" },
                new Entry { Id = 3, Address = "example3@example.com", Password = "password3" }
            };
            _session.SetObject(EntriesKey, entries);

            return View(entries);
        }

        // GET: Entries/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            

            // Получаем данные Entries из сессии
            var entries = _session.GetObject<List<Entry>>(EntriesKey) ?? new List<Entry>();

            var entryToRemove = entries.FirstOrDefault(e => e.Id == id);
            if (entryToRemove == null)
                return NotFound();
            

            entries.Remove(entryToRemove);

            // Сохраняем обновленный список Entries в сессию
            _session.SetObject(EntriesKey, entries);

            return RedirectToAction(nameof(Index));
        }
    }
}
