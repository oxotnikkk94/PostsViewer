using Microsoft.AspNetCore.Mvc;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VkPostsViewer.Controllers
{
    public class VkController : Controller
    {
        // Токен доступа, который необходимо заменить на ваш токен
        private static string AccessToken = "vk1.a.Z_bXR_6iGXFlhhDCps4tKP9BeoLyqxr5jbZlq0U7QzdG9LO6hJvAQYTZSsywF568rTpxA1S_b7NzMdh0Ihy11WZjDYaPJaPJgzN256zuz9YKRgXdI9DtFvdGx3IXcsVGn3wkbNYSyZv9aCMA1jg7dRcGW2H_jKacTIYTvSvDhgvG4fM7Hz5Q5WrGSeMEeLZysB2gaPtXxbtivjLY7WKDcQ&";

        // ID группы или её короткое имя (screen_name)
        private static string GroupId = "poboru";

        // Теги, по которым будет осуществляться поиск
        private static List<string> Tags = new List<string>
        {
            "школьное питание",
            "питание в школах",
            "питание школьное",
            "питание школьников",
            "департамент продовольствия",
            "департамент питания",
            "организация питания",
            "организация школьного питания"
        };

        public IActionResult Index()
        {
            // Получение отфильтрованных постов
            var filteredPosts = GetFilteredVkPosts();

            // Передача данных в представление
            return View(filteredPosts);
        }

        // Метод для получения отфильтрованных постов
        private List<Post> GetFilteredVkPosts()
        {
            // Инициализация VK API клиента
            var api = new VkApi();
            api.Authorize(new ApiAuthParams { AccessToken = AccessToken });

            // Получение постов из группы
            var wallPosts = api.Wall.Get(new WallGetParams
            {
                OwnerId = -GetGroupId(api, GroupId),
                Count = 100
            });

            // Фильтрация постов по тегам
            var filteredPosts = FilterPostsByTags(wallPosts.WallPosts, Tags);

            return filteredPosts;
        }

        // Получение ID группы по её короткому имени (screen_name)
        private long GetGroupId(VkApi api, string groupName)
        {
            var group = api.Groups.GetById(null, groupName, GroupsFields.All).FirstOrDefault();
            return group?.Id ?? 0;
        }

        // Фильтрация постов по тегам
        private List<Post> FilterPostsByTags(IEnumerable<Post> posts, List<string> tags)
        {
            return posts.Where(post =>
                tags.Any(tag => post.Text != null && post.Text.Contains(tag, System.StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
    }
}
