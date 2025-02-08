using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramCardsBot
{
    public class WordService(BotInitializer initializer, MenuService menuService)
    {
        private readonly TelegramBotClient _botClient = initializer.botClient;
        private readonly JObject _data = initializer.data;
        private readonly Dictionary<long, string> _userState = initializer.userState;
        private readonly MenuService _menuService = menuService;

        public async Task DrawCard(Message msg)
        {
            var wordsArray = (JArray)_data["words"];
            if (wordsArray.Count == 0)
            {
                await _menuService.ShowOneOption("Список слов пуст", msg.Chat.Id, msg.MessageId);
                return;
            }

            var rnd = new Random().Next(wordsArray.Count);
            var rndWord = _data["words"][rnd]["word"].ToString();
            var rndWordTranslate = _data["words"][rnd]["translate"].ToString();

            var inlineMarkup = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Тянуть", "Тянуть")],
            [
                InlineKeyboardButton.WithCallbackData("Перевод", $"translate_{rndWordTranslate}"),
                InlineKeyboardButton.WithCallbackData("Назад", "Назад")
            ]
            ]);

            await _botClient.EditMessageText(
                msg.Chat.Id,
                msg.MessageId,
                $"Ваше слово: {rndWord}",
                replyMarkup: inlineMarkup);

        }

        public async Task CreateCard(Message msg)
        {
            var inlineMarkup = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Назад", "Назад")]
            ]);

            await _botClient.EditMessageText(
                msg.Chat.Id,
                msg.MessageId,
                "Напишите слово на английском",
                replyMarkup: inlineMarkup);

            _userState[msg.Chat.Id] = "awaiting_word";
        }

        public async Task RemoveCard(Message msg)
        {
            var wordsArray = (JArray)_data["words"];
            if (wordsArray.Count == 0)
            {
                await _menuService.ShowOneOption("Список слов пуст", msg.Chat.Id, msg.MessageId);
                return;
            }

            var inlineMarkup = new InlineKeyboardMarkup(
                wordsArray.Select(word => new[]
                {
            InlineKeyboardButton.WithCallbackData(word["word"].ToString(), $"del_{word["word"]}")
                }).Append([InlineKeyboardButton.WithCallbackData("Назад", "Назад")]));

            await _botClient.EditMessageText(
                msg.Chat.Id,
                msg.MessageId,
                "Выберите слово для удаления:",
                replyMarkup: inlineMarkup);
        }

        public async Task RemoveData(CallbackQuery query)
        {
            ((JArray)_data["words"]).Where(word => word["word"].ToString() == query.Data[4..]).First().Remove();
            await File.WriteAllTextAsync(Config.DataFilePath, _data.ToString());

            await RemoveCard(query.Message);
        }
    }
}
