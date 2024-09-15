using Microsoft.AspNetCore.Mvc;

namespace VkPostsViewer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
