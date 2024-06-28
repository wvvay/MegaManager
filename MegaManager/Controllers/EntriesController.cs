using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MegaManager.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        public IActionResult Index() => View();
    }
}
