using MCQs_generator.model;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCQs_generator.service
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyBTwLk5bI_4i_Il_L-xcjqCpkLfNkWbmrA";

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<mcq>> GetMCQsFromGeminiAsync(string topic)
        {
            // Ask Gemini to return plain format
            var prompt = $"Generate 50 MCQs on {topic} in EXACT format with no markdown or extra text:\n" +
                         "Question: ...\nA) ...\nB) ...\nC) ...\nD) ...\nCorrect Answer: A/B/C/D";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(responseText);
            var mcqText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            Console.WriteLine("=== RAW GEMINI TEXT ===");
            Console.WriteLine(mcqText);
            Console.WriteLine("========================");

            mcqText = mcqText.Replace("\\n", "\n").Replace("**", "").Trim();

            var mcqs = new List<mcq>();

            // Flexible MCQ block splitter
            var entries = Regex.Split(mcqText, @"(?=Question:)", RegexOptions.IgnoreCase);

            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry)) continue;

                var q = new mcq();

                // Match using flexible regex
                var questionMatch = Regex.Match(entry, @"Question:\s*(.+)", RegexOptions.IgnoreCase);
                var a = Regex.Match(entry, @"A[\).]\s*(.+)", RegexOptions.IgnoreCase);
                var b = Regex.Match(entry, @"B[\).]\s*(.+)", RegexOptions.IgnoreCase);
                var c = Regex.Match(entry, @"C[\).]\s*(.+)", RegexOptions.IgnoreCase);
                var d = Regex.Match(entry, @"D[\).]\s*(.+)", RegexOptions.IgnoreCase);
                var ans = Regex.Match(entry, @"Correct\s*Answer:\s*([ABCD])", RegexOptions.IgnoreCase);

                if (questionMatch.Success && a.Success && b.Success && c.Success && d.Success && ans.Success)
                {
                    q.Question = questionMatch.Groups[1].Value.Trim();
                    q.OptionA = a.Groups[1].Value.Trim();
                    q.OptionB = b.Groups[1].Value.Trim();
                    q.OptionC = c.Groups[1].Value.Trim();
                    q.OptionD = d.Groups[1].Value.Trim();
                    q.CorrectAnswer = ans.Groups[1].Value.Trim();

                    mcqs.Add(q);

                    // Debug print each MCQ
                    Console.WriteLine($"   Q: {q.Question}");
                    Console.WriteLine($"   A) {q.OptionA}");
                    Console.WriteLine($"   B) {q.OptionB}");
                    Console.WriteLine($"   C) {q.OptionC}");
                    Console.WriteLine($"   D) {q.OptionD}");
                    Console.WriteLine($"   Correct: {q.CorrectAnswer}\n");
                }
            }

            Console.WriteLine($" Total MCQs Parsed: {mcqs.Count}");
            return mcqs;
        }
    }
}
