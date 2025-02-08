using Newtonsoft.Json.Linq;

namespace TelegramCardsBot
{
    class Translator
    {
        private static readonly HttpClient client = new();
        private const string apiUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";

        public static async Task<string> TranslateText(string text)
        {
            string url = string.Format(apiUrl, "en", "ru", Uri.EscapeDataString(text));
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Ошибка при получении данных от API");
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            JArray jsonResponse = JArray.Parse(responseBody);
            string translatedText = jsonResponse[0][0][0].ToString();

            return translatedText;
        }
    }
}
