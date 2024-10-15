using DeepMorphy;

namespace PostsViewer.Controllers
{
    public class SentimentResult
    {
        public string Label { get; set; }  // Тональность (положительная/отрицательная/нейтральная)
        public float Score { get; set; }   // Уверенность модели в ответе
    }

    public class Morphy
    {
        public MorphAnalyzer Analyzer { get; set; }

        public Morphy()
        {
            Analyzer = new MorphAnalyzer(withLemmatization: true);
        }

        private async Task<List<string>> PerformMorphologicalAnalysis(List<string> text)
        {
            List<string> newText = new List<string>();

            List<string> TagsFiltered = new List<string>();

            foreach (var morph in text)
            {
                var a = morph.Split(' ').ToList();
                foreach (var b in a)
                {
                    if (b.Length > 4)
                        newText.Add(b);
                }
            }
            var reluts = Analyzer.Parse(newText).ToList();

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
}

