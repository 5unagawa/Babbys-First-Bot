using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;
using Discord.Legacy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabbysFirstBot.Modules.Admin
{
    ///<summary> This client enables user management from chat. </summary>
    public class AdminModule
    {
        public DiscordClient adminClient;
        public CommandService cService;
        Dictionary<string, int> strikeList;

        /// <summary> Constructor for the AdminModule class </summary>
        /// <param name="myClient"> The current Discord client. </param>
        public AdminModule(DiscordClient myClient)
        {
            this.cService = myClient.GetService<CommandService>();
            this.adminClient = myClient;
            strikeList = new Dictionary<string, int>();
            Populate(ref strikeList);
            CreateAdminCommands(ref strikeList);
            System.Console.WriteLine("Admin Module Initialised");
        }

        /// Populate strike list using local file
        private void Populate(ref Dictionary<string, int> strikeList)
        {
            int count = 0;      //Count of line position
            string line;        //Current line
            string pLine = "";  //Previous line

            System.IO.StreamReader file = new System.IO.StreamReader("strikes.txt");
            System.Console.WriteLine("File opened: strikes.txt \n Users found:");
            while ((line = file.ReadLine()) != null)
            {
                if ((count % 2) != 0)
                {
                    strikeList.Add(pLine, System.Convert.ToInt32(line));
                    System.Console.WriteLine(pLine + " " + line);
                }
                pLine = line;
                count++;
            }
            System.Console.WriteLine("Strike list populated successfully.");
            file.Close();
        }
 
        public void CreateAdminCommands(ref Dictionary<string, int> strikes)
        {
            Dictionary<string, int> strikeList = strikes;
            cService.CreateCommand("strike")
                .Description("~strike @user | Add strike against user. Once a user recieves 3 strikes, they will be kicked.")
                .Parameter("struckUser", ParameterType.Required)
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    try
                    {
                        if (e.User.ServerPermissions.KickMembers)
                        {
                            User struckUser = null;
                            try
                            {
                                //find user
                                struckUser = e.Server.FindUsers(e.GetArg("struckUser")).First();
                            }
                            catch (System.Exception ex)
                            {
                                await e.Channel.SendMessage(ex.ToString());
                            }

                            //HipCheckEm can never do wrong! (Neither can the bot!)
                            if (struckUser.Id == 191643641373130752 || struckUser.Id == 265833525784084480)
                            {
                                await e.Channel.SendMessage("Unable to process request.");
                            }
                            else
                            {
                                {
                                    string suID = struckUser.Id.ToString();
                                    System.Console.WriteLine(e.GetArg("struckUser") + " received a strike.");
                                   
                                    if (strikeList.ContainsKey(suID) == true)
                                    {
                                        await e.Channel.SendMessage(e.GetArg("struckUser") + " has received a strike.\n"
                                              + e.GetArg("struckUser") + $" has {strikeList[suID]} strikes.");

                                        strikeList[suID] += 1;
                                        using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter("strikes.txt"))
                                        {
                                            foreach (var entry in strikeList)
                                            {
                                                outputFile.WriteLine(entry.Key);
                                                outputFile.Write(entry.Value);
                                            }

                                            outputFile.Close();
                                        }

                                        if (strikeList[suID] > 3)
                                        {   
                                            await e.Channel.SendMessage(e.GetArg("stuckUser") + " has been banned - Warning limit exceeded.");
                                            System.Console.WriteLine(e.GetArg("struckUser") + " has been banned - Warning limit exceeded.");
                                            await e.Server.Ban(struckUser);
                                        }
                                    }

                                    else
                                    {
                                        string[] newUser = { suID, "1" };
                                        using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter("strikes.txt"))
                                        {
                                            foreach (string line in newUser)
                                            {
                                                outputFile.WriteLine(line);
                                            }
                                            outputFile.Close();
                                        }

                                        strikeList.Add(suID, 1);
                                        System.Console.WriteLine(e.GetArg("struckUser") + " added to list.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to perform this action.");
                        }
                    }
                    catch (System.Exception exptn)
                    {
                        await e.Channel.SendMessage(exptn.ToString());
                    }
                });

            cService.CreateCommand("destrike")
                .Description("~destrike @user | Remove a strike against user.")
                .Parameter("struckUser", ParameterType.Required)
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    try
                    {
                        if (e.User.ServerPermissions.KickMembers)
                        {
                            User struckUser = null;
                            try
                            {
                                //find user
                                struckUser = e.Server.FindUsers(e.GetArg("struckUser")).First();
                            }
                            catch (System.Exception ex)
                            {
                                await e.Channel.SendMessage(ex.ToString());
                            }
                            
                            string suID = struckUser.Id.ToString();

                            //remove strike is user is on list
                            if (strikeList.ContainsKey(suID) == true)
                            {
                                strikeList[suID] -= 1;
                                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter("strikes.txt"))
                                {
                                    foreach (var entry in strikeList)
                                    {
                                        outputFile.WriteLine(entry.Key);
                                        outputFile.Write(entry.Value);
                                    }
                                    
                                    outputFile.Close();
                                }
                                await e.Channel.SendMessage("Removed strike against " + e.GetArg("struckUser")
                                      + $"\n User has {strikeList[suID]} strikes.");
                            }

                            else
                            {
                                System.Console.WriteLine(e.GetArg("struckUser") + " does not have any strikes against them.");
                            }
                        }
                            
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to perform this action.");
                        }
                    }
                    catch (System.Exception exptn)
                    {
                        await e.Channel.SendMessage(exptn.ToString());
                    }
                });

            cService.CreateCommand("ban")
                .Description("~ban @user x | Ban mentioned user. Optional: Ban for x minutes.")
                .Parameter("bannedUser", ParameterType.Required)
                .Parameter("banTime", ParameterType.Optional)
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    try 
                    { 
                        if (e.User.ServerPermissions.ManageRoles)
                        {
                            User bannedUser = null;
                            int banLength = 0;
                            Role bannedRole = e.Server.FindRoles("Banned").First();
                            
                            try
                            {
                                //Find user and assign "Banned role"
                                bannedUser = e.Server.FindUsers(e.GetArg("bannedUser")).First();
                                await bannedUser.AddRoles(bannedRole);


                                //Maintain ban for specified time
                                //Validate format
                                if (int.TryParse(e.GetArg("banTime"), out banLength))
                                {
                                    System.Threading.Thread.Sleep(banLength * 60000);
                                    await bannedUser.RemoveRoles(bannedRole);
                                }
                            }

                            catch (System.Exception ex)
                            {
                                await e.Channel.SendMessage(ex.ToString());
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to perform this action.");
                        }
                    }

                    catch (System.Exception exptn)
                    {
                        await e.Channel.SendMessage(exptn.ToString());
                    }
                });

            cService.CreateCommand("unban")
                .Description("~unban @user | Unban mentioned user.")
                .Parameter("bannedUser", ParameterType.Required)
                .MinPermissions((int)PermissionLevel.ServerModerator)
                .Do(async e =>
                {
                    try
                    {
                        if (e.User.ServerPermissions.ManageRoles)
                        {
                            User bannedUser = null;
                            Role bannedRole = e.Server.FindRoles("Banned").First();

                            try
                            {
                                //Find user and assign "Banned role"
                                bannedUser = e.Server.FindUsers(e.GetArg("bannedUser")).First();
                                await bannedUser.RemoveRoles(bannedRole);
                            }

                            catch (System.Exception ex)
                            {
                                await e.Channel.SendMessage(ex.ToString());
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to perform this action.");
                        }
                    }

                    catch (System.Exception exptn)
                    {
                        await e.Channel.SendMessage(exptn.ToString());
                    }
                });

                //
        }
    }
}
 