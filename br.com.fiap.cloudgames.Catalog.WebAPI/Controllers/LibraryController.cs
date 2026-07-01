using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Catalog.WebAPI.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
