using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;
using Discord.Legacy;
using System.Collections.Generic;

namespace BabbysFirstBot.Modules.Game
{
    ///<summary> This client enables user management from chat. </summary>
    public class GameModule
    {
        public DiscordClient currentClient;
        public CommandService cService;

        /// <summary> Constructor for the AdminModule class </summary>
        /// <param name="myClient"> The current Discord client. </param>
        public GameModule(DiscordClient myClient)
        {
            this.cService = myClient.GetService<CommandService>();
            this.currentClient = myClient;
            CreateGameCommands();
            System.Console.WriteLine("Game Module Initialised");
        }

        public void CreateGameCommands()
        {
            string fileName;
            cService.CreateCommand("dose")
               .Description("OH MAH GOD! THAT WAS A FAT DOSE ULEH!")
               .Do(async (e) =>
               {
                   await e.Channel.SendFile("vl.jpg");
                   await e.Channel.SendMessage("tsu tsu tsu");
               });

            cService.CreateCommand("8ball")
                .Description("Ask the mystical 8-ball a question.")
                .Parameter("question", ParameterType.Optional)
                .Do(async (e) =>
                {
                    fileName = "8ball.txt";
                    await e.Channel.SendMessage(Funpost(fileName));
                });

            cService.CreateCommand("wew")
                .Description("Randomly selects a shitpost for your amusement.")
                .Do(async (e) =>
                {
                    fileName = "shitposts.txt";
                    await e.Channel.SendMessage(Funpost(fileName));
                });
        }

        /// <summary> Reads the given text file and returns a random line. </summary>
        public string Funpost(string fileName)
        {
            //Open file at given location and read each line into an array
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            System.Console.WriteLine($"File opened: {fileName}");
            string[] responses = System.IO.File.ReadAllLines(fileName);

            //Choose and return a random line
            var rand = new System.Random();
            int pos = rand.Next(0, responses.Length);
            file.Close();
            System.Console.WriteLine($"File closed: {fileName}");
            return responses[pos];
        }
    }
}
