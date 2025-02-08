using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramCardsBot
{
    public class MessageHandler(BotInitializer initializer, MenuService menuService)
    {
        private readonly TelegramBotClient _botClient = initializer.botClient;
        private readonly JObject _data = initializer.data;
        private readonly Dictionary<long, string> _userState = initializer.userState;
        private readonly MenuService _menuService = menuService;

        public async Task ProcessUserInput(Message msg)
        {
            if (_userState.TryGetValue(msg.Chat.Id, out var state))
            {
                if (state == "awaiting_word")
                {
                    string word = msg.Text.ToLower();
                    ((JArray)_data["words"]).Add(new JObject { ["id"] = msg.MessageId, ["word"] = word, ["translate"] = Translator.TranslateText(word).Result });
                    File.WriteAllText(Config.DataFilePath, _data.ToString());

                    await _botClient.SendMessage(msg.Chat.Id, "Ваше слово было успешно добавлено!");
                    _userState.Remove(msg.Chat.Id);
                    await _menuService.ShowMainMenu(msg.Chat.Id);
                }
            }
        }
    }
}
