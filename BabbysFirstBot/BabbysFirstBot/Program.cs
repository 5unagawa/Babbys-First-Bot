using BabbysFirstBot.Modules.Admin;
using BabbysFirstBot.Modules.Game;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using System;

namespace BabbysFirstBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().Start(args);
        private const string AppName = "Babby's First Bot";
        private string APP_TOKEN = "";

        private static DiscordClient _client;
        private AdminModule _admin;     //Bot Module for user maangement
        private GameModule _game;       //Bot Module for "fun stuff"
        
        private void Start(string[] args)
        {   
#if !DNXCORE50
            Console.Title = $"{AppName} (Discord.Net v{DiscordConfig.LibVersion})";
#endif
            
            _client = new DiscordClient( x=>
            {
                x.AppName = AppName;
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = OnLogMessage;
            })
            .UsingCommands(x =>
            {
               x.AllowMentionPrefix = true;
               x.HelpMode = HelpMode.Public;
               x.ExecuteHandler = OnCommandExecuted;
               x.ErrorHandler = OnCommandError;
               x.PrefixChar = '~'; //prefix needed to activate bot commands
            })
            .UsingPermissionLevels(PermissionLevelResolver);

            //Begin loggine messages
            _client.Log.Message += (sender, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            //Initialise Modules
            _admin = new AdminModule(_client);
            _game = new GameModule(_client);

            OnJoin(_client);
            OnLeave(_client);

            //Connect Bot to Server
            _client.ExecuteAndWait(async () =>
            {
                    try
                    {
                        await _client.Connect(APP_TOKEN, TokenType.Bot);
                        _client.SetGame("ur mum");
                    }
                    catch (Exception ex)
                    {
                        _client.Log.Error($"Login Failed", ex);
                    }
            });
        }

        static private void OnCommandError(object sender, CommandErrorEventArgs e)
        {
            string msg = e.Exception?.Message;
            if (msg == null)
            {
                switch (e.ErrorType)
                {
                    case CommandErrorType.Exception:
                        msg = "Unknown Error";
                        break;
                    case CommandErrorType.BadPermissions:
                        msg = "You do not have permission to execute this command.";
                        break;
                    case CommandErrorType.BadArgCount:
                        msg = "Incorrect number of arguments for this command.";
                        break;
                    case CommandErrorType.InvalidInput:
                        msg = "Invalid input.";
                        break;
                    case CommandErrorType.UnknownCommand:
                        msg = "Unknown Command.";
                        break;
                }
            }
            if (msg != null)
            {
                //_client.ReplyError(e, msg);
                _client.Log.Error("Command", msg);
            }
        }

        static private void OnCommandExecuted(object sender, CommandEventArgs e)
        {
            _client.Log.Info("Command", $"{e.Command.Text} ({e.User.Name})");
        }

        static private void OnJoin(DiscordClient myClient)
        {
            _client.UserJoined += async (s, e) =>
            {
                var channel = myClient.GetChannel(265825030187778048);
                await channel.SendMessage(e.User.Mention + " has joined!");
            };
        }

        static private void OnLeave(DiscordClient myClient)
        {
            _client.UserLeft += async (s, e) =>
            {
                var channel = myClient.GetChannel(265825030187778048);
                await channel.SendMessage(e.User.Mention + " has left!");
            };
        }

        static private void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            //Message colour
            ConsoleColor colour;
            switch (e.Severity)
            {
                case LogSeverity.Debug:
                default:
                    colour = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    colour = ConsoleColor.Red;
                    break;
                case LogSeverity.Info:
                    colour = ConsoleColor.Blue;
                    break;
                case LogSeverity.Verbose:
                    colour = ConsoleColor.Gray;
                    break;
                case LogSeverity.Warning:
                    colour = ConsoleColor.Yellow;
                    break;
            }
        }

        static private int PermissionLevelResolver(User user, Channel channel)
        {
            if (user.Id == 191643641373130752) //That's me!
                return (int)PermissionLevel.BotOwner;
            if (user.Server != null)
            {
                if (user == channel.Server.Owner)
                    return (int)PermissionLevel.ServerOwner;

                var serverPerms = user.ServerPermissions;
                if (serverPerms.ManageRoles)
                    return (int)PermissionLevel.ServerAdmin;
                if (serverPerms.ManageMessages && serverPerms.KickMembers && serverPerms.BanMembers)
                    return (int)PermissionLevel.ServerModerator;

                var channelPerms = user.GetPermissions(channel);
                if (channelPerms.ManagePermissions)
                    return (int)PermissionLevel.ChannelAdmin;
                if (channelPerms.ManageMessages)
                    return (int)PermissionLevel.ChannelModerator;
            }
            return (int)PermissionLevel.User;
        }
    }
}
