    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
using VkNet.Model;
using WTelegram;
using TL;

    namespace VkPostsViewer.Controllers
    {
        [Route("telegram")]
        public class TelegramController : Controller
        {
            private readonly IMemoryCache _cache;
            private Client _telegramClient;
            private const string SessionFilePath = "telegram_session.bin"; // файл для хранения сессии
            private static List<string> Tags = new List<string>
        {
            "школьное питание",
            "департамент продовольствия",
            // Добавьте свои ключевые слова для фильтрации
        };

            public TelegramController(IMemoryCache cache)
            {
                _cache = cache;
            }

            // Метод для авторизации по номеру телефона
            [HttpGet("Login")]
            public async Task<IActionResult> Login(string phoneNumber)
            {
                if (string.IsNullOrEmpty(phoneNumber))
                    return BadRequest("Номер телефона обязателен для авторизации.");

                _telegramClient = new Client(Config);

                // Если существует сохраненная сессия, загружаем её
                if (System.IO.File.Exists(SessionFilePath))
                {
                   // _telegramClient.LoadSession(File.ReadAllBytes(SessionFilePath));
                    return Ok("Авторизация выполнена через сохранённую сессию.");
                }

                // Пытаемся авторизоваться через номер телефона
                try
                {
                    var sentCode = await _telegramClient.Login(phoneNumber);
                    return Ok(new { Message = "Введите код, отправленный в Telegram.", PhoneNumber = phoneNumber });
                }
                catch (Exception ex)
                {
                    return BadRequest($"Ошибка авторизации: {ex.Message}");
                }
            }

            // Метод для завершения авторизации с помощью кода, полученного из Telegram
            [HttpPost("ConfirmCode")]
            public async Task<IActionResult> ConfirmCode(string phoneNumber, string code)
            {
                try
                {
                    if (_telegramClient == null) _telegramClient = new Client(Config);
                    var sentCode = await _telegramClient.Login(phoneNumber);

                    if (sentCode.Type == WTelegram.Helpers.CodeType.SMS || sentCode.Type == WTelegram.Helpers.CodeType.Call)
                    {
                        // Код отправлен, ожидаем его ввода
                        return Ok(new { Message = "Введите код, отправленный на ваш телефон.", PhoneNumber = phoneNumber });
                    }

                    await _telegramClient.Login(code); // Завершаем авторизацию

                    // Сохраняем сессию в файл
                    //System.IO.File.WriteAllBytes(SessionFilePath, _telegramClient.SaveSession());
                    return Ok("Авторизация успешно завершена и сессия сохранена.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Ошибка подтверждения кода: {ex.Message}");
                }
            }

            // Основной метод для получения сообщений
            [HttpGet("GetFilteredNews")]
            public async Task<IActionResult> GetFilteredNews()
            {
                // Кэшируем результаты
                var cacheKey = "telegram_filtered_posts";
                List<ParsedResult> allFilteredResults;

                if (!_cache.TryGetValue(cacheKey, out allFilteredResults))
                {
                    // Загружаем сессию или выполняем авторизацию (если сессия отсутствует)
                    await EnsureTelegramClientIsAuthenticated();

                    // Получаем сообщения с каналов
                    allFilteredResults = await FetchTelegramPosts();

                    // Кэшируем результаты
                    _cache.Set(cacheKey, allFilteredResults, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                }

                return View("TelegramResults", allFilteredResults);
            }

            // Метод для парсинга сообщений из Telegram
            private async Task<List<ParsedResult>> FetchTelegramPosts()
            {
                List<ParsedResult> results = new List<ParsedResult>();

                var channels = new[] { "@your_channel_1", "@your_channel_2" }; // Укажите свои каналы

                foreach (var channel in channels)
                {
                    var chat = await _telegramClient.Contacts_ResolveUsername(channel); // Получаем информацию о канале
                var inputPeer = new InputPeerChannel { channel_id = chat.peer.ID, access_hash = chat.peer.access_hash };
                var messages = await _telegramClient.Messages_GetHistory(inputPeer, 0, 0, 100);
                    foreach (var message in messages.Messages)
                    {
                        if (message.message != null) // Только текстовые сообщения
                        {
                            var filteredTexts = FilterMessagesByTags(message.message, Tags);
                            foreach (var text in filteredTexts)
                            {
                                results.Add(new ParsedResult
                                {
                                    Date = message.date,
                                    SourceUrl = channel,
                                    Tag = Tags.FirstOrDefault(tag => text.Contains(tag, StringComparison.OrdinalIgnoreCase)),
                                    Text = text
                                });
                            }
                        }
                    }
                }

                return results;
            }

            // Метод для фильтрации сообщений по ключевым словам
            private List<string> FilterMessagesByTags(string messageText, List<string> tags)
            {
                return tags.Where(tag => messageText.Contains(tag, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Проверяем, авторизован ли клиент. Если сессия сохранена, загружаем её
            private async Task EnsureTelegramClientIsAuthenticated()
            {
                if (_telegramClient == null)
                    _telegramClient = new Client(Config);

                if (!System.IO.File.Exists(SessionFilePath))
                {
                    throw new Exception("Сессия не найдена. Пожалуйста, выполните авторизацию.");
                }

               // _telegramClient.LoadSession(System.IO.File.ReadAllBytes(SessionFilePath));
            }

            // Конфигурация Telegram API для WTelegramClient
            private static string Config(string what)
            {
                return what switch
                {
                    "api_id" => "26908424",              // Замените на ваш API ID
                    "api_hash" => "9d40c54db3163c9141ef097b97415d37",          // Замените на ваш API HASH
                    _ => null
                };
            }

            // Класс для хранения результата
            public class ParsedResult
            {
                public DateTime Date { get; set; }
                public string SourceUrl { get; set; }
                public string Tag { get; set; }
                public string Text { get; set; }
            }
        }
    }
