using Discord.Commands;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fuyanami.Module
{
    public class Systemctl : ModuleBase
    {
        [Command("Restart")]
        public async Task Restart(string name)
        {
            var program = Program.AllowedPrograms.FirstOrDefault(x => x.ToLower() == name.ToLower());
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else if (program == null)
                await ReplyAsync("Invalid program name");
            else
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = "systemctl restart " + program,
                        UseShellExecute = false,
                        RedirectStandardInput = true
                    }
                };
                p.Start();
                p.StandardInput.WriteLine(Program.SudoPassword);
                p.WaitForExit();
                await ReplyAsync($"Done");
            }
        }

        [Command("Status")]
        public async Task Status(string name)
        {
            var program = Program.AllowedPrograms.FirstOrDefault(x => x.ToLower() == name.ToLower());
            if (!Program.AllowedIds.Contains(Context.User.Id.ToString()))
                await ReplyAsync("You are not allowed to do this command.");
            else if (program == null)
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
                        Arguments = "systemctl status " + program
                    }
                };
                p.Start();
                p.StandardInput.WriteLine(Program.SudoPassword);
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                await ReplyAsync($"```\n`{output}\n```");
            }
        }
    }
}
