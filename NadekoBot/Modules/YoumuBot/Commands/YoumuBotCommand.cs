using Discord.Commands;
using NadekoBot.Extensions;
using NadekoBot.Modules;
using NadekoBot.Modules.Permissions.Classes;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace NadekoBot.Classes.YoumuBot.Commands
{
    internal class YoumuBotCommand : DiscordCommand
    {

        XmlSerializer mySerializer = new XmlSerializer(typeof(User));
        // To write to a file, create a StreamWriter object.  

        public static string DMHelpString => NadekoBot.Config.DMHelpString;

        private User user;
        public List<User> users = new List<User>();

        public Action<CommandEventArgs> DoGitFunc() => e =>
        {
            var helpstr = new StringBuilder();

            var lastCategory = "";
            foreach (var com in NadekoBot.Client.GetService<CommandService>().AllCommands)
            {
                if (com.Category != lastCategory)
                {
                    helpstr.AppendLine("\n### " + com.Category + "  ");
                    helpstr.AppendLine("Command and aliases | Description | Usage");
                    helpstr.AppendLine("----------------|--------------|-------");
                    lastCategory = com.Category;
                }
                helpstr.AppendLine($"`{com.Text}`{string.Concat(com.Aliases.Select(a => $", `{a}`"))} | {com.Description}");
            }
            helpstr = helpstr.Replace(NadekoBot.BotMention, "@BotName");
#if DEBUG
            File.WriteAllText("../../../docs/Commands List.md", helpstr.ToString());
#else
            File.WriteAllText("commandlist.md", helpstr.ToString());
#endif
        };

        internal override void Init(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand(Module.Prefix + "test")
                .Alias(Module.Prefix + "guide")
                .Description($"Sends a readme and a guide links to the channel. | `{Prefix}readme` or `{Prefix}guide`")
                .Do(async e =>
                    await e.Channel.SendMessage("Test Successful").ConfigureAwait(false));

            cgb.CreateCommand(Module.Prefix + "setrealname")
                .Alias(Module.Prefix + "iam")
                .Description($"Sets your real life name. | `{Prefix}setrealname` or `{Prefix}iam`")
                .Parameter("realname", ParameterType.Unparsed)
                .Do(async e =>
                {
                    User user = new User();
                    user.uid = e.User.Id;
                    user.username = e.User.Name;
                    user.realname = e.GetArg("realname");

                    var result = await SerializeUserDataAsync(user);

                    await e.Channel.SendMessage(result).ConfigureAwait(false);
                });

            cgb.CreateCommand(Module.Prefix + "realname")
                .Alias(Module.Prefix + "whois")
                .Description($"Displays the real name of a mentioned user. | `{Prefix}realname` or `{Prefix}whois`")
                .Parameter("username", ParameterType.Required)
                .Do(async e =>
                    {
                        string resp = "Could not find the real name of that user.";
                        User user = await DeserializeUserDataAsync(e.GetArg("username"));
                        if (!string.IsNullOrWhiteSpace(user.realname))
                        {
                            resp = $"Real name of {user.username} is {user.realname}.";
                        }
                        await e.Channel.SendMessage(resp).ConfigureAwait(false);
                    });

        }

        private static string PrintCommandHelp(Command com)
        {
            var str = "`" + com.Text + "`";
            str = com.Aliases.Aggregate(str, (current, a) => current + (", `" + a + "`"));
            str += " **Description:** " + com.Description + "\n";
            return str;
        }

        private Task<string> SerializeUserDataAsync(User user)
        {
            return Task.Factory.StartNew(() => SerializeUserData(user));
        }

        private string SerializeUserData(User user)
        {
            try
            {
                // To write to a file, create a StreamWriter object.  
                StreamWriter myWriter = new StreamWriter("C:/YoumuBotData/Users/" + user.username + ".xml");
                mySerializer.Serialize(myWriter, user);
                return $"{user.username}'s real name set to {user.realname}.";
            }
            catch (Exception)
            {
                return "Something went wrong.";
                throw;
            }
            
            
        }

        private Task<User> DeserializeUserDataAsync(string username)
        {
            return Task.Factory.StartNew(() => DeserializeUserData(username));
        }

        private User DeserializeUserData(string username)
        {
            try
            {
                StreamReader myReader = new StreamReader("C:/YoumuBotData/Users/" + username + ".xml");
                return (User)mySerializer.Deserialize(myReader);
            }
            catch (Exception)
            {
                return new User();
                throw;
            }
        }

        public YoumuBotCommand(DiscordModule module) : base(module) { }


    }

    [System.Serializable]
    public class User
    {
        public ulong uid { set; get; }
        public string realname { set; get; }
        public string username { set; get; }
    }
}
