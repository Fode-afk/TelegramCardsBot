using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TelegramCardsBot
{
    public class BotInitializer
    {
        public readonly CancellationTokenSource cts = new();
        public JObject data;
        public JObject config;
        public string botToken;
        public TelegramBotClient botClient;
        private readonly ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = true
        };
        public readonly Dictionary<long, string> userState = [];

        public async Task InitializeAsync()
        {
            await LoadConfigAsync();
            botClient = new TelegramBotClient(botToken, cancellationToken: cts.Token);

            var menuService = new MenuService(botClient);
            var updateHandler = new UpdateHandler(
                menuService,
                new WordService(this, menuService),
                new MessageHandler(this, menuService)
            );

            var me = await botClient.GetMe();
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

            botClient.StartReceiving(updateHandler.HandleUpdate, HandleError, receiverOptions, cts.Token);
            Console.ReadLine();
            cts.Cancel();
        }

        private async Task LoadConfigAsync()
        {
            data = JObject.Parse(await File.ReadAllTextAsync(Config.DataFilePath));
            config = JObject.Parse(await File.ReadAllTextAsync(Config.ConfigFilePath));
            botToken = config["BotToken"].ToString();
        }

        private static Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken token)
        {
            ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();
            logger.LogError(exception, "Ошибка обработки бота");
            return Task.CompletedTask;
        }
    }
}

