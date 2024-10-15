using Microsoft.AspNetCore.Mvc;
using VkNet;
using VkNet.Model;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using VkNet.Exception;
using PostsViewer.Controllers;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Net.Mime.MediaTypeNames;

namespace VkPostsViewer.Controllers
{
    public class VkController : Controller
    {
        private readonly VkApi _vkApi;
        private readonly string accessToken = "5340627a5340627a5340627a07505e4aba553405340627a35a6df42b88c441143d65a89";
        private readonly HttpClient _httpClient; // Для взаимодействия с API DeepPavlov
        private Morphy AnalizerMorphy = new Morphy();
        private static List<string> Tags = new List<string>
        {
            "школьное питание",
            "питание в школах",
            "питание школьное",
            "питание школьников",
            "департамент продовольствия",
            "департамент питания",
            "организация питания",
            "организация школьного питания",
            "школьное питание",
            "питание в школах",
            "питание школьное",
            "питание школьников",
            "департамент продовольствия",
            "департамент питания",
            "организация питания",
            "организация школьного питания",
            "жалоба на питание",
            "обращение на питание",
            "качество питания",
            "качественное питание",
            "некачественное питание",
            "жалоба на качество питания",
            "питание детей",
            "питание школьников",
            "питание в казани",
            "казанское питание",
            "школы казани",
            "школьники казани",
            "poelidovolen",

            "Куда пожаловаться на некачественное питание в школе Казань?",
            "Куда пожаловаться на некачественное питание в гимназии Казань?",
            "Куда пожаловаться на некачественное питание в гимназии Казань?",
            "Куда пожаловаться на некачественное питание в ДОУ Казань?",
            "Куда пожаловаться на некачественное питание в садике Казань?",
            "Куда пожаловаться на некачественное питание в больнице Казань?",

            "Жалоба на питание в школе Казань",
            "Жалоба на питание в гимназии Казань",
            "Жалоба на питание в гимназии Казань",
            "Жалоба на питание в ДОУ Казань",
            "Жалоба на питание в садике Казань",
            "Жалоба на питание в больнице Казань",

            "Некачественное питание в школе Казань",
            "Некачественное питание в гимназии Казань",
            "Некачественное питание в гимназии Казань",
            "Некачественное питание в ДОУ Казань",
            "Некачественное питание в садике Казань",
            "Некачественное питание в больнице Казань",

            "Плохое питание в школе Казань",
            "Плохое питание в гимназии Казань",
            "Плохое питание в гимназии Казань",
            "Плохое питание в ДОУ Казань",
            "Плохое питание в садике Казань",
            "Плохое питание в больнице Казань",

            "Ужасное питание в школе Казань",
            "Ужасное питание в гимназии Казань",
            "Ужасное питание в гимназии Казань",
            "Ужасное питание в ДОУ Казань",
            "Ужасное питание в садике Казань",
            "Ужасное питание в больнице Казань",

            "Плохо кормят в школе Казань",
            "Плохо кормят в гимназии Казань",
            "Плохо кормят в гимназии Казань",
            "Плохо кормят в ДОУ Казань",
            "Плохо кормят в садике Казань",
            "Плохо кормят в больнице Казань",

            "Кому пожаловаться на питание в школе Казань?",
            "Кому пожаловаться на питание в гимназии Казань?",
            "Кому пожаловаться на питание в гимназии Казань?",
            "Кому пожаловаться на питание в ДОУ Казань?",
            "Кому пожаловаться на питание в садике Казань?",
            "Кому пожаловаться на питание в больнице Казань?",

            "Куда обратиться по вопросам питания в школе Казань?",
            "Куда обратиться по вопросам питания в гимназии Казань?",
            "Куда обратиться по вопросам питания в гимназии Казань?",
            "Куда обратиться по вопросам питания в ДОУ Казань?",
            "Куда обратиться по вопросам питания в садике Казань?",
            "Куда обратиться по вопросам питания в больнице Казань?",

            "Кто кормит в школе Казань",
            "Кто кормит в гимназии Казань",
            "Кто кормит в гимназии Казань",
            "Кто кормит в ДОУ Казань",
            "Кто кормит в садике Казань",
            "Кто кормит в больнице Казань",

            "Оставить жалобу на питание в школе Казань",
            "Оставить жалобу на питаниев гимназии Казань",
            "Оставить жалобу на питание в гимназии Казань",
            "Оставить жалобу на питание в ДОУ Казань",
            "Оставить жалобу на питание в садике Казань",
            "Оставить жалобу на питание в больнице Казань",

            "Невкусно кормят в школе Казань",
            "Невкусно кормят в гимназии Казань",
            "Невкусно кормят в гимназии Казань",
            "Невкусно кормят в ДОУ Казань",
            "Невкусно кормят в садике Казань",
            "Невкусно кормят в больнице Казань",

            "Невкусно кормят в школе Казань",
            "Невкусно кормят в гимназии Казань",
            "Невкусно кормят в гимназии Казань",
            "Невкусно кормят в ДОУ Казань",
            "Невкусно кормят в садике Казань",
            "Невкусно кормят в больнице Казань",

            "Школа Казань питание жалоба",
            "гимназия Казань питание жалоба",
            "лицей Казань питание жалоба",
            "ДОУ Казань питание жалоба",
            "садик Казань питание жалоба",
            "больница Казань питание жалоба",

            "Где пожаловаться на питание в Казани школа",
            "Где пожаловаться на питание в Казани гимназия",
            "Где пожаловаться на питание в Казани лицей",
            "Где пожаловаться на питание в Казани ДОУ",
            "Где пожаловаться на питание в Казани сад ",
            "Где пожаловаться на питание в Казани больница",

            "Оставить жалобу на питание",
            "Оставить жалобу на питание в Казани"
        };
        private static List<string> TagsFiltered = new List<string>();

        public VkController(HttpClient httpClient)
        {
            _vkApi = new VkApi();
            _vkApi.Authorize(new ApiAuthParams { AccessToken = accessToken });
            _httpClient = httpClient;
        }

        // Метод для получения постов из групп
        public async Task<IActionResult> GetFilteredNews()
        {
            var groups = new List<string>
            {
                "poboru",
                "ro_s_t",
                "kzn",
                "region_kazan116",
                "kznlife",
                "vkazani",
                "kazanushka",
                "public109737032",
                "kazan_news",
                "kazan__kzn",
                "i_kzn",
                "megamamakazan",
                "kazanpronews",
                "kazan.aktual",
                "tatarstan_da"
            };

            var filteredPosts = new List<VkPostViewModel>();

            TagsFiltered = await PerformMorphologicalAnalysis();

            foreach (var group in groups)
            {
                var groupPosts = await GetPostsFromGroup(group);
                foreach (var post in groupPosts)
                {
                    if (TagsFiltered.Any(k => post.Text.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        var sentiment = await AnalyzeSentiment(post.Text); // Асинхронный вызов анализа настроений
                        var comments = await GetPostComments(post.Id.Value, post.OwnerId.Value);

                        filteredPosts.Add(new VkPostViewModel
                        {
                            Group = group,
                            Text = post.Text,
                            Date = post.Date,
                            Sentiment = sentiment,
                            Comments = comments
                        });
                    }
                }
            }

            return View(filteredPosts); // Возвращаем отфильтрованные данные во View
        }

        private async Task<IEnumerable<Post>> GetPostsFromGroup(string groupName)
        {
            try
            {
                // Асинхронное получение информации о группе
                var groups = await _vkApi.Groups.GetByIdAsync(null, groupName, null);
                var group = groups.FirstOrDefault();

                if (group == null)
                {
                    Console.WriteLine($"Группа {groupName} не найдена.");
                    return Enumerable.Empty<Post>();
                }

                // Асинхронное получение постов из группы
                var posts = await _vkApi.Wall.GetAsync(new WallGetParams
                {
                    OwnerId = -group.Id,  // Для групп указываем отрицательное значение owner_id
                    Count = 10
                });

                Console.WriteLine($"Найдено {posts.WallPosts.Count} постов в группе {groupName}");

                return posts.WallPosts;
            }
            catch (AccessDeniedException ex)
            {
                Console.WriteLine($"Доступ запрещен для группы {groupName}: {ex.Message}");
                return Enumerable.Empty<Post>(); // Возвращаем пустой список, если доступ запрещен
            }
            catch (ParameterMissingOrInvalidException ex)
            {
                Console.WriteLine($"Ошибка параметров для группы {groupName}: {ex.Message}");
                return Enumerable.Empty<Post>(); // Возвращаем пустой список в случае других ошибок
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении постов из группы {groupName}: {ex.Message}");
                return Enumerable.Empty<Post>(); // Возвращаем пустой список в случае других ошибок
            }
        }



        // Метод для получения комментариев к посту
        private async Task<List<CommentViewModel>> GetPostComments(long postId, long ownerId)
        {
            var commentsResponse = _vkApi.Wall.GetComments(new WallGetCommentsParams
            {
                PostId = postId,
                OwnerId = ownerId,
                Count = 10
            });

            var comments = new List<CommentViewModel>();

            foreach (var comment in commentsResponse.Items)
            {
                var sentiment = await AnalyzeSentiment(comment.Text); // Асинхронный вызов анализа настроений
                comments.Add(new CommentViewModel
                {
                    Text = comment.Text,
                    Sentiment = sentiment,
                    Author = GetUserInfo((long)comment.FromId)
                });
            }

            return comments;
        }

        private async Task<string> AnalyzeSentiment(string text)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { text = text });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://121.127.37.60:5000/sentiment", content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SentimentResponse>>(responseBody)[0].Label; // Обработка результата
            }

            return "Error"; // Обработка ошибок
        }

        // Получение информации об авторе поста или комментария
        private string GetUserInfo(long userId)
        {
            try
            {
                if (userId < 0)
                {
                    // Получение информации о группе
                    var groupId = userId.ToString().TrimStart('-'); // Извлекаем положительное значение ID
                    var group = _vkApi.Groups.GetById(null, groupId, null).FirstOrDefault();
                    return group != null ? group.Name : "Unknown Group";
                }
                else
                {
                    // Получение информации о пользователе
                    var user = _vkApi.Users.Get(new List<long> { userId }).FirstOrDefault();
                    return user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User";
                }
            }
            catch (AccessDeniedException ex)
            {
                Console.WriteLine($"Access denied for user/group {userId}: {ex.Message}");
                return "Access Denied"; // Возвращаем сообщение о запрете доступа
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user info for {userId}: {ex.Message}");
                return "Error"; // Возвращаем сообщение об ошибке
            }
        }


        private async Task<List<string>> PerformMorphologicalAnalysis()
        {
            List<string> newText = new List<string>();

            foreach (var morph in Tags)
            {
                var a = morph.Split(' ').ToList();
                foreach (var b in a)
                {
                    if (b.Length > 4)
                        newText.Add(b);
                }
            }
            var reluts = AnalizerMorphy.Analyzer.Parse(newText).ToList();

            foreach (var item in reluts)
            {
                foreach (var a in item.Tags.ToList())
                {
                    if (!TagsFiltered.Contains(a.Lemma))
                        TagsFiltered.Add(a.Lemma);
                }
            }

            return TagsFiltered ?? new List<string>();
        }
    }

    // Модель для обработки результата анализа настроений
    public class SentimentResponse
    {
        public string Label { get; set; }
        public float Score { get; set; }
    }

    // ViewModel для поста
    public class VkPostViewModel
    {
        public string Group { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public string Sentiment { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    // ViewModel для комментария
    public class CommentViewModel
    {
        public string Text { get; set; }
        public string Sentiment { get; set; }
        public string Author { get; set; }
    }
}
