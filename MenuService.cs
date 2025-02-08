using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramCardsBot
{
    public class MenuService(TelegramBotClient botClient)
    {
        private readonly TelegramBotClient _botClient = botClient;

        public async Task ShowMainMenu(long chatId, int? messageId = null)
        {
            var inlineMarkup = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Тянуть", "Тянуть")],
            [
                InlineKeyboardButton.WithCallbackData("Создать", "Создать"),
                InlineKeyboardButton.WithCallbackData("Удалить", "Удалить")
            ]
            ]);

            if (messageId != null)
            {
                await _botClient.EditMessageText(chatId, messageId.Value, "Выберите одну из опций ниже", replyMarkup: inlineMarkup);
            }
            else
            {
                await _botClient.SendMessage(chatId, "Выберите одну из опций ниже", replyMarkup: inlineMarkup);
            }
        }

        public async Task ShowTranslate(CallbackQuery query)
        {
            string translate = query.Data.Split('_')[1];
            await ShowOneOption("Перевод слова: " + translate, query.Message.Chat.Id, query.Message.MessageId);
        }

        public async Task ShowOneOption(string text, long chatId, int? messageId = null)
        {
            await _botClient.EditMessageText(
                   chatId,
                   messageId.Value,
                   text,
                   replyMarkup: new InlineKeyboardMarkup(
                       InlineKeyboardButton.WithCallbackData("Назад", "Назад")
                       ));
            return;
        }
    }
}
