using System;
using Discord;
using Discord.Commands;

namespace BabbysFirstBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient(x =>
            {
                x.AppName = "Babby's First Bot";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!'; //prefix needed to activate bot commands
                x.HelpMode = HelpMode.Public;
            });

            CreateCommands();

            var token = "";

            //Connect Bot to Server
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
            });
        }

        public void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();
            cService.CreateCommand("ping")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("What do you want?");
                });

            cService.CreateCommand("dose")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("vl.jpg");
                    await e.Channel.SendMessage("tsu tsu tsu");
                });
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
            // [INFO] [Discord] Client Connected
        }
    }
}
