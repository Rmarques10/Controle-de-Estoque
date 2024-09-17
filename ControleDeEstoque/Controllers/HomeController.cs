using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
