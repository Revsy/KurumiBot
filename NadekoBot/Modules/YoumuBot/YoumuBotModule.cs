using Discord.Commands;
using Discord.Modules;
using NadekoBot.Classes;
using NadekoBot.Classes.YoumuBot.Commands;
using NadekoBot.Extensions;
using NadekoBot.Modules.Permissions.Classes;
using System.Linq;

namespace NadekoBot.Modules.YoumuBot
{
    internal class YoumuBotModule : DiscordModule
    {
        public YoumuBotModule()
        {
            commands.Add(new YoumuBotCommand(this));
        }

        public override string Prefix { get; } = NadekoBot.Config.CommandPrefixes.YoumuBot;

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);
                commands.ForEach(com => com.Init(cgb));
            });
        }
    }
}
