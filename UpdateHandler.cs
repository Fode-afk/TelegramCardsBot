using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramCardsBot
{
    public class UpdateHandler
    {
        private readonly MenuService _menuService;
        private readonly WordService _wordService;
        private readonly MessageHandler _messageHandler;

        public UpdateHandler(MenuService menuService, WordService wordService, MessageHandler messageHandler)
        {
            _menuService = menuService;
            _wordService = wordService;
            _messageHandler = messageHandler;
        }
        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Message is { } message && message.Text == "/start")
            {
                await _menuService.ShowMainMenu(message.Chat.Id);
            }
            else if (update.Message is { } userMessage)
            {
                await _messageHandler.ProcessUserInput(userMessage);
            }
            else if (update.CallbackQuery is { } query)
            {
                await HandleCallbackQuery(query);
            }
        }

        public async Task HandleCallbackQuery(CallbackQuery query)
        {
            if (query.Message == null)
            {
                Console.WriteLine("Ошибка: CallbackQuery не содержит Message.");
                return;
            }
            if (query.Data == "Тянуть")
            {
                await _wordService.DrawCard(query.Message);
            }
            else if (query.Data == "Создать")
            {
                await _wordService.CreateCard(query.Message);
            }
            else if (query.Data == "Удалить")
            {
                await _wordService.RemoveCard(query.Message);
            }
            else if (query.Data == "Назад")
            {
                await _menuService.ShowMainMenu(query.Message.Chat.Id, query.Message.MessageId);
            }
            else if (query.Data.StartsWith("translate"))
            {
                await _menuService.ShowTranslate(query);
            }
            else if (query.Data.StartsWith("del"))
            {
                await _wordService.RemoveData(query);
            }
        }
    }
}
