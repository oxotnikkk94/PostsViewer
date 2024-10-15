using Microsoft.AspNetCore.Mvc;

namespace PostsViewer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetFilteredNewsJson(int offset, int limit)
        {
            // Здесь ваша логика получения постов
            return Json(new List<VkPostViewModel>());
        }
    }
}
