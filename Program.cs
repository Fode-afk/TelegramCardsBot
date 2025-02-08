namespace TelegramCardsBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BotInitializer bot = new();
            await bot.InitializeAsync();
        }
    }
}

