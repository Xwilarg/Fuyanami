using Discord.Commands;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fuyanami.Module
{
    public class Systemctl : ModuleBase
    {
        [Command("Status")]
        public async Task Status(string name)
        {
            name = char.ToUpper(name[0]) + string.Join("", name.Skip(1)).ToLower();
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else if (name != "Sanara" && name != "Fuyanami" && name != "Pina" && name != "Yuuka")
                await ReplyAsync("Invalid program name");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        FileName = "sudo",
                        Arguments = "journalctl -n 10 -u " + name
                    }
                };
                p.Start();
                p.StandardInput.WriteLine(Program.SudoPassword);
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                await ReplyAsync($"```\n`{output}\n```");
            }
        }

        [Command("Restart")]
        public async Task Restart(string name)
        {
            name = char.ToUpper(name[0]) + string.Join("", name.Skip(1)).ToLower();
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else if (name != "Sanara" && name != "Fuyanami" && name != "Pina" && name != "Yuuka")
                await ReplyAsync("Invalid program name");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = "systemctl restart " + name
                    }
                };
                p.Start();
                p.WaitForExit();
                await ReplyAsync($"Done");
            }
        }
    }
}
