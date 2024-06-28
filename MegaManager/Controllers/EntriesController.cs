using MegaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MegaManager.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        public IActionResult Index()
        {
            //Обратиться к БД и получить список обьектов класса Entry

            var entries = new List<Entry>
            {
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example1@example.com", Password = "password1" },
                new Entry { Address = "example2@example.com", Password = "password2" },
                new Entry { Address = "example3@example.com", Password = "password3" }
            };


            return View(entries);
        }
    }
}
