using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DeepMorphy;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PostsViewer.Controllers;
using VkNet.Model;

namespace VkPostsViewer.Controllers
{
    [Route("SiteParser")]
    public class SiteParserController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> SiteUrls = new List<string>
        {
            "https://realnoevremya.ru/",
            "https://m.business-gazeta.ru/",
            "https://www.tatar-inform.ru/",
            "https://kazanfirst.ru/", 
            "https://inkazan.ru/",
            "https://prokazan.ru/",
            "https://kazved.ru/",
            "https://www.evening-kazan.ru/",
            "https://tatarstan24.tv/",
            "https://russia-tv.online/efir?region=16",
            "https://tnv.ru/",
            "https://trt-tv.ru/" 
        };

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

        private Morphy AnalizerMorphy = new Morphy();
        private readonly HttpClient _httpClient; // Для взаимодействия с API DeepPavlov

        private static List<string> TagsFiltered = new List<string>();


        public SiteParserController(IMemoryCache cache, HttpClient httpClient)
        {
            _cache = cache;
            _httpClient = httpClient;
        }

        [HttpGet("GetParsedData")]
        public async Task<IActionResult> GetParsedData()
        {
            var cacheKey = "filteredTexts";
            List<ParsedResult> allFilteredResults;
            TagsFiltered = await PerformMorphologicalAnalysis(Tags);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!_cache.TryGetValue(cacheKey, out allFilteredResults))
            {
                allFilteredResults = new List<ParsedResult>();

                var fetchTasks = SiteUrls.Select(FetchAndProcessSiteContent).ToArray();
                var siteResults = await Task.WhenAll(fetchTasks);

                foreach (var result in siteResults)
                {
                    allFilteredResults.AddRange(result);
                }

                // Убираем дубликаты по SourceUrl и Tag
                allFilteredResults = allFilteredResults
                    .GroupBy(r => new { r.SourceUrl, r.Tag, r.Text })
                    .Select(g => g.First())
                    .ToList();

                // Кешируем результаты
                _cache.Set(cacheKey, allFilteredResults, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            stopwatch.Stop();
            ViewBag.ParsingTime = stopwatch.Elapsed.TotalSeconds;

            return Json(new { results = allFilteredResults, parsingTime = ViewBag.ParsingTime });
        }

        private async Task<List<ParsedResult>> FetchAndProcessSiteContent(string siteUrl)
        {
            var htmlContent = await FetchHtmlContent(siteUrl);
            var allUrls = ExtractUrlsFromHtml(htmlContent, siteUrl);

            var allFilteredResults = new List<ParsedResult>();
            try
            {
                foreach (var url in allUrls)
                {
                    if (SiteUrls.Any(site => url.StartsWith(site, StringComparison.OrdinalIgnoreCase)))
                    {
                        try
                        {
                            var content = await FetchHtmlContent(url);
                            var extractedTexts = ExtractTextFromHtml(content);
                            var filteredTexts = FilterTextsByTags(extractedTexts, TagsFiltered);

                            foreach (var text in filteredTexts)
                            {
                                var publishDate = ExtractPublishDate(content) ?? DateTime.Now; // Время публикации
                                var sentiment = await AnalyzeSentiment(text); // Асинхронный вызов анализа настроений
                                allFilteredResults.Add(new ParsedResult
                                {
                                    Date = publishDate, // Устанавливаем извлечённое время публикации
                                    SourceUrl = url,
                                    Tag = Tags.First(tag => text.Contains(tag, StringComparison.OrdinalIgnoreCase)),
                                    Text = text,
                                    Sentiment = sentiment
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching content from {url}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching site {siteUrl}: {ex.Message}");
            }

            return allFilteredResults;
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

        private async Task<string> FetchHtmlContent(string url)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке страницы {url}: {ex.Message}");
                return string.Empty;
            }
        }

        // Извлекаем все ссылки со страницы
        private List<string> ExtractUrlsFromHtml(string htmlContent, string baseUrl)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var links = document.DocumentNode.SelectNodes("//a[@href]")
                ?.Select(node => node.GetAttributeValue("href", string.Empty))
                .Where(href => !string.IsNullOrWhiteSpace(href))
                .Distinct()
                .Select(href => href.StartsWith("http") ? href : new Uri(new Uri(baseUrl), href).ToString())
                .ToList();

            return links ?? new List<string> { baseUrl };
        }

        // Извлекаем текст из HTML
        private List<string> ExtractTextFromHtml(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var nodes = document.DocumentNode.SelectNodes("//p | //div");
            if (nodes == null) return new List<string>();

            var extractedTexts = nodes
                .Select(node => node.InnerText.Trim())
                .Where(text => !string.IsNullOrEmpty(text))
                .ToList();

            return extractedTexts;
        }

        private async Task<List<string>> PerformMorphologicalAnalysis(List<string> text)
        {
            List<string> newText = new List<string>();

            foreach (var morph in text)
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

        private List<string> FilterTextsByTags(IEnumerable<string> texts, List<string> tags)
        {
            return texts.Where(text =>
                tags.Any(tag => text.Contains(tag, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Функция для извлечения даты публикации
        private DateTime? ExtractPublishDate(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // Пример поиска мета-тегов с датой публикации
            var metaDate = document.DocumentNode.SelectSingleNode("//meta[@property='article:published_time']");
            if (metaDate != null)
            {
                var content = metaDate.GetAttributeValue("content", "");
                if (DateTime.TryParse(content, out DateTime publishDate))
                {
                    return publishDate;
                }
            }

            return null;
        }

        public class ParsedResult
        {
            public DateTime Date { get; set; }
            public string SourceUrl { get; set; }
            public string Tag { get; set; }
            public string Text { get; set; }
            public string Sentiment { get; set; }
        }
    }
}
