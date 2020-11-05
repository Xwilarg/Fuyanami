using Discord.Commands;
using DiscordUtils;
using System.Threading.Tasks;

namespace Fuyanami.Module
{
    public class Communication : ModuleBase
    {
        [Command("Info")]
        public async Task Info()
        {
            await ReplyAsync(embed: Utils.GetBotInfo(Program.P.StartTime, "Fuyanami", Program.P.Client.CurrentUser));
        }
    }
}
