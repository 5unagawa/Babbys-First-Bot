/*using System;
using Discord;
using Discord.Legacy;
using Discord.Commands;
using System.Collections.Generic;

namespace BabbysFirstBot
{
    class DiscordBot
    {
        static void Main(string[] args)
        {
            new DiscordBot().Start();
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

            var token = "MjY1ODMzNTI1Nzg0MDg0NDgw.C1OQrA.tldP0yAwWOgGTYrsnPEiuMzyC3Y";

            //Create and populate strikelist
            Dictionary<string, int> strikeList = new Dictionary<string, int>();
            Populate(ref strikeList);

            //Create client commands. Pass strike list so it can be used by bot
            CreateCommands(ref strikeList);

            //Connect Bot to Server
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
            });

            //Log Messages
            _client.Log.Message += (sender, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
            // [INFO] [Discord] Client Connected
        }

        public string Funpost(string fileName)
        {
            //Open file at given location and read each line into an array
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            Console.WriteLine($"File opened: {fileName}");
            string[] responses = System.IO.File.ReadAllLines(fileName);

            //Choose and return a random line
            var rand = new Random();
            int pos = rand.Next(0, responses.Length);
            file.Close();
            Console.WriteLine($"File closed: {fileName}");
            return responses[pos];
        }

        //Populate strike list using local file
        private void Populate(ref Dictionary<string, int> strikeList)
        {
            int count = 0;   //Count of line postion
            string line;   //Current line
            string pLine = ""; ;  //Previous line

            System.IO.StreamReader file = new System.IO.StreamReader("strikes.txt");
            Console.WriteLine("File opened: strikes.txt");
            while ((line = file.ReadLine()) != null)
            {
                if ((count % 2) != 0)
                {
                    strikeList.Add(pLine, Convert.ToInt32(line));
                    Console.WriteLine(pLine + " " + line);
                }
                pLine = line;
                count++;
            }
            System.Console.WriteLine("Strike list populated successfully.");
            file.Close();
        }

        public void CreateCommands(ref Dictionary<string, int> strikes)
        {
            string fileName;
            Dictionary<string, int> strikeList = strikes;
            var cService = _client.GetService<CommandService>();

            cService.CreateCommand("ping")
                .Description("Used to test bot response.")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("pong!");
                });

            cService.CreateCommand("dose")
                .Description("OH MAH GOH! THAT WAS A FAT DOSE ULEH!")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("vl.jpg");
                    await e.Channel.SendMessage("tsu tsu tsu");
                });

            cService.CreateCommand("8ball")
                .Description("Ask the mystical 8-ball a question.")
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

            cService.CreateCommand("strike")
                 .Description("!strike @user - Add strike against user. Once a user recieves 3 strikes, they will be kicked.")
                 .Parameter("StrikedUser", ParameterType.Required)
                 .MinPermissions((int)PermissionLevel.ServerModerator)
                 .Do(async e =>
                 {
                     var user = await _client.FindUsers(e, e.Args[0], e.Args[1]);


                     await e.Channel.SendMessage(e.GetArg("StrikedUser"));
                     var temp = e.GetArg("StrikedUser");
                     string userID = e.MentionedUser.User.Id.ToString();

                    //HipCheckEm can never do wrong! (Neither can the bot!)
                    /*if (userID == "191643641373130752" || userID =="265833525784084480")
                    {
                        await e.Channel.SendMessage("LMAO nice try!");
                    }

                    // else
                    //{

                    Console.WriteLine(e.GetArg("StrikedUser") + " received a strike.");
                     await e.Channel.SendMessage(e.GetArg("StrikedUser") + " has received a strike.\n"
                         + e.GetArg("StrikedUser") + $" has {strikeList[userID]} strikes.");

                     if (strikeList.ContainsKey(userID) == true)
                         {
                             strikeList[userID] += 1;
                             using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter("strikes.txt"))
                             {
                                 foreach (var entry in strikeList)
                                 {
                                     outputFile.WriteLine(entry.Key);
                                     outputFile.Write(entry.Value);
                                 }
                                 outputFile.Close();
                             }

                             if (strikeList[userID] > 3)
                             {
                                 Console.WriteLine(e.GetArg("StrikedUser") + " has been banned - Warning limit exceeded.");
                                 await e.Server.Ban(e.User, 1);
                                 await e.Channel.SendMessage(e.GetArg("StrikedUser") + " has been banned - Warning limit exceeded.");
                             }
                         }

                         else
                         {
                             string[] newUser = {userID, "1"};
                             using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter("strikes.txt"))
                             {
                                 foreach (string line in newUser)
                                 {
                                     outputFile.WriteLine(line);
                                 }
                                 outputFile.Close();
                             }

                             strikeList.Add(userID, 1);
                             Console.WriteLine(e.GetArgs("StrikedUser") + " added to list.");
                         }
                     //}
                    
                 }
                 );
        }
    }
}

//how will i save the list?
//rewrite every line or overwrite the one entry?

*/