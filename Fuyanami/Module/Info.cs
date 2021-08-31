using Discord.Commands;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fuyanami.Module
{
    public class Info : ModuleBase
    {
        [Command("RAM")]
        public async Task RAM()
        {
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "free",
                        Arguments = "-h"
                    }
                };
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                await ReplyAsync($"```\n`{output}\n```");
            }
        }

        [Command("CPU")]
        public async Task CPU()
        {
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "top",
                        Arguments = "-b -n 1 -i"
                    }
                };
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                await ReplyAsync($"```\n`{output}\n```");
            }
        }

        [Command("Storage")]
        public async Task Memory()
        {
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "df"
                    }
                };
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                await ReplyAsync($"```\n`{output}\n```");
            }
        }
    }
}
