using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using TL;
using WTelegram;

namespace PostsViewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private Client _client;
        private readonly HttpClient _httpClient; // Для взаимодействия с API DeepPavlov
        private Morphy AnalizerMorphy = new Morphy();
        private string _apiId = "26908424"; // Замени на свой API ID
        private string _apiHash = "9d40c54db3163c9141ef097b97415d37"; // Замени на свой API Hash

        public TelegramController(HttpClient httpClient)
        {
            // Конфигурация WTelegramClient
            _client = new Client(Config);
            _httpClient = httpClient;
        }

        private string Config(string what)
        {
            return what switch
            {
                "api_id" => _apiId,
                "api_hash" => _apiHash,
                "phone_number" => "+79296727018", // Можно оставить пустым, если есть сессия
                "verification_code" => Console.ReadLine(), // Код подтверждения для авторизации
                _ => null
            } ?? string.Empty;
        }

        private static List<string> TagsFiltered = new List<string>();

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

        private static List<string> Channels = new List<string>
        {
            "@kazancity",
            "vkazani",
            "region116_dtp_official",
            "kznonline",
            "kzn_official",
            "tatarstan_da",
            "Eduvtatarstan",
            "kznpapa",
            "prtatar",
            "gazetabo",
            "kazanfirst",
            "inkazanischa",
            "Tatarstan24TV",
            "szsocpit",
            "tatmediaofficial"
        };

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

        // Анализ настроений
        private async Task<string> AnalyzeSentiment(string text)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { text = text });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://121.127.37.60:5000/sentiment", content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SentimentResponse>>(responseBody)[0].Label;
            }

            return "Error"; // Обработка ошибок
        }

        // Получение комментариев к сообщению
        private async Task<List<CommentViewModel>> GetMessageReplies(Message post)
        {
            // Получение информации о канале
            InputChannel inputChannel = new InputChannel(post.peer_id, post.grouped_id);
            var channel = await _client.Channels_GetChannels(new[] { inputChannel });
            var foundChannel = channel.chats.OfType<Channel>().FirstOrDefault();

            if (foundChannel == null)
            {
                throw new Exception("Не удалось найти канал для данного сообщения.");
            }

            InputPeerChannel inputPeerChannel = new InputPeerChannel(foundChannel.id, foundChannel.access_hash);

            var replies = await _client.Messages_GetReplies(inputPeerChannel, post.id);

            var comments = new List<CommentViewModel>();

            foreach (var reply in replies.Messages.OfType<Message>())
            {
                var sentiment = await AnalyzeSentiment(reply.message); // Анализ настроений комментария
                comments.Add(new CommentViewModel
                {
                    Text = reply.message,
                    Sentiment = sentiment,
                    Author = reply.From?.ToString() ?? "Unknown" // Получаем имя автора
                });
            }

            return comments;
        }

        // Получение сообщений из канала
        private async Task<List<Message>> GetMessagesFromChannel(string channelName)
        {
            var dialogs = await _client.Messages_GetAllDialogs(); // Получаем все чаты
            var channel = dialogs.chats.Values.OfType<Channel>().FirstOrDefault(c => c.username == channelName.TrimStart('@'));

            if (channel == null)
            {
                Console.WriteLine($"Канал {channelName} не найден.");
                return new List<Message>();
            }

            var history = await _client.Messages_GetHistory(channel, limit: 10); // Получаем историю сообщений
            return history.Messages.OfType<Message>().ToList(); // Возвращаем список сообщений
        }


        public async Task<IActionResult> GetFilteredNews()
        {
            var channels = Channels;

            var filteredPosts = new List<TelegramPostViewModel>();

            TagsFiltered = await PerformMorphologicalAnalysis(); // Фильтрация тегов

            foreach (var channel in channels)
            {
                var channelPosts = await GetMessagesFromChannel(channel); // Получение сообщений из канала
                foreach (var post in channelPosts)
                {
                    if (TagsFiltered.Any(k => post.message.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        var sentiment = await AnalyzeSentiment(post.message); // Анализ настроения
                        var comments = await GetMessageReplies(post);

                        filteredPosts.Add(new TelegramPostViewModel
                        {
                            Channel = channel,
                            Text = post.message,
                            Date = post.Date,
                            Sentiment = sentiment,
                            Comments = comments
                        });
                    }
                }
            }

            return base.View(filteredPosts); // Возвращаем отфильтрованные данные во View
        }


        //    // Метод для авторизации по номеру телефона
        [HttpGet("Login")]
        private async Task LoginAsync()
        {
            _client = new Client(Config);
            var user = await _client.LoginUserIfNeeded();
            Console.WriteLine($"Logged in as {user.username ?? user.first_name}");
        }

        // Метод для получения сообщений из канала
        private async Task<string[]> FetchMessagesFromChannel(string channelName, int messageCount = 50)
        {
            var dialogs = await _client.Messages_GetAllDialogs();
            var channel = dialogs.chats.Values
                .FirstOrDefault(c => c is Channel ch && ch.title == channelName) as Channel;

            if (channel == null)
            {
                throw new Exception($"Channel {channelName} not found.");
            }

            var messages = await _client.Messages_GetHistory(channel, limit: messageCount);
            return messages.Messages.OfType<Message>().Select(m => m.message).ToArray();
        }

        // Метод для получения сообщений с фильтрацией по ключевым словам (тегам)
        private async Task<string[]> FetchMessagesWithKeywords(string channelName, string[] keywords,
            int messageCount = 50)
        {
            var dialogs = await _client.Messages_GetAllDialogs();
            var channel = dialogs.chats.Values
                .FirstOrDefault(c => c is Channel ch && ch.title == channelName) as Channel;

            if (channel == null)
            {
                throw new Exception($"Channel {channelName} not found.");
            }

            var messages = await _client.Messages_GetHistory(channel, limit: messageCount);
            return messages.Messages.OfType<Message>()
                .Where(m => keywords.Any(keyword => m.message.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .Select(m => m.message)
                .ToArray();
        }

        // Метод API для получения сообщений из канала
        [HttpGet("channel/{channelName}")]
        public async Task<IActionResult> GetMessages(string channelName, [FromQuery] string[] keywords,
            int messageCount = 50)
        {
            try
            {
                await LoginAsync(); // Логинимся
                string[] messages;

                if (keywords != null && keywords.Length > 0)
                {
                    // Если заданы ключевые слова, фильтруем по ним
                    messages = await FetchMessagesWithKeywords(channelName, keywords, messageCount);
                }
                else
                {
                    // Получаем все сообщения без фильтрации
                    messages = await FetchMessagesFromChannel(channelName, messageCount);
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Класс для хранения результата
        public class ParsedResult
        {
            public DateTime Date { get; set; }
            public string SourceUrl { get; set; }
            public string Tag { get; set; }
            public string Text { get; set; }
        }

        public class TelegramPostViewModel
        {
            public string Channel { get; set; } // Название канала
            public string Text { get; set; } // Текст сообщения
            public DateTime Date { get; set; } // Дата сообщения
            public string Sentiment { get; set; } // Анализ настроения
            public List<CommentViewModel> Comments { get; set; } // Комментарии к сообщению
        }

        public class CommentViewModel
        {
            public string Text { get; set; } // Текст комментария
            public string Sentiment { get; set; } // Настроение комментария
            public string Author { get; set; } // Автор комментария
        }

        // Модель ответа для анализа настроений
        public class SentimentResponse
        {
            public string Label { get; set; }
        }
    }

}